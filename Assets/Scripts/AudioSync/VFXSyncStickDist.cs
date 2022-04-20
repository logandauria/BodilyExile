using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VFXSyncStickDist : AudioSyncer
{

	VisualEffect vfx = new VisualEffect();


	public float restStickDist = 0f;
	public float beatStickDist = 0f;
	// How long the effect lasts every beat
	public float triggerTime = .2f;
	
	private float timer = 0;
	private float randomPercent = 0;

	public override void OnUpdate()
	{
		base.OnUpdate();

		if (m_isBeat)
		{
			//vfx.SetFloat("stickdistance", Mathf.Lerp(vfx.GetFloat("stickdistance"), beatStickDist, Time.deltaTime * 10));
			vfx.SetFloat("stickdistance", beatStickDist);
			timer += Time.deltaTime;
			if(timer > triggerTime)
            {
				m_isBeat = false;
				timer = 0;
            }
			return;
		}
		vfx.SetFloat("stickdistance", Mathf.Lerp(vfx.GetFloat("stickdistance"), restStickDist, Time.deltaTime*10));
	}

	public override void OnBeat()
	{
		base.OnBeat();

	}

	public void Start()
	{
		vfx = GetComponent<VisualEffect>();
	}
}
