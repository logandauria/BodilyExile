using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeGrow : MonoBehaviour
{
    public float size = 1;
    public float timeDamper = 6;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.localScale.x < size)
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(size, size, size), Time.deltaTime / timeDamper);
        }
    }
}
