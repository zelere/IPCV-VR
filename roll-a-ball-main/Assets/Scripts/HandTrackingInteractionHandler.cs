using System;
using System.Collections.Generic;
using Leap;
using Leap.Unity;
using UnityEngine;



public class HandTrackingInteractionHandler : IInteractionHandler
{
    private LeapProvider leap;
    private BallBehaviour ball;

    [Header("Settings")]
    public float moveForce = 8.0f;
    public float pinchThreshold = 0.5f; // Lowered from 0.8f for easier pinching
    public float followSmoothness = 10f;
    public float pinchDistance = 40.0f;
    public float maxForce = 10f; // Maximum force to prevent overshooting
    public int requiredFrames = 5; // Number of frames hand must be present
    public int handLostFramesTolerance = 30; // Frames to wait before releasing when hand is lost
    public int rightHandLostFramesTolerance = 15; // Frames to wait before stopping ball when right hand is lost
    public float slowdownRate = 0.95f; // Rate at which ball slows down when hand is lost


    private bool isPinched = false;
    private Hand activeHand;
    private Vector3 pinchOffset;
    private int rightHandFrameCount = 0;
    private int leftHandLostFrameCount = 0;
    private int rightHandLostFrameCount = 0;
    private Vector3 lastKnownPalmPosition;
    private Vector3 previousRightHandPosition;
    private Vector3 rightHandVelocity;
    private bool hasValidRightHandPosition = false;
    private bool justReleasedFromPinch = false;
    private float releaseTime = 0f;
    private float releaseGracePeriod = 0.5f; // Time after release to not slow down

    public float smoothTime = 0.05f;

    public void EnterMode(BallBehaviour b)
    {
        ball = b;
        leap = UnityEngine.Object.FindFirstObjectByType<LeapProvider>();
        if (leap != null)
            leap.OnUpdateFrame += OnUpdateFrame;
        Debug.Log("Entered Hand Tracking mode");
    }

    public void ExitMode(BallBehaviour b)
    {
        if (leap != null)
            leap.OnUpdateFrame -= OnUpdateFrame;

        ReleasePinch();
        ball = null;
        Debug.Log("Exited Hand Tracking mode");
    }

    public void Update(BallBehaviour b) { }
    private void OnUpdateFrame(Frame frame)
    {
        if (frame == null || ball == null) return;

        var rightHand = frame.GetHand(Chirality.Right);
        var leftHand = frame.GetHand(Chirality.Left);


        HandlePinching(leftHand);

        bool inGracePeriod = justReleasedFromPinch && (Time.time - releaseTime) < releaseGracePeriod;

        // only allow pointing if not pinched and not within release grace period
        if (!isPinched && !inGracePeriod)
            HandlePointing(rightHand);

        //if (!isPinched)
        //    HandlePointing(rightHand);
    }



    private void HandlePointing(Hand rightHand)
    {

        //Track right hand presence over multiple frames
        if (rightHand != null)
        {

            rightHandFrameCount++;
            rightHandLostFrameCount = 0;

            Vector3 tipWorld = rightHand.GetFinger(Finger.FingerType.INDEX).TipPosition;
            if (hasValidRightHandPosition)
            {
                Vector3 delta = tipWorld - previousRightHandPosition;
                rightHandVelocity = delta / Time.deltaTime;
            }
            else
            {
                rightHandVelocity = Vector3.zero;
                hasValidRightHandPosition = true;
            }

            previousRightHandPosition = tipWorld;
        }
        else
        {
            rightHandFrameCount = 0;
            rightHandLostFrameCount++;
            hasValidRightHandPosition = false;
            rightHandVelocity = Vector3.zero;

            bool inGrace = justReleasedFromPinch && (Time.time - releaseTime) < releaseGracePeriod;
            if (rightHandLostFrameCount >= rightHandLostFramesTolerance && !inGrace)
            {
                ball?.SlowDown(slowdownRate);
                if (ball.rb.velocity.magnitude < 0.1f)
                    ball?.Stop();
            }
            return;
        }

        if (rightHandFrameCount >= requiredFrames)
        {

            Vector3 tipWorld = rightHand.GetFinger(Finger.FingerType.INDEX).TipPosition;

            Vector3 dir = tipWorld - ball.transform.position;
            float distance = dir.magnitude;

            float forceMag = Mathf.Min(distance * moveForce, maxForce);
            Vector3 force = dir.normalized * forceMag * Time.deltaTime;
            ball?.ApplyForce(force, ForceMode.VelocityChange);

            if (rightHandVelocity.magnitude > 0.05f)
            {
                Vector3 dragForce = rightHandVelocity * moveForce * 0.05f * Time.deltaTime;
                dragForce.y = Mathf.Min(dragForce.y, 0f);
                ball?.ApplyForce(dragForce, ForceMode.VelocityChange);
            }
        }
    }

    private void HandlePinching(Hand leftHand)
    {
        if (!isPinched)
        {
            // Reset lost frame count when not pinching
            leftHandLostFrameCount = 0;
            if (TryStartPinch(leftHand))
                ball.SetKinematic(false);
        }
        else
        {
            if (leftHand == null)
            {
                leftHandLostFrameCount++;
                Debug.Log($"Left hand lost for {leftHandLostFrameCount} frames");
                if (leftHandLostFrameCount >= handLostFramesTolerance)
                {
                    Debug.Log("Left hand lost for too long, releasing pinch");
                    ReleasePinch();
                    return;
                }
                // Keep ball at last known position while hand is temporarily lost
                Vector3 target = lastKnownPalmPosition + pinchOffset;
                ball?.MoveTo(target, followSmoothness * 0.5f);
            }
            else
            {
                // Hand is back, reset lost frame count
                leftHandLostFrameCount = 0;
                activeHand = leftHand; // Update active hand reference

                // Check if we should release the pinch based on pinch strength
                if (leftHand.PinchStrength < 0.2f)
                {
                    ReleasePinch();
                }
                else
                {
                    Vector3 palmWorld = leftHand.PalmPosition;
                    lastKnownPalmPosition = palmWorld; // Update last known position
                    Vector3 target = palmWorld + pinchOffset;
                    ball.MoveTo(target, followSmoothness);


                }
            }
        }
    }


    private bool TryStartPinch(Hand hand)
    {

        if (hand == null) return false;

        Vector3 palmWorld = hand.PalmPosition;

        float dist = Vector3.Distance(ball.transform.position, palmWorld);
        float pinchStrength = hand.PinchStrength;

        bool startPinch = pinchStrength > pinchThreshold && dist <= pinchDistance;
        if (startPinch)
        {
            isPinched = true;
            activeHand = hand;
            lastKnownPalmPosition = palmWorld;
            pinchOffset = ball.transform.position - palmWorld;
            //ball?.SetKinematic(true);
            return true;
        }

        return false;
    }

    private void ReleasePinch()
    {
        if (!isPinched) return;
        isPinched = false;
        activeHand = null;
        leftHandLostFrameCount = 0; // Reset lost frame count
        justReleasedFromPinch = true; // Mark that we just released from pinch
        releaseTime = Time.time; // Record release time
        ball?.SetKinematic(false);
        ball?.Stop();
        Debug.Log("Ball released");
    }
}
