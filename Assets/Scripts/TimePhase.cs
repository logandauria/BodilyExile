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
    public UnityEvent markov1;
    public UnityEvent markov2;
    public UnityEvent markov3;
    public UnityEvent markov4;

    // Start is called before the first frame update
    void Start()
    {
        markov1.Invoke();
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if(timer > midTimeLimit && !triggerMidOnce)
        {
            markov2.Invoke();
            triggerMidOnce = true;
            midEvent.Invoke();
        }
        if(timer > timeLimit && !triggerOnce)
        {
            markov4.Invoke();
            triggerOnce = true;
            nextPhase.Invoke();
        }
        if(timer > explodeTimeLimit && !triggerExpOnce)
        {
            markov3.Invoke();
            triggerExpOnce = true;
            explode.Invoke();
        }
    }
}
