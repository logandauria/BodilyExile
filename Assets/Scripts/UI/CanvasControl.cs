using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

// Used to toggle the appearance of a UI canvas with a controller button
public class CanvasControl : MonoBehaviour
{
    // Unity's new XR Interaction Toolkit input handling. See instantiation in Start()
    private XRNode leftNode = XRNode.LeftHand;
    private XRNode rightNode = XRNode.RightHand;
    private InputDevice leftController;
    private InputDevice rightController;
    // used to fetch available devices
    private List<InputDevice> devices = new List<InputDevice>();

    // to track when the controller button is pressed
    private bool rightPressed = false;
    private bool leftPressed = false;

    private float timer = 0;
    // amount in seconds required between button presses
    private float pressTimeGap = 0.25f;

    /// <summary>
    /// Called when one of the buttons is triggered. Deactivates/Activates the canvas based on current state
    /// </summary>
    void Trigger()
    {
        Debug.Log("Canvas Trigger");
        if (this.transform.GetChild(0).gameObject.activeSelf) {
            this.transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            this.transform.GetChild(0).gameObject.SetActive(true);
        }
    }


    /// <summary>
    /// Retrieve and assign controller devices
    /// </summary>
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
        // Get devices if either controller is invalid
        if (!leftController.isValid || !rightController.isValid)
        {
            GetDevice();
        }

        // make sure button only gets pressed every 0.1 seconds
        if(timer > pressTimeGap) OnTriggerStay();

        timer += Time.deltaTime;
    }

    /// <summary>
    /// Detect controller input of the primary button on either controller
    /// </summary>
    private void OnTriggerStay()
    {
        if (rightController.TryGetFeatureValue(CommonUsages.primaryButton, out rightPressed) && rightPressed)
        {
            Trigger();
            timer = 0;
        }
        if (leftController.TryGetFeatureValue(CommonUsages.primaryButton, out leftPressed) && leftPressed)
        {
            Trigger();
            timer = 0;
        }
    }
}