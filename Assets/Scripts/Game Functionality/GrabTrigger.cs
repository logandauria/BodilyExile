using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Used to trigger an event when an object is grabbed
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

    //public GameObject phaseToLoad;
    //public int levelToLoad;
    //public Material fadeMat;

    // INSPECTOR INPUT
    // hand objects
    public GameObject rightHand;
    public GameObject leftHand;
    // Event to be triggered on grab
    public UnityEvent myEvent;

    private bool rightHandOnObject = false;
    private bool leftHandOnObject = false;

    private bool grabbed = false;
    // Used to prevent trigger occuring more than once
    private bool triggerOnce = false;

    /// <summary>
    /// Calls the unity event once when the object is grabbed
    /// </summary>
    void Trigger()
    {
        grabbed = true;
        // only call the event once
        if (!triggerOnce)
        {
            myEvent.Invoke();
            triggerOnce = true;
        }
        //FadeToLevel();
        //transform.parent.gameObject.SetActive(false);
        //phaseToLoad.SetActive(true);
        //SceneManager.LoadScene(levelToLoad);
    }


    /// <summary>
    /// retrieve and assign controller devices
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
        if (!leftController.isValid /*|| !rightController.isValid*/)
        {
            GetDevice();
        }
        if(!grabbed) OnTriggerStay();
    }

    /// <summary>
    /// Detect when the object is grabbed, call the Trigger() method
    /// </summary>
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