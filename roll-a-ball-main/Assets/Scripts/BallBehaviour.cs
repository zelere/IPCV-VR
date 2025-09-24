using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Leap;

public class BallBehaviour : MonoBehaviour
{
    private Rigidbody rb;
    public LeapProvider leapProvider;


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
        leapProvider.OnUpdateFrame += OnUpdateFrame;
    }
    private void OnDisable()
    {
        leapProvider.OnUpdateFrame -= OnUpdateFrame;
    }

    void OnUpdateFrame(Frame frame)
    {
        //Use a helpful utility function to get the first hand that matches the Chirality
        Hand _leftHand = frame.GetHand(Chirality.Left);

        //When we have a valid left hand, we can begin searching for more Hand information
        if(_leftHand != null)
        {
            OnUpdateHand(_leftHand);
            Debug.Log("We found a left hand !");

        }

        Hand _rightHand = frame.GetHand(Chirality.Right);

        if(_rightHand != null)
        {
            OnUpdateHand(_rightHand);
            Debug.Log("We found a right hand !");

        }


    }

    void OnUpdateHand(Hand _hand)
    {
        Finger _index = _index = _hand.Fingers[1];
        if(_index != null)
        {
            Debug.Log("Index found");
        }
    }
}
