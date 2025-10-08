using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Leap;
using UnityEngine;
using UnityEngine.UI;

public class BallBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public LeapProvider leapProvider;

    [Header("Settings")]
    public float moveForce = 1.0f; // Reduced from 5.0f for less sensitivity
    public float pinchThreshold = 0.5f; // Lowered from 0.8f for easier pinching
    public float followSmoothness = 10f;
    public float pinchDistance = 2.0f; 
    public float maxForce = 10f; // Maximum force to prevent overshooting
    public int requiredFrames = 5; // Number of frames hand must be present
    public int handLostFramesTolerance = 30; // Frames to wait before releasing when hand is lost
    public int rightHandLostFramesTolerance = 15; // Frames to wait before stopping ball when right hand is lost
    public float slowdownRate = 0.95f; // Rate at which ball slows down when hand is lost

    [Header("Keyboard Controls")]
    public float keyboardMoveForce = 4.0f; // Force applied when using keyboard (reduced from 8.0f)
    public float keyboardMaxSpeed = 8.0f; // Maximum speed when using keyboard (reduced from 15.0f)
    public bool enableKeyboardControls = true; // Toggle to enable/disable keyboard controls

    [Header("Interaction Mode")]
    public InteractionMode currentInteractionMode = InteractionMode.None;

    public enum InteractionMode
    {
        None,
        HandTracking,
        Keyboard
    }

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
    private float releaseGracePeriod = 2.0f; // Time after release to not slow down

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        // Handle interaction based on selected mode
        if (currentInteractionMode == InteractionMode.Keyboard)
        {
            HandleKeyboardInteraction();
        }
        // Hand tracking is handled in FixedUpdate through OnUpdateFrame
    }

    void FixedUpdate()
    {
        // Hand tracking interaction is processed here through the Leap Motion callbacks
        // This ensures consistent physics-based movement
    }

    public void SetInteractionMode(InteractionMode mode)
    {
        currentInteractionMode = mode;
        Debug.Log($"Interaction mode set to: {mode}");
        
        // Reset ball state when switching modes
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        
        // Release pinch if switching away from hand tracking
        if (mode != InteractionMode.HandTracking && isPinched)
        {
            ReleasePinch();
        }
    }

    void HandleKeyboardInteraction()
    {
        if (rb == null) return;

        Vector3 inputVector = Vector3.zero;

        // Get input for horizontal and vertical movement
        // WASD keys and Arrow keys
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            inputVector.z += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
            inputVector.z -= 1f;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            inputVector.x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            inputVector.x += 1f;

        // Normalize the input vector to prevent faster diagonal movement
        if (inputVector.magnitude > 1f)
            inputVector = inputVector.normalized;

        // Apply force if there's input
        if (inputVector != Vector3.zero)
        {
            // Check current speed and limit it
            Vector3 currentVelocity = rb.velocity;
            float currentSpeed = new Vector3(currentVelocity.x, 0, currentVelocity.z).magnitude;

            if (currentSpeed < keyboardMaxSpeed)
            {
                Vector3 force = inputVector * keyboardMoveForce;
                rb.AddForce(force, ForceMode.Force);
            }
        }
    }

    void HandleHandTrackingInteraction(Frame frame)
    {
        if (frame == null) return;

        // Get both hands
        Hand rightHand = frame.GetHand(Chirality.Right);
        Hand leftHand = frame.GetHand(Chirality.Left);

        // Handle pinching with left hand only
        HandlePinching(leftHand);

        // Handle pointing/movement with right hand only (but not when pinched)
        if (!isPinched)
        {
            HandlePointing(rightHand);
        }
    }

    private void OnEnable()
    {
        if (leapProvider != null)
            leapProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        if (leapProvider != null)
            leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }

  

    // Called every frame when Leap Motion has data
    void OnUpdateFrame(Frame frame)
    {
        // Only process hand tracking if that's the current interaction mode
        if (currentInteractionMode == InteractionMode.HandTracking)
        {
            HandleHandTrackingInteraction(frame);
        }
    }

    void HandlePinching(Hand leftHand)
    {
        if (!isPinched)
        {
            // Reset lost frame count when not pinching
            leftHandLostFrameCount = 0;
            
            // Try to start pinching with left hand only
            if (TryStartPinch(leftHand))
            {
                return;
            }
        }
        else
        {
            // Handle when left hand disappears during pinching
            if (leftHand == null)
            {
                leftHandLostFrameCount++;
                Debug.Log($"Left hand lost for {leftHandLostFrameCount} frames");
                
                // If hand is lost for too long, release the pinch
                if (leftHandLostFrameCount >= handLostFramesTolerance)
                {
                    Debug.Log("Left hand lost for too long, releasing pinch");
                    ReleasePinch();
                    return;
                }
                
                // Keep ball at last known position while hand is temporarily lost
                Vector3 target = lastKnownPalmPosition + pinchOffset;
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSmoothness * 0.5f);
            }
            else
            {
                // Hand is back, reset lost frame count
                leftHandLostFrameCount = 0;
                activeHand = leftHand; // Update active hand reference
                
                // Check if we should release the pinch based on pinch strength
                if (ShouldReleasePinch(leftHand))
                {
                    ReleasePinch();
                }
                else
                {
                    // Follow the pinching hand using palm position
                    if (TryGetPalmPosition(leftHand, out Vector3 palmWorld))
                    {
                        lastKnownPalmPosition = palmWorld; // Update last known position
                        Vector3 target = palmWorld + pinchOffset;
                        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSmoothness);
                    }
                }
            }
        }
    }

    void HandlePointing(Hand rightHand)
    {
        // Track right hand presence over multiple frames
        if (rightHand != null)
        {
            rightHandFrameCount++;
            rightHandLostFrameCount = 0; // Reset lost frame count when hand is present
            
            // Calculate right hand velocity for drag effect
            if (TryGetIndexTipPosition(rightHand, out Vector3 tipWorld))
            {
                if (hasValidRightHandPosition)
                {
                    Vector3 deltaPosition = tipWorld - previousRightHandPosition;
                    rightHandVelocity = deltaPosition / Time.deltaTime;
                }
                else
                {
                    rightHandVelocity = Vector3.zero;
                    hasValidRightHandPosition = true;
                }
                previousRightHandPosition = tipWorld;
            }
        }
        else
        {
            rightHandFrameCount = 0;
            rightHandLostFrameCount++;
            hasValidRightHandPosition = false;
            rightHandVelocity = Vector3.zero;
            
            // Check if we're in grace period after pinch release
            bool inGracePeriod = justReleasedFromPinch && (Time.time - releaseTime) < releaseGracePeriod;
            
            // Gradually slow down the ball if right hand has been lost (but not during grace period)
            if (rightHandLostFrameCount >= rightHandLostFramesTolerance && rb != null && !inGracePeriod)
            {
                rb.velocity *= slowdownRate;
                rb.angularVelocity *= slowdownRate;
                
                // Stop completely if velocity is very low
                if (rb.velocity.magnitude < 0.1f)
                {
                    rb.velocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
            return;
        }

        // Only move ball if right hand has been present for required frames
        if (rightHandFrameCount >= requiredFrames)
        {
            if (TryGetIndexTipPosition(rightHand, out Vector3 tipWorld))
            {
                // Move toward finger tip
                Vector3 dir = (tipWorld - transform.position);
                float distance = dir.magnitude;
                
                // Reduce force based on distance to prevent overshooting
                float forceMagnitude = Mathf.Min(distance * moveForce, maxForce);
                
                // Apply smoother force with distance damping
                Vector3 force = dir.normalized * forceMagnitude * Time.deltaTime;
                rb.AddForce(force, ForceMode.VelocityChange);
                
                // Add drag force in the direction of hand movement
                if (rightHandVelocity.magnitude > 0.05f) // Only apply if hand is moving
                {
                    Vector3 dragForce = rightHandVelocity * moveForce * 0.05f * Time.deltaTime;
                    
                    // Limit upward drag to prevent lifting ball too high
                    if (dragForce.y > 0)
                    {
                        dragForce.y = Mathf.Min(dragForce.y, 0.0f); // Cap upward force
                    }
                    
                    rb.AddForce(dragForce, ForceMode.VelocityChange);
                }
            }
        }
    }

    bool TryStartPinch(Hand hand)
    {
        if (hand == null) return false;

        // Use palm position for more reliable detection
        if (!TryGetPalmPosition(hand, out Vector3 palmWorld)) return false;

        float dist = Vector3.Distance(transform.position, palmWorld);
        bool startPinch = false;

        bool pinchStrengthAvailable = TryGetPinchStrength(hand, out float pinchStrength);
        if (pinchStrengthAvailable)
        {
            // Lower threshold and larger distance for easier pickup
            startPinch = (pinchStrength > pinchThreshold) && dist <= (pinchDistance );
            Debug.Log($"Pinch strength: {pinchStrength}, Distance: {dist}, Threshold: {pinchThreshold}, pinchDistance: {pinchDistance}, startPinch: {startPinch}");
        }
        else
        {
            // Fallback: just use distance
            startPinch = dist <= pinchDistance * 1.5f;
            Debug.Log($"No pinch strength available, using distance only: {dist}");
        }

        if (startPinch)
        {
            isPinched = true;
            activeHand = hand;
            lastKnownPalmPosition = palmWorld; // Store initial palm position
            leftHandLostFrameCount = 0; // Reset lost frame count
            pinchOffset = Vector3.up * -0.05f; // Lift ball 5cm below palm
            if (rb != null) rb.isKinematic = true;
            Debug.Log("Ball pinched successfully!");
            return true;
        }

        return false;
    }

    void ReleasePinch()
    {
        isPinched = false;
        activeHand = null;
        leftHandLostFrameCount = 0; // Reset lost frame count
        justReleasedFromPinch = true; // Mark that we just released from pinch
        releaseTime = Time.time; // Record release time
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
        }
        Debug.Log("Ball released!");
    }

    bool ShouldReleasePinch(Hand hand)
    {
        if (!TryGetPalmPosition(hand, out Vector3 palmWorld)) return true;

        float dist = Vector3.Distance(transform.position, palmWorld);
        
        bool pinchStrengthAvailable = TryGetPinchStrength(hand, out float pinchStrength);
        if (pinchStrengthAvailable)
        {
            // Lower release threshold for easier release
            bool shouldRelease = pinchStrength < 0.2f;
            Debug.Log($"Release check - Pinch strength: {pinchStrength}, Should release: {shouldRelease}");
            return shouldRelease;
        }
        else
        {
            return dist > pinchDistance * 3.0f; // Larger range to prevent accidental release
        }
    }

    bool TryGetPalmPosition(Hand hand, out Vector3 palmWorld)
    {
        palmWorld = Vector3.zero;
        if (hand == null) return false;

        // Try to get palm position
        object palmPosObj = GetMemberValue(hand, "PalmPosition") ?? GetMemberValue(hand, "palmPosition");
        if (palmPosObj != null && TryConvertToVector3(palmPosObj, out palmWorld))
        {
            return true;
        }

        // Fallback to index finger tip if palm not available
        return TryGetIndexTipPosition(hand, out palmWorld);
    }

    bool TryGetPinchStrength(Hand hand, out float pinchStrength)
    {
        pinchStrength = 0f;
        object val = GetMemberValue(hand, "PinchStrength") ?? GetMemberValue(hand, "pinchStrength") ?? GetMemberValue(hand, "GrabStrength");
        if (val == null) return false;
        try
        {
            pinchStrength = Convert.ToSingle(val);
            return true;
        }
        catch { return false; }
    }

    bool TryGetIndexTipPosition(Hand hand, out Vector3 tipWorld)
    {
        tipWorld = Vector3.zero;
        if (hand == null) return false;

        // Access "Fingers" collection and get the index finger (element 1)
        object fingersCol = GetMemberValue(hand, "Fingers") ?? GetMemberValue(hand, "fingers");
        if (fingersCol != null && fingersCol is System.Collections.IEnumerable)
        {
            try
            {
                var en = ((System.Collections.IEnumerable)fingersCol).GetEnumerator();
                int i = 0;
                object indexFinger = null;
                while (en.MoveNext())
                {
                    if (i == 1) { indexFinger = en.Current; break; }
                    i++;
                }
                if (indexFinger != null && TryExtractTipFromFingerObject(indexFinger, out tipWorld)) 
                {
                    return true;
                }
            }
            catch (Exception e) 
            { 
                Debug.Log($"Exception accessing Fingers collection: {e.Message}");
            }
        }

        return false;
    }

    bool TryExtractTipFromFingerObject(object fingerObj, out Vector3 tipWorld)
    {
        tipWorld = Vector3.zero;
        if (fingerObj == null) return false;

        // Get the TipPosition property from the finger
        object tipPosObj = GetMemberValue(fingerObj, "TipPosition");
        if (tipPosObj != null && TryConvertToVector3(tipPosObj, out tipWorld)) 
        {
            return true;
        }

        return false;
    }

    bool TryConvertToVector3(object vecObj, out Vector3 v)
    {
        v = Vector3.zero;
        if (vecObj == null) return false;

        // If it's already a Unity Vector3
        if (vecObj is Vector3 vec3)
        {
            v = vec3;
            return true;
        }

        Type t = vecObj.GetType();

        // Try calling ToVector3() if present
        MethodInfo toVec = t.GetMethod("ToVector3", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (toVec != null)
        {
            try
            {
                object result = toVec.Invoke(vecObj, null);
                if (result is Vector3 r) { v = r; return true; }
            }
            catch { }
        }

        // Try reading x,y,z fields/properties (Leap.Vector or similar)
        object ox = GetMemberValue(vecObj, "x") ?? GetMemberValue(vecObj, "X");
        object oy = GetMemberValue(vecObj, "y") ?? GetMemberValue(vecObj, "Y");
        object oz = GetMemberValue(vecObj, "z") ?? GetMemberValue(vecObj, "Z");

        if (ox != null && oy != null && oz != null)
        {
            try
            {
                float fx = Convert.ToSingle(ox);
                float fy = Convert.ToSingle(oy);
                float fz = Convert.ToSingle(oz);

                // Leap's Vector is typically in millimeters — convert to meters for Unity units
                v = new Vector3(fx, fy, fz) * 0.001f;
                return true;
            }
            catch { }
        }

        return false;
    }

    object GetMemberValue(object obj, string memberName)
    {
        if (obj == null || string.IsNullOrEmpty(memberName)) return null;
        Type t = obj.GetType();

        // Try property
        PropertyInfo pi = t.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (pi != null)
        {
            try { return pi.GetValue(obj); } catch { }
        }

        // Try field
        FieldInfo fi = t.GetField(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (fi != null)
        {
            try { return fi.GetValue(obj); } catch { }
        }

        return null;
    }
}
