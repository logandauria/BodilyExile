using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;


public class HapticSync : AudioSyncer
{

    // Unity's new XR Interaction Toolkit input handling. See instantiation in Start()
    private XRNode leftNode = XRNode.LeftHand;
    private XRNode rightNode = XRNode.RightHand;
    private InputDevice leftController;
    private InputDevice rightController;
    // used to fetch available devices
    private List<InputDevice> devices = new List<InputDevice>();

    // range from 0-1
    public float pulseStrength = 0;
    public bool useLeft = false;
    public bool useRight = false;

    // retrieve and assign controller devices
    void GetDevice()
    {
        InputDevices.GetDevicesAtXRNode(leftNode, devices);
        Debug.Log(devices);
        Debug.Log("count: " + devices.Count);
        if (devices.Count > 0 && useLeft)
        {
            leftController = devices[0];
            Debug.Log("LEFT CONNECTED");
        }
        InputDevices.GetDevicesAtXRNode(rightNode, devices);
        if (devices.Count > 0 && useRight)
        {
            rightController = devices[0];
            Debug.Log("RIGHT CONNECTED");
        }
    }

    public override void OnUpdate()
    {
        base.OnUpdate();
        if (!leftController.isValid && useLeft || !rightController.isValid && useRight)
            GetDevice();
    }


    public override void OnBeat()
    {
        base.OnBeat();
        if(useLeft) leftController.SendHapticImpulse(1, pulseStrength, 1);
        if (useRight) rightController.SendHapticImpulse(1, pulseStrength, 1);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    
}
