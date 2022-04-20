using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TimePhase : MonoBehaviour
{
    public float timer = 0;

    private bool triggerOnce = false;

    public float timeLimit = 10;
    public UnityEvent nextPhase;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > timeLimit && !triggerOnce)
        {
            triggerOnce = true;
            nextPhase.Invoke();
        }
    }
}
