using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class WarningTimer : MonoBehaviour
{

    private float warningTimer = 0;
    private bool TriggerOnce = false;
    public float phase0Time = 10f;
    public UnityEvent nextPhase;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        warningTimer += Time.deltaTime;
        if(warningTimer > phase0Time && !TriggerOnce)
        {
            TriggerOnce = true;
            nextPhase.Invoke();
        }
    }
}
