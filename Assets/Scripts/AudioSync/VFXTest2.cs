using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VFXTest2 : AudioSyncer
{

	VisualEffect vfx = new VisualEffect();

	public Vector3 beatVector;
	public Vector3 restVector;

	public float colorBlendPercent = 0f;
	public float triggerTime  = .2f;
	public bool randomRange;

	private float timer = 0;
	private float randomPercent = 0;
	

	public override void OnUpdate()
	{
		base.OnUpdate();

		if (m_isBeat)
		{
			vfx.SetFloat("blend1", Mathf.Lerp(vfx.GetFloat("blend1"), randomPercent, Time.deltaTime));



			timer += Time.deltaTime;
			if (timer > triggerTime) {
				m_isBeat = false;
				timer = 0;
			}

			return;
		}

		vfx.SetFloat("intensity", restVector.x);
		vfx.SetFloat("drag", restVector.y);
		vfx.SetFloat("frequency", restVector.z);
		vfx.SetFloat("blend1", Mathf.Lerp(vfx.GetFloat("blend1"), 0, Time.deltaTime));

	}

	public override void OnBeat()
	{
		base.OnBeat();

		if (randomRange)
		{
			vfx.SetFloat("intensity", Random.Range(restVector.x, beatVector.x));
			vfx.SetFloat("drag", Random.Range(restVector.y, beatVector.y));
			vfx.SetFloat("frequency", Random.Range(restVector.z, beatVector.z));
			randomPercent = Random.Range(0, colorBlendPercent);
		}
		else
		{
			vfx.SetFloat("intensity", beatVector.x);
			vfx.SetFloat("drag", beatVector.y);
			vfx.SetFloat("frequency", beatVector.z);
		}
	}

	public void Start()
	{
		vfx = GetComponent<VisualEffect>();
	}

}
