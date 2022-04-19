using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Head : MonoBehaviour
{

    [SerializeField] private Transform rootObject, followObject;
    [SerializeField] private Vector3 positionOffset, rotationOffset, headBodyOffset, headBodyRotOffset;

    // Update is called once per frame
    void LateUpdate()
    {
        rootObject.position = followObject.position + positionOffset;
        //rootObject.forward = Vector3.ProjectOnPlane(followObject.up, Vector3.up).normalized;
        rootObject.eulerAngles = new Vector3(transform.rotation.x, followObject.rotation.y + rotationOffset.y, transform.rotation.z);
        //transform.position = followObject.transform.position + positionOffset;
        //transform.rotation = followObject.rotation * Quaternion.Euler(rotationOffset) * Quaternion.Euler(headBodyRotOffset);
    }
}
