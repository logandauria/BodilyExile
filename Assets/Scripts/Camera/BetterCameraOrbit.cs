using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BetterCameraOrbit : MonoBehaviour
{

    public GameObject target;
    public float speed = 1f;
    public float sideSpeed = 1f;
    public float initCamZPos = 15.5f;

    private GameObject cur;
    // makes orbit faster when camera is to a side of the object
    public float speedMul = 1f;
    public Vector3 observeRot;

    // Start is called before the first frame update
    void Start()
    {
        //cur = GetComponent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(target.transform);
        // calculate multiplier for side speed
        observeRot = transform.localEulerAngles;
        float yAng = transform.localEulerAngles.y;
        //yAng = Mathf.Abs(180 - yAng) % 90;
        yAng = Mathf.Min(Mathf.Abs(180 - yAng), Mathf.Abs(360 - yAng), Mathf.Abs(yAng));
        speedMul = yAng * sideSpeed;
        if(transform.position.z > 15.5)
        {
            transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, transform.position.y, initCamZPos), Time.deltaTime);
        }
        transform.Translate(Vector3.right * speed);
        transform.Translate(Vector3.right * speedMul);
    }
}
