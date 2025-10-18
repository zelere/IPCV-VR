using Leap;
using UnityEngine;
using System.Collections.Generic;

public class BallBehaviour : MonoBehaviour
{
    public Rigidbody rb;
    public LeapProvider leapProvider;


    [Header("Movement Settings")]
    public float followSmoothness = 10f;
    public float slowdownRate = 0.95f;
    private bool isKinematic = false;

    public float keyboardMoveForce = 4.0f; // Force applied when using keyboard (reduced from 8.0f)
    public float keyboardMaxSpeed = 8.0f; // Maximum speed when using keyboard (reduced from 15.0f)
    public bool enableKeyboardControls = true; // Toggle to enable/disable keyboard controls


    // --- Interaction System ---
    public enum InteractionMode { None, Keyboard, HandTracking }
    public InteractionMode currentMode = InteractionMode.None;

    private IInteractionHandler currentHandler;
    private readonly Dictionary<InteractionMode, IInteractionHandler> handlers = new();


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        handlers[InteractionMode.Keyboard] = new KeyboardInteractionHandler();
        handlers[InteractionMode.HandTracking] = new HandTrackingInteractionHandler();
    }

    void Start()
    {
    }

    void FixedUpdate()
    {
        currentHandler?.Update(this);
    }


    public void SetInteractionMode(InteractionMode mode)
    {
        if (mode == currentMode) return;

        currentHandler?.ExitMode(this);
        currentMode = mode;

        if (handlers.TryGetValue(mode, out var h))
        {
            currentHandler = h;
            currentHandler.EnterMode(this);
            Debug.Log($"Successfully set interaction mode to: {mode}");
        }
        else 
        {
            currentHandler = null;
            Debug.LogError($"No handler found for interaction mode: {mode}");
        }

        Stop();
        
        // Additional debug info for hand tracking
        if (mode == InteractionMode.HandTracking)
        {
            if (leapProvider == null)
            {
                leapProvider = FindFirstObjectByType<LeapProvider>();
                if (leapProvider == null)
                {
                    Debug.LogError("LeapProvider not found! Hand tracking will not work. Please ensure Ultraleap SDK is properly set up.");
                }
                else
                {
                    Debug.Log("LeapProvider found: " + leapProvider.name);
                }
            }
        }
    }
    public void ApplyForce(Vector3 force, ForceMode forcemode)
    {
        if (rb == null || isKinematic) return;
        rb.AddForce(force, forcemode);
    }

    public void MoveTo(Vector3 target, float smoothness)
    {
        transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * smoothness);
    }

    public void SetKinematic(bool value)
    {
        if (rb == null) return;
        isKinematic = value;
        rb.isKinematic = value;
    }

    public void Stop()
    {
        if (rb == null) return;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    public void SlowDown(float rate)
    {
        if (rb == null) return;
        rb.velocity *= rate;
        rb.angularVelocity *= rate;
    }
}

