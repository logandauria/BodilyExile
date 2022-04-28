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
    // filter for triggering audio distortion when ball is far away
    public AudioReverbFilter audioFilter;
    // hand objects
    public GameObject rightHand;
    public GameObject leftHand;
    // object transform array of grabbable positions
    public Transform[] snapPositions;
    public Transform directonalObject;

    // Track hand state
    private bool rightHandOnObject = false;
    private bool leftHandOnObject = false;
    private Transform leftHandOriginalParent;
    private Transform rightHandOriginalParent;

    // Track init ball state
    private Vector3 initPos;
    private Vector3 initRot;
    private Rigidbody rb;
    // New rotation to be applied based on angle of hands changing when ball is thrown
    private Vector3 newRot = new Vector3(0, 0, 0);
    // Track multiple hand velocities at different times for better throwing physics
    private Vector3 lhandPreviousPos1;
    private Vector3 lhandPreviousPos2;
    private Vector3 rhandPreviousPos1;
    private Vector3 rhandPreviousPos2;
    // For combining velocities of both hands for two handed throw
    private Vector3 combinedVelocity = new Vector3(0, 0, 0);
    // For velocity calculations
    private Vector3 rhandVelocity = new Vector3(0, 0, 0);
    private Vector3 lhandVelocity = new Vector3(0, 0, 0);
    public Vector3 rotVelocity = new Vector3(0, 0, 0);
    public Vector3 previousRotVelocity = new Vector3(0, 0, 0);

    // Timers
    public float velocityTimer = 0;
    private bool timeVel = false;
    public float timer = 0;

    // VARIABLES TO INFLUENCE ORB VELOCITIES
    private float comebackSpeed = 0.5f;
    private float throwSpeedDamper = 20f;
    private float rotDamper = 35f;
    // Floor y position for ball to bounce off of
    private float floorYPos = 10f;

    // Track if the ball has been let go of so OnLetGo() only triggers once
    private bool needToLetGo = false;

    // Start is called before the first frame update
    void Start()
    {
        // initialize needed variables
        initPos = this.transform.position;
        initRot = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, transform.eulerAngles.z);
        initScale = this.transform.localScale;
        rb = GetComponent<Rigidbody>();
        lhandPreviousPos1 = leftHand.transform.position;
        lhandPreviousPos2 = leftHand.transform.position;
        rhandPreviousPos1 = rightHand.transform.position;
        rhandPreviousPos2 = rightHand.transform.position;
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

    /// <summary>
    /// When the ball is let go of by both hands, this will be called to update initPos and make the hands appear
    /// </summary>
    private void OnLetGo()
    {
        // update initpos to position ball was thrown from
        initPos = transform.position;
        rightHand.SetActive(true);
        leftHand.SetActive(true);
        needToLetGo = false;
    }

    // Update is called once per frame
    /// <summary>
    /// Call all necessary methods each frame to control the interaction
    /// </summary>
    void Update()
    {
        timer += Time.deltaTime;

        // Fetch controller devices if either are invalid
        if (!leftController.isValid || !rightController.isValid)
        {
            GetDevice();
        }

        OnTriggerStay();

        ReleaseHandsFromObject();

        ControlBall();

        //this.transform.eulerAngles += newRot;
        
        if(timeVel) velocityTimer += Time.deltaTime;
        
        // if ball isn't being grabbed, update and apply necessary values
        if (rightHandOnObject == false && leftHandOnObject == false)
        {
            // Apply velocity to ball
            this.transform.position += combinedVelocity;
            // Lerp the ball's rotation to zero
            newRot = Vector3.Lerp(newRot, new Vector3(0, 0, 0), Time.deltaTime / 5);
            // Lerp the ball's velocity to zero
            combinedVelocity = Vector3.Lerp(combinedVelocity, new Vector3(0, 0, 0), Time.deltaTime);
            // Lerp the ball's rotation velocity to zero
            rotVelocity = Vector3.Lerp(rotVelocity, new Vector3(0, 0, 0), Time.deltaTime / 2);

            // prevent stutter from rotational force if its magnitude isn't large enough
            if(rotVelocity.magnitude > 0.001) this.transform.eulerAngles += rotVelocity;

            // slowly revert the ball to its initial position, scale and rotation
            transform.localScale = Vector3.Lerp(transform.localScale, initScale, Time.deltaTime);
            transform.position = Vector3.Lerp(transform.position, initPos, Time.deltaTime/comebackSpeed);
            timeVel = false;
        }
        else
        {
            //combinedVelocity = (transform.position - previousPos) / (Time.deltaTime * throwSpeed);
        }
        // Update hand position for velocity calculation when ball is thrown
        lhandPreviousPos1 = leftHand.transform.position;
        rhandPreviousPos1 = rightHand.transform.position;
        // Calculate distance from left hand to ball
        float dist = Vector3.Distance(transform.position, leftHand.transform.position);
        // Apply audio distortion based on above distance
        audioFilter.decayTime = dist;
        audioFilter.decayHFRatio = dist/10;
        audioFilter.dryLevel = -dist*500;
        // Bounce the ball by inverting its y velocity if it hits the floor
        if(transform.position.y < floorYPos)
        {
            combinedVelocity.y = combinedVelocity.y / -2;
        }
        // Track more hand positions for smoother throw velocity calculation
        if (timer % 0.02f < 0.01f)
        {
            lhandPreviousPos1 = leftHand.transform.position;
            rhandPreviousPos1 = rightHand.transform.position;
            rhandPreviousPos2 = rightHand.transform.position;
            lhandPreviousPos2 = rightHand.transform.position;
            previousRotVelocity = this.transform.localEulerAngles;
        } else if(timer % 0.1f < 0.05f)
        {
            //rhandPreviousPos2 = rightHand.transform.position;
            //lhandPreviousPos2 = rightHand.transform.position;
        }
        //transform.eulerAngles = new Vector3(initRot.x, initRot.y, transform.eulerAngles.z);
    }



    /// <summary>
    /// Changes the scale of the object based on the x distance value between the two hands
    /// Changes the rotation speed of the object based on the z distance value between the two hands
    /// </summary>
    private void ControlBall()
    {
        // Check that both hands are grabbing a snap position on the object
        if (rightHandOnObject == true && leftHandOnObject == true)
        {
            // Update ball position to be in between the hands
            this.transform.position = (rightHand.transform.position + leftHand.transform.position) / 2;

            // current hand distances - initial hand distances for x and z
            float xDiff = Mathf.Abs(initRightHandPos.x - initLeftHandPos.x) - Mathf.Abs(rightHand.transform.position.x - leftHand.transform.position.x);
            float zDiff = Mathf.Abs(initRightHandPos.z - initLeftHandPos.z) - Mathf.Abs(rightHand.transform.position.z - leftHand.transform.position.z);

            // apply x difference of hands to the scale
            this.transform.localScale = initScale - new Vector3(xDiff / 2, xDiff / 2, xDiff / 2);

            // apply z difference of hands to the rotation
            newRot = new Vector3(0, .5f - zDiff * 0.5f, 0); // 3 before

            // Send haptic feedback to both controllers while grabbing the ball
            leftController.SendHapticImpulse(1, 0.1f);
            rightController.SendHapticImpulse(1, 0.1f);
        }
        // If only left hand is grabbing the ball
        else if (rightHandOnObject == false && leftHandOnObject == true)
        {
            // Update ball pos to pos of left hand
            this.transform.eulerAngles = leftHand.transform.eulerAngles;
            this.transform.position = Vector3.Lerp(this.transform.position, leftHand.transform.position, Time.deltaTime*10);

            needToLetGo = true;
            // Send haptic feedback to left controller
            leftController.SendHapticImpulse(1, 0.1f);
        }
        // If only right hand is grabbing the ball
        else if (rightHandOnObject == true && leftHandOnObject == false)
        {
            // Update ball pos to pos of right hand
            this.transform.position = Vector3.Lerp(this.transform.position, rightHand.transform.position, Time.deltaTime*10);
            this.transform.eulerAngles = rightHand.transform.eulerAngles;

            needToLetGo = true;
            // Send haptic feedback to right controller
            leftController.SendHapticImpulse(1, 0.1f);
        }
        else
        {
            // Call OnLetGo once if ball is let go of
            if(needToLetGo) OnLetGo();
        }
    }

    /// <summary>
    /// Catches when hand(s) are released from the object
    /// </summary>
    private void ReleaseHandsFromObject()
    {
        // Detect if right hand lets go
        if (rightHandOnObject && rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightGripped) && !rightGripped)
        {
            // fix right hand
            rightHand.transform.parent = rightHandOriginalParent;
            rightHand.transform.position = rightHandOriginalParent.position;
            rightHand.transform.rotation = rightHandOriginalParent.rotation;
            rightHand.transform.localScale = rightHandOriginalParent.localScale;
            rightHandOnObject = false;
            
            // if other hand is still grabbing the ball, start velocity timer to determine whether or not its a two handed throw
            if (leftGripped) {
                velocityTimer = 0;
                timeVel = true;
            // whether or not to include both hands velocities in the throw
            } else if (velocityTimer < .2f)
            {
                // combine velocities of the 2 last picked hand positions of both hands
                combinedVelocity = (rhandPreviousPos1 - rightHand.transform.position) / (Time.deltaTime * throwSpeedDamper) +
                                    (lhandPreviousPos1 - leftHand.transform.position) / (Time.deltaTime * throwSpeedDamper) / 2;/* + 
                                    (rhandPreviousPos2 - rightHand.transform.position) / (Time.deltaTime * throwSpeedDamper) + 
                                    (lhandPreviousPos2 - leftHand.transform.position) / (Time.deltaTime * throwSpeedDamper) / 4 ;*/
            }
            else
            {
                combinedVelocity = (rhandPreviousPos1 - rightHand.transform.position) / throwSpeedDamper;// + (rhandPreviousPos2 - rightHand.transform.position) / (Time.deltaTime * throwSpeedDamper) / 2;
            }
            rotVelocity = (previousRotVelocity - this.transform.eulerAngles) / rotDamper;
        }
        // Detect if left hand lets go
        if (leftHandOnObject && leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftGripped) && !leftGripped)
        {
            // fix left hand
            leftHand.transform.parent = leftHandOriginalParent;
            leftHand.transform.position = leftHandOriginalParent.position;
            leftHand.transform.rotation = leftHandOriginalParent.rotation;
            leftHand.transform.localScale = leftHandOriginalParent.localScale;
            leftHandOnObject = false;

            // if other hand is still grabbing the ball, start velocity timer to determine whether or not its a two handed throw
            if (rightGripped)
            {
                velocityTimer = 0;
                timeVel = true;
            }
            // whether or not to include both hands velocities in the throw
            else if (velocityTimer < .2f)
            {
                // combine velocities of the 2 last picked hand positions

                combinedVelocity = (rhandPreviousPos1 - rightHand.transform.position) / (Time.deltaTime * throwSpeedDamper) +
                                    (lhandPreviousPos1 - leftHand.transform.position) / (Time.deltaTime * throwSpeedDamper) / 2;/* + 
                                    (rhandPreviousPos2 - rightHand.transform.position) / (Time.deltaTime * throwSpeedDamper) + 
                                    (lhandPreviousPos2 - leftHand.transform.position) / (Time.deltaTime * throwSpeedDamper) / 4 ;*/
            }
            else
            {
                combinedVelocity = (lhandPreviousPos1 - leftHand.transform.position);// / (Time.deltaTime * throwSpeedDamper);// + (lhandPreviousPos2 - leftHand.transform.position) / (Time.deltaTime * throwSpeedDamper) / 2;
            }
            rotVelocity = (this.transform.eulerAngles - previousRotVelocity) / rotDamper;
        }
        if (!leftHandOnObject && !leftHandOnObject)
        {
            transform.parent = null;
        }
    }

    /// <summary>
    /// Detect controller input to grab the ball
    /// </summary>
    private void OnTriggerStay()
    {
        // How far away the hand is allowed to be to trigger a grab.
        float minGrabDist = 0.8f;

        if (rightHandOnObject == false && rightController.TryGetFeatureValue(CommonUsages.triggerButton, out rightGripped) && rightGripped)
        {
            if ((rightHand.transform.position - this.transform.position).magnitude < minGrabDist) // check that right hand is close enough to grab
            {
                initRightHandPos = rightHand.transform.localPosition;
                PlaceHandOnObject(ref rightHand, ref rightHandOriginalParent, ref rightHandOnObject);
            }
        }
        if (leftHandOnObject == false && leftController.TryGetFeatureValue(CommonUsages.triggerButton, out leftGripped) && leftGripped)
        {
            if ((leftHand.transform.position - this.transform.position).magnitude < minGrabDist) // check that right hand is close enough to grab
            {
                initLeftHandPos = leftHand.transform.localPosition;
                PlaceHandOnObject(ref leftHand, ref leftHandOriginalParent, ref leftHandOnObject);
            }
        }

    }

    /// <summary>
    /// Place the hand on the object by finding the nearest snap position. Used for both hands by having reference parameters
    /// </summary>
    /// <param name="hand">Reference to the hand object that was used to grab</param>
    /// <param name="originalParent">Reference to the original parent of the hand</param>
    /// <param name="handOnObject">Reference to the bool that tracks if the hand is on the object</param>
    private void PlaceHandOnObject(ref GameObject hand, ref Transform originalParent, ref bool handOnObject)
    {

        Debug.Log("Hand placed on object");
         
        // Find best snap position object
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