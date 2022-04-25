using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCanvas : MonoBehaviour
{
    public float activationTime = 3f;
    private float timer = 0;
    private bool triggerOnce = false;
    public GameObject toDisable;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > activationTime && !triggerOnce)
        {
            toDisable.SetActive(false);
            triggerOnce = true;
        }
    }
}
