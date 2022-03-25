using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BallMovement : MonoBehaviour
{
    // Unity's new XR Interaction Toolkit input handling. See instantiation in Start()
    [SerializeField]
    private XRNode leftNode = XRNode.LeftHand;
    private XRNode rightNode = XRNode.RightHand;
    private InputDevice leftController;
    private InputDevice rightController;
    // used to fetch available devices
    private List<InputDevice> devices = new List<InputDevice>();

    // to track when an object is gripped
    private bool rightGripped = false;
    private bool leftGripped = false;

    // to track the initial position of the hands when they grab the object
    private Vector3 initRightHandPos;
    private Vector3 initLeftHandPos;
    // to track initial scale
    private Vector3 initScale;

    // INSPECTOR INPUT
    // hand objects
    public GameObject rightHand;
    public GameObject leftHand;
    // object transform array of grabbable positions
    public Transform[] snapPositions;
    public Transform directonalObject;

    private bool rightHandOnObject = false;
    private bool leftHandOnObject = false;
    private Transform leftHandOriginalParent;
    private Transform rightHandOriginalParent;

    private Vector3 initPos;
    private Vector3 initRot;
    

    // Start is called before the first frame update
    void Start()
    {
        initPos = this.transform.position;
        initRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);

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

        ReleaseHandsFromObject();

        //ConvertRotation();

        ConvertScale();

        //TurnVehicle();


        transform.position = initPos;
        transform.eulerAngles = new Vector3(initRot.x, initRot.y, transform.eulerAngles.z);
    }


    // Changes the scale of the object based on the distance value between the two hands
    private void ConvertScale()
    {
        // Check that both hands are grabbing a snap position on the object
        if (rightHandOnObject == true && leftHandOnObject == true)
        {
            this.transform.parent = directonalObject;
            // current hand distances - initial hand distances
            float diff = Vector3.Distance(initRightHandPos, initLeftHandPos) - Vector3.Distance(rightHand.transform.position, leftHand.transform.position);
            this.transform.localScale += new Vector3(diff,diff,diff);
        }
    }

    private void ConvertRotation()
    {
        float initx = transform.eulerAngles.x;
        float inity = transform.eulerAngles.y;
        if(rightHandOnObject == true && leftHandOnObject == false)
        {
            Quaternion newRot = Quaternion.Euler(initx, inity, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            this.transform.rotation = newRot;
            this.transform.parent = directonalObject;
        } 
        else if (rightHandOnObject == false && leftHandOnObject == true)
        {
            Quaternion newRot = Quaternion.Euler(initx, inity, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            this.transform.rotation = newRot;
            this.transform.parent = directonalObject;
        } 
        else if (rightHandOnObject == true && leftHandOnObject == true)
        {
            Quaternion rightRot = Quaternion.Euler(initx, inity, rightHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion leftRot = Quaternion.Euler(initx, inity, leftHandOriginalParent.transform.rotation.eulerAngles.z);
            Quaternion finalRot = Quaternion.Slerp(leftRot, rightRot, 1.0f / 2.0f);
            directonalObject.rotation = finalRot;
            this.transform.parent = directonalObject;
        }
    }

    /*private void TurnVehicle()
    {
        var turn = -transform.rotation.eulerAngles.z;
        if(turn < -350)
        {
            turn = turn + 360;
        }
        VehicleRigidBody.MoveRotation(Quaternion.RotateTowards(Vehicle.transform.rotation, Quaternion.Euler(0, turn, 0), Time.deltaTime * ));
    }*/

    private void ReleaseHandsFromObject()
    {
        if (rightHandOnObject && rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripped) && !rightGripped)
        {
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.rotation = rightHandOriginalParent.rotation;
            rightHand.transform.localScale = rightHandOriginalParent.localScale;
            rightHandOnObject = false;
        }
        if (leftHandOnObject && leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripped) && !leftGripped)
        {
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.rotation = leftHandOriginalParent.rotation;
            leftHand.transform.localScale = leftHandOriginalParent.localScale;
            leftHandOnObject = false;
        }
        if(!leftHandOnObject && !leftHandOnObject)
        {
            transform.parent = null;
        }
    }

    private void OnTriggerStay()
    {

        if (rightHandOnObject == false && rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripped) && rightGripped)
        {
            if ((rightHand.transform.position - this.transform.position).magnitude < 1f)
            {

                PlaceHandOnObject(ref rightHand, ref rightHandOriginalParent, ref rightHandOnObject);
            }
        }
        if (leftHandOnObject == false && leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripped) && leftGripped)
        {
            if ((leftHand.transform.position - this.transform.position).magnitude < 1f)
            {
                PlaceHandOnObject(ref leftHand, ref leftHandOriginalParent, ref leftHandOnObject);
            }
        }

    }

    private void PlaceHandOnObject(ref GameObject hand, ref Transform originalParent, ref bool handOnWheel)
    {
         
        var shortestDistance = Vector3.Distance(snapPositions[0].position, hand.transform.position);
        var bestSnap = snapPositions[0];

        foreach (var snap in snapPositions)
        {
            if (snap.childCount == 0)
            {
                var distance = Vector3.Distance(snap.position, hand.transform.position);
                if(distance < shortestDistance)
                {
                    shortestDistance = distance;
                    bestSnap = snap;
                }
            }
        }
        originalParent = hand.transform.parent;

        hand.transform.parent = bestSnap.transform;
        hand.transform.position = bestSnap.transform.position;
        //hand.transform.scale

        handOnWheel = true;

        // update initial values for later calculation 
        initLeftHandPos = leftHand.transform.localPosition;
        initRightHandPos = rightHand.transform.localPosition;
        initScale = this.transform.localScale;
    }
}