using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class explode : MonoBehaviour
{
    private bool grow = false;
    public VisualEffect vfx;
    public Transform mergeTransform;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Explode()
    {
        grow = true;
        BallMovement b = GetComponent<BallMovement>();
        b.enabled = false;

        vfx.SetFloat("stickdist", 0);
        vfx.SetFloat("intensity", 10);
    }

    void Update()
    {
        if (grow)
        {
            transform.position = Vector3.Lerp(transform.position, mergeTransform.position, Time.deltaTime/ 10);
            transform.eulerAngles = Vector3.Lerp(transform.eulerAngles, new Vector3(0,0,0), Time.deltaTime / 10);
            if (transform.localScale.x < 3) transform.localScale += new Vector3(.01f, .01f, .01f);
        }
    }
}
