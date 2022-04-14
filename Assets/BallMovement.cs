using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

public class BallMovement : MonoBehaviour
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
    private Rigidbody rb;
    private Vector3 newRot = new Vector3(0, 0, 0);
    private Vector3 previousPos;
    public Vector3 velocity = new Vector3(0, 0, 0);

    private float comebackSpeed = 1f;
    private float throwSpeed = 3.5f;

    private bool needToLetGo = false;

    // Start is called before the first frame update
    void Start()
    {
        // initialize needed variables
        initPos = this.transform.position;
        initRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        initScale = this.transform.localScale;
        rb = GetComponent<Rigidbody>();
        previousPos = this.transform.position;
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

    private void OnLetGo()
    {
        // update initpos to position ball was thrown from
        initPos = transform.position;
        rightHand.SetActive(true);
        leftHand.SetActive(true);
        needToLetGo = false;
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

        ControlBall();

        //TurnVehicle();

        this.transform.eulerAngles += newRot;


        if (rightHandOnObject == false && leftHandOnObject == false)
        {
            this.transform.position += velocity;
            newRot = Vector3.Lerp(newRot, new Vector3(0, 0, 0), Time.deltaTime / 5);
            velocity = Vector3.Lerp(velocity, new Vector3(0, 0, 0), Time.deltaTime);
            // slowly revert to initial pos and scale
            transform.localScale = Vector3.Lerp(transform.localScale, initScale, Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, initPos, Time.deltaTime/comebackSpeed);
        }
        else
        {
            velocity = (transform.position - previousPos) / (Time.deltaTime * throwSpeed);
            previousPos = this.transform.position;
        }
        //transform.eulerAngles = new Vector3(initRot.x, initRot.y, transform.eulerAngles.z);
    }


    // Changes the scale of the object based on the x distance value between the two hands
    // Changes the rotation speed of the object based on the z distance value between the two hands
    private void ControlBall()
    {

        // Check that both hands are grabbing a snap position on the object
        if (rightHandOnObject == true && leftHandOnObject == true)
        {
            this.transform.position = (rightHand.transform.position + leftHand.transform.position) / 2;
            

            // current hand distances - initial hand distances for x and z
            float xDiff = Mathf.Abs(initRightHandPos.x - initLeftHandPos.x) - Mathf.Abs(rightHand.transform.position.x - leftHand.transform.position.x);
            float zDiff = Mathf.Abs(initRightHandPos.z - initLeftHandPos.z) - Mathf.Abs(rightHand.transform.position.z - leftHand.transform.position.z);

            // apply x difference of hands to the scale
            this.transform.localScale = initScale - new Vector3(xDiff / 2, xDiff / 2, xDiff / 2);

            // apply z difference of hands to the rotation
            newRot = new Vector3(0, .5f - zDiff * 0.5f, 0); // 3 before

            leftController.SendHapticImpulse(1, 0.1f);
            rightController.SendHapticImpulse(1, 0.1f);


        }
        else if (rightHandOnObject == false && leftHandOnObject == true)
        {
            this.transform.eulerAngles = leftHand.transform.eulerAngles;
            this.transform.position = Vector3.Lerp(this.transform.position, leftHand.transform.position, Time.deltaTime*10);

            needToLetGo = true;
            leftController.SendHapticImpulse(1, 0.1f);


        }
        else if (rightHandOnObject == true && leftHandOnObject == false)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, rightHand.transform.position, Time.deltaTime*10);
            this.transform.eulerAngles = rightHand.transform.eulerAngles;

            needToLetGo = true;
            leftController.SendHapticImpulse(1, 0.1f);

        }
        else
        {
            if(needToLetGo) OnLetGo();
        }

    }

    /*private void ConvertRotation()
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

    private void TurnVehicle()
    {
        var turn = -transform.rotation.eulerAngles.z;
        if(turn < -350)
        {
            turn = turn + 360;
        }
        VehicleRigidBody.MoveRotation(Quaternion.RotateTowards(Vehicle.transform.rotation, Quaternion.Euler(0, turn, 0), Time.deltaTime * ));
    }*/


    // Catches when hand(s) are released from the object
    private void ReleaseHandsFromObject()
    {
        if (rightHandOnObject && rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripped) && !rightGripped)
        {
            // fix right hand
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.rotation = rightHandOriginalParent.rotation;
            rightHand.transform.localScale = rightHandOriginalParent.localScale;
            rightHandOnObject = false;
        }
        if (leftHandOnObject && leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripped) && !leftGripped)
        {
            // fix left hand
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.rotation = leftHandOriginalParent.rotation;
            leftHand.transform.localScale = leftHandOriginalParent.localScale;
            leftHandOnObject = false;
        }
        if (!leftHandOnObject && !leftHandOnObject)
        {
            transform.parent = null;
        }
    }

    private void OnTriggerStay()
    {
        // How far away the hand is allowed to be to trigger a grab.
        float minGrabDist = 0.8f;

        if (rightHandOnObject == false && rightController.TryGetFeatureValue(CommonUsages.gripButton, out rightGripped) && rightGripped)
        {
            if ((rightHand.transform.position - this.transform.position).magnitude < minGrabDist) // check that right hand is close enough to grab
            {
                initRightHandPos = rightHand.transform.localPosition;
                PlaceHandOnObject(ref rightHand, ref rightHandOriginalParent, ref rightHandOnObject);
            }
        }
        if (leftHandOnObject == false && leftController.TryGetFeatureValue(CommonUsages.gripButton, out leftGripped) && leftGripped)
        {
            if ((leftHand.transform.position - this.transform.position).magnitude < minGrabDist) // check that right hand is close enough to grab
            {
                initLeftHandPos = leftHand.transform.localPosition;
                PlaceHandOnObject(ref leftHand, ref leftHandOriginalParent, ref leftHandOnObject);
            }
        }

    }

    private void PlaceHandOnObject(ref GameObject hand, ref Transform originalParent, ref bool handOnObject)
    {

        Debug.Log("Hand placed on object");
         
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

        //hand.transform.parent = bestSnap.transform.parent;
        hand.transform.position = bestSnap.transform.position;
        //hand.transform.scale

        // make hand disappear
        hand.SetActive(false);


        handOnObject = true;

        // update initial values for later calculation 
    }
}