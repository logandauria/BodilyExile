using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hand : MonoBehaviour
{

    [SerializeField] private GameObject followObject;
    [SerializeField] private float followSpeed = 30f;
    [SerializeField] private float rotateSpeed = 100f;
    [SerializeField] private Vector3 positionOffset;
    [SerializeField] private Vector3 rotationOffset;
    private Transform _followTarget;
    private Rigidbody _body;

    private string animatorGripParam = "Grip";
    private string animatorTriggerParam = "Trigger";

    // Start is called before the first frame update
    void Start()
    {
        _followTarget = followObject.transform;
        _body = GetComponent<Rigidbody>();
        _body.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _body.interpolation = RigidbodyInterpolation.Interpolate;
        _body.mass = 20f;

        _body.position = _followTarget.position;
        _body.rotation = _followTarget.rotation;
    }

    // Update is called once per frame
    void Update()
    {
        //AnimateHand();

        PhysicsMove();
    }

    private void PhysicsMove()
    {
        var positionWithOffset = _followTarget.TransformPoint(positionOffset);
        var distance = Vector3.Distance(positionWithOffset, transform.position);
        _body.velocity = (positionWithOffset - transform.position).normalized * (followSpeed * distance);

        var rotationWithOffset = _followTarget.rotation * Quaternion.Euler(rotationOffset);
        var q = rotationWithOffset * Quaternion.Inverse(_body.rotation);
        q.ToAngleAxis(out float angle, out Vector3 axis);
        _body.angularVelocity = axis * (angle * Mathf.Deg2Rad * rotateSpeed);
    }

    //internal void SetGrip(float v)
    //{
    //    gripTarget = v;
    //}

    //internal void SetTrigger(float v)
    //{
    //    triggerTarget = v;
    //}

    //void AnimateHand()
    //{
    //    if (gripCurrent!=gripTarget)
    //    {
    //        gripCurrent = Mathf.MoveTowards(gripCurrent, gripTarget, speed * Time.unscaledDeltaTime);
    //        animator.SetFloat(animatorGripParam, gripCurrent);
    //    }
    //    if (triggerCurrent != triggerTarget)
    //    {
    //        triggerCurrent = Mathf.MoveTowards(triggerCurrent, triggerTarget, speed * Time.unscaledDeltaTime);
    //        animator.SetFloat(animatorTriggerParam, triggerCurrent);
    //    }
    //}
}
