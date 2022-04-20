using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.XR;
using UnityEngine.Events;


public class SoulControl : MonoBehaviour
{

    // Unity's new XR Interaction Toolkit input handling. See instantiation in Start()
    private XRNode leftNode = XRNode.LeftHand;
    private XRNode rightNode = XRNode.RightHand;
    private InputDevice leftController;
    private InputDevice rightController;
    // used to fetch available devices
    private List<InputDevice> devices = new List<InputDevice>();

    VisualEffect soul = new VisualEffect();
    public VisualEffect soulLines;
    public UnityEvent extractionComplete;
    public UnityEvent nextPhase;
    private float zScale = 0;
    private float initScale = 0;
    private int rate = 0;

    private bool triggerOnce = false;
    private bool triggerOnce2 = false;

    public float timer = 0;

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

    // sense the axis of the controller
    private void OnTriggerStay()
    {
        // How far away the hand is allowed to be to trigger a grab.

        //var axis;

        if (rightController.TryGetFeatureValue(CommonUsages.primary2DAxis, out var axis))
        {
            //Debug.Log("AXIS X: " + axis.x);
            //Debug.Log("AXIS Y: " + axis.y);
            if(axis.y > 0.7f && zScale < 0.8)
            {
                zScale += 0.001f;
            } else if(axis.y > 0.7f && zScale > 0.8)
            {
                if (rate < 80) rate += 1;
                else if (rate < 800) rate += 5;
                else rate += 100;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        soul = GetComponent<VisualEffect>();
        initScale = soul.GetFloat("zscale");
    }

    // Update is called once per frame
    void Update()
    {
        if (!leftController.isValid /*|| !rightController.isValid*/)
        {
            GetDevice();
        }
        OnTriggerStay();
        soul.SetFloat("zscale", initScale + zScale);
        soulLines.SetInt("rate", rate);
        Debug.Log("rate: " + rate);
        if(rate > 50000 && !triggerOnce)
        {
            extractionComplete.Invoke();
            triggerOnce = true;
            Debug.Log("rate exceeded");
        }
        if(rate > 50000)
        {
            timer += Time.deltaTime;
        }
        if (timer > 10 && !triggerOnce2)
        {
            triggerOnce2 = true;
            nextPhase.Invoke();
        }
    }
}
