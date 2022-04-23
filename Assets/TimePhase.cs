using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class TimePhase : MonoBehaviour
{
    public float timer = 0;

    private bool triggerOnce = false;
    private bool triggerMidOnce = false;
    private bool triggerExpOnce = false;

    public float timeLimit = 10;
    public float midTimeLimit = 60;
    public float explodeTimeLimit = 50;
    public UnityEvent midEvent;
    public UnityEvent nextPhase;
    public UnityEvent explode;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > midTimeLimit && !triggerMidOnce)
        {
            triggerMidOnce = true;
            midEvent.Invoke();
        }
        if(timer > timeLimit && !triggerOnce)
        {
            triggerOnce = true;
            nextPhase.Invoke();
        }
        if(timer > explodeTimeLimit && !triggerExpOnce)
        {
            triggerExpOnce = true;
            explode.Invoke();
        }
    }
}
