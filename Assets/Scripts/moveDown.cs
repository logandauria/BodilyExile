using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveDown : MonoBehaviour
{

    private bool trigger = false;
    private float moveY = 0.01f;
    private float rotY = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (trigger)
        {
            this.transform.position -= new Vector3(0, moveY, 0);
            this.transform.eulerAngles = new Vector3(0, rotY, 0);
            moveY += 0.005f;
            rotY += .1f;
        }
    }

    public void enableTrigger()
    {
        trigger = true;
    }
}
