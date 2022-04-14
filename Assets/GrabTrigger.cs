using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class GrabTrigger : MonoBehaviour
{
    // Unity's new XR Interaction Toolkit input handling. See instantiation in Start()
    private XRNode leftNode = XRNode.LeftHand;
    private XRNode rightNode = XRNode.RightHand;
    private InputDevice leftController;
    private InputDevice rightController;
    // used to fetch available devices
    private List<InputDevice> devices = new List<InputDevice>();

    // to track when an object is gripped
    private bool rightGripped = false;
    private bool leftGripped = false;

    // INSPECTOR INPUT
    // hand objects
    public GameObject rightHand;
    public GameObject leftHand;

    private bool rightHandOnObject = false;
    private bool leftHandOnObject = false;


    // Start is called before the first frame update
    void Trigger()
    {
        
    }


    // retrieve and assign controller devices
    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(leftNode, devices);
        Debug.Log(devices);
        Debug.Log("count: " + devices.Count);
        if (devices.Count > 0)
        {
            leftController = devices[0];
            Debug.Log("LEFT CONNECTED");
        }
        InputDevices.GetDevicesAtXRNode(rightNode, devices);
        if (devices.Count > 0)
        {
            rightController = devices[0];
            Debug.Log("RIGHT CONNECTED");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftController.isValid /*|| !rightController.isValid*/)
        {
            GetDevice();
        }

        OnTriggerStay();
    }

    private void OnTriggerStay()
    {
        // How far away the hand is allowed to be to trigger a grab.
        float minGrabDist = 0.5f;

        if (rightHandOnObject == false && rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightGripped) && rightGripped)
        {
            if ((rightHand.transform.position - this.transform.position).magnitude < minGrabDist) // check that right hand is close enough to grab
            {
                Trigger();
            }
        }
        if (leftHandOnObject == false && leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftGripped) && leftGripped)
        {
            if ((leftHand.transform.position - this.transform.position).magnitude < minGrabDist) // check that right hand is close enough to grab
            {
                Trigger();
            }
        }
    }
}