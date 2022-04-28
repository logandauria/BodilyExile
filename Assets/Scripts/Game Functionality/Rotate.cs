using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Make a gameObject rotate constantly based on input values
public class Rotate : MonoBehaviour
{
    public float xRotation;
    public float yRotation;
    public float zRotation;
    public float speed = 1f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(xRotation, yRotation, zRotation) * Time.deltaTime * speed);
    }
}
