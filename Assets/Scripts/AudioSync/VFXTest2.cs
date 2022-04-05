using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VFXTest2 : AudioSyncer
{

	VisualEffect vfx = new VisualEffect();

	// Max value for random range
	public Vector3 beatVectorUpperBound;
	// Min value for random range
	public Vector3 beatVectorLowerBound;
	// Default values to return to
	public Vector3 restVector;

	// Upper bound for random percentage of color blend on beats
	public float colorBlendPercent = 0f;
	// How long the effect lasts every beat
	public float triggerTime  = .2f;
	// Whether or not to include the random range. If set to false, script will use the upper bound vector
	public bool randomRange = true;

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
			vfx.SetFloat("intensity", Random.Range(beatVectorLowerBound.x, beatVectorUpperBound.x));
			vfx.SetFloat("drag", Random.Range(beatVectorLowerBound.y, beatVectorUpperBound.y));
			vfx.SetFloat("frequency", Random.Range(beatVectorLowerBound.z, beatVectorUpperBound.z));
			randomPercent = Random.Range(0, colorBlendPercent);
		}
		else
		{
			vfx.SetFloat("intensity", beatVectorUpperBound.x);
			vfx.SetFloat("drag", beatVectorUpperBound.y);
			vfx.SetFloat("frequency", beatVectorUpperBound.z);
		}
	}

	public void Start()
	{
		vfx = GetComponent<VisualEffect>();
	}

}
