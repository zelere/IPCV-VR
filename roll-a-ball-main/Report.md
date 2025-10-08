# Leap Motion Interaction Techniques: Robust Strategies for Unity

## 1. Limitations Identified

- **SDK Version Differences:** Leap Motion API changes can affect method/property names (`GetIndexFinger`, `Fingers`, `TipPosition`, etc.). Reflection is used to handle these, but is error-prone.
- **Missing/Incomplete Data:** Sometimes fingertip or pinch strength fields aren’t available, requiring fallback to palm or other heuristics.
- **Gesture Ambiguity:** Pinch detection is simple, and may cause false positives or negatives if hand tracking is noisy.

## 2. Proposed Robust Interaction Techniques

### A. **Palm-Based Gestures for Object Manipulation**
**Technique:** Instead of relying solely on index fingertip position and pinch strength, allow users to interact with objects using their palm’s proximity and orientation.
- **Implementation:** Use palm position (always available as a fallback) and its facing direction to "hover" over objects. When the palm is close and facing upward, the object can be lifted.
- **Justification:** Palm position and orientation are consistently reported by Leap Motion, and do not depend on finger data, making this more robust.

### B. **Thumb-Index Pinch Angle Detection**
**Technique:** Use the angle between thumb and index fingers, or their proximity, to detect a pinch, instead of relying only on pinch strength.
- **Implementation:** Measure the distance and angle between thumb and index tip positions. If they are close and the angle is appropriate, trigger a pinch.
- **Justification:** This combines spatial geometry and distance, reducing false positives, and can be implemented using multiple fallback methods for position extraction.


### C. **Open-Hand "Push" and "Pull" Gestures**

**Technique:** When the hand is open and moving toward/away from an object, interpret this as a push or pull interaction.

- **Implementation:** Track palm velocity and distance to the object. If the palm moves rapidly toward the object, apply a force (push). If away, attract the object (pull).

- **Justification:** Palm velocity and position are robustly available, and this gesture is intuitive and doesn’t depend on precise finger data.

### D. **Hand Closure Percentage for Grasp**
**Technique:** Use the "GrabStrength" or compute a closure percentage (number of fingers curled) to trigger grasp/release events.
- **Implementation:** If "GrabStrength" isn't available, estimate using finger angles (if accessible via reflection) or fallback to palm proximity.
- **Justification:** This avoids reliance on pinch strength, works with more SDK versions, and is robust to missing data.

## 3. Example Implementation: Palm Hover and Push

Below is a Unity C# script snippet that implements palm-based hover and push gestures, using robust reflection-based extraction similar to your current style.

```csharp name=LeapPalmPushBehaviour.cs
using System;
using System.Reflection;
using Leap;
using UnityEngine;

public class LeapPalmPushBehaviour : MonoBehaviour
{
    public LeapProvider leapProvider;
    public Chirality controllingHand = Chirality.Right;
    public float hoverDistance = 0.2f;
    public float pushVelocityThreshold = 0.3f;
    public float pushForce = 5f;
    private Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    void OnEnable()
    {
        if (leapProvider != null) leapProvider.OnUpdateFrame += OnUpdateFrame;
    }
    void OnDisable()
    {
        if (leapProvider != null) leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame)
    {
        if (frame == null) return;
        Hand hand = frame.GetHand(controllingHand);
        if (hand == null) return;

        Vector3 palmPos;
        if (!TryGetPalmPosition(hand, out palmPos)) return;

        float dist = Vector3.Distance(transform.position, palmPos);

        // Hover highlight
        if (dist < hoverDistance)
        {
            // e.g., change color to highlight
            GetComponent<Renderer>().material.color = Color.yellow;

            Vector3 palmVel;
            if (TryGetPalmVelocity(hand, out palmVel))
            {
                // If palm moves quickly toward the object, apply push
                Vector3 toObject = (transform.position - palmPos).normalized;
                float velToward = Vector3.Dot(palmVel, toObject);
                if (velToward > pushVelocityThreshold)
                {
                    rb.AddForce(toObject * pushForce, ForceMode.VelocityChange);
                }
            }
        }
        else
        {
            // Remove highlight
            GetComponent<Renderer>().material.color = Color.white;
        }
    }

    bool TryGetPalmPosition(Hand hand, out Vector3 palmWorld)
    {
        palmWorld = Vector3.zero;
        object palmObj = GetMemberValue(hand, "PalmPosition") ?? GetMemberValue(hand, "Palm") ?? GetMemberValue(hand, "StabilizedPalmPosition");
        return palmObj != null && TryConvertToVector3(palmObj, out palmWorld);
    }

    bool TryGetPalmVelocity(Hand hand, out Vector3 palmVel)
    {
        palmVel = Vector3.zero;
        object velObj = GetMemberValue(hand, "PalmVelocity") ?? GetMemberValue(hand, "Velocity");
        return velObj != null && TryConvertToVector3(velObj, out palmVel);
    }

    // Use similar TryConvertToVector3 and GetMemberValue as in your snippet
    bool TryConvertToVector3(object vecObj, out Vector3 v)
    {
        v = Vector3.zero;
        if (vecObj == null) return false;
        if (vecObj is Vector3 vec3) { v = vec3; return true; }
        Type t = vecObj.GetType();
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
        PropertyInfo pi = t.GetProperty(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (pi != null) { try { return pi.GetValue(obj); } catch { } }
        FieldInfo fi = t.GetField(memberName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (fi != null) { try { return fi.GetValue(obj); } catch { } }
        return null;
    }
}
```

## 4. Conclusion

By relying on palm-based data, angle/distance between thumb and index, and hand closure metrics, you make your interactions more robust to incomplete or varied Leap Motion data. These techniques are more reliable, intuitive, and portable across SDK versions—leading to a better user experience.

---

**Next Steps:**  
- Implement the example script in your Unity project, attach it to an interactable object, and test palm-based hover and push.
- Extend to other proposed gestures as needed.