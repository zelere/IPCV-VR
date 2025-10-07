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
    public Chirality controllingHand = Chirality.Right;
    public float moveForce = 5.0f;
    public float pinchThreshold = 0.8f;
    public float followSmoothness = 10f;
    public float pinchDistance = 0.1f;

    private bool isPinched = false;
    private Hand activeHand;
    private Vector3 pinchOffset;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float z = Input.GetAxis("Vertical");
        float x = Input.GetAxis("Horizontal");
        Vector3 forces = new Vector3(x, 0, z);
        rb.AddForce(0.5f * forces);
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
        if (frame == null) return;

        
        Hand hand = frame.GetHand(controllingHand);
        if (hand == null) return;

        Debug.Log($"We found a {controllingHand.ToString().ToLower()} hand!");

        
        if (!TryGetIndexTipPosition(hand, out Vector3 tipWorld))
        {
           
            Debug.Log("BallBehaviour: couldn't read fingertip/palm position via reflection.");
            return;
        }

        float dist = Vector3.Distance(transform.position, tipWorld);

        
        bool pinchStrengthAvailable = TryGetPinchStrength(hand, out float pinchStrength);
        if (!isPinched)
        {
            bool startPinch = false;

            if (pinchStrengthAvailable)
            {

                startPinch = pinchStrength > pinchThreshold && dist <= pinchDistance * 1.5f;
            }
            else
            {

                startPinch = dist <= pinchDistance;
            }

            if (startPinch)
            {
                isPinched = true;
                activeHand = hand;
                pinchOffset = transform.position - tipWorld;
                if (rb != null) rb.isKinematic = true;
                Debug.Log("Ball pinched / picked up.");
                return;
            }
        }
        else
        {
            // Currently pinched: update follow or release
            bool release = false;
            if (pinchStrengthAvailable)
            {
                if (pinchStrength < pinchThreshold * 0.7f) release = true;
            }
            else
            {
                if (dist > pinchDistance * 1.6f) release = true;
            }

            if (release)
            {
                isPinched = false;
                activeHand = null;
                if (rb != null)
                {
                    rb.isKinematic = false;
                    rb.velocity = Vector3.zero;
                }
                Debug.Log("Ball released.");
            }
            else
            {
                // follow hand while pinched
                Vector3 target = tipWorld + pinchOffset;
                transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSmoothness);
                return;
            }
        }

        // ----------- MOVE TOWARD FINGER  -----------
        if (!isPinched && rb != null)
        {
            Vector3 dir = (tipWorld - transform.position);
            rb.AddForce(dir * moveForce * Time.deltaTime, ForceMode.VelocityChange);
        }
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

        Type handType = hand.GetType();

        // 1) Try common helper methods: GetIndex(), GetIndexFinger(), GetIndexTip()
        string[] fingerMethodNames = new[] { "GetIndex", "GetIndexFinger", "GetFinger" };
        foreach (var mName in fingerMethodNames)
        {
            MethodInfo mi = handType.GetMethod(mName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (mi != null)
            {
                try
                {
                    object fingerObj = null;
                    // If method signature requires an int (GetFinger), try with 1 (index)
                    var parms = mi.GetParameters();
                    if (parms.Length == 0) fingerObj = mi.Invoke(hand, null);
                    else if (parms.Length == 1)
                    {
                        // Try passing int 1
                        try { fingerObj = mi.Invoke(hand, new object[] { 1 }); }
                        catch { fingerObj = mi.Invoke(hand, new object[] { (object)1 }); }
                    }
                    if (fingerObj != null && TryExtractTipFromFingerObject(fingerObj, out tipWorld)) return true;
                }
                catch { /* ignore runtime invoke errors, try next method */ }
            }
        }

        // 2) Try access "Fingers" collection (List/Array/IEnumerable) and take second element (index = 1)
        object fingersCol = GetMemberValue(hand, "Fingers") ?? GetMemberValue(hand, "fingers");
        if (fingersCol != null && fingersCol is System.Collections.IEnumerable)
        {
            try
            {
                var en = ((System.Collections.IEnumerable)fingersCol).GetEnumerator();
                int i = 0;
                object second = null;
                while (en.MoveNext())
                {
                    if (i == 1) { second = en.Current; break; }
                    i++;
                }
                if (second != null && TryExtractTipFromFingerObject(second, out tipWorld)) return true;
            }
            catch { /* ignore and continue to palm fallback */ }
        }

        // 3) As fallback, try palm position (PalmPosition or Palm)
        object palmObj = GetMemberValue(hand, "PalmPosition") ?? GetMemberValue(hand, "Palm") ?? GetMemberValue(hand, "StabilizedPalmPosition");
        if (palmObj != null && TryConvertToVector3(palmObj, out tipWorld))
        {
            return true;
        }

        return false;
    }

    bool TryExtractTipFromFingerObject(object fingerObj, out Vector3 tipWorld)
    {
        tipWorld = Vector3.zero;
        if (fingerObj == null) return false;

        // Try TipPosition, StabilizedTipPosition, Tip
        object tipPosObj = GetMemberValue(fingerObj, "TipPosition") ?? GetMemberValue(fingerObj, "StabilizedTipPosition") ?? GetMemberValue(fingerObj, "Tip");
        if (tipPosObj != null && TryConvertToVector3(tipPosObj, out tipWorld)) return true;

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
