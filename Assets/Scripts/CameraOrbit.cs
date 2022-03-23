using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraOrbit : MonoBehaviour
{

    public GameObject target;
    public float speed = 1f;

    private GameObject cur;

    // Start is called before the first frame update
    void Start()
    {
        //cur = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
        transform.Translate(Vector3.right * speed);
    }
}
