using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VFXTest3 : AudioSyncer
{
	VisualEffect vfx = new VisualEffect();

	// Max value for random range
	public Vector3 beatVectorUpperBound;
	// Min value for random range
	public Vector3 beatVectorLowerBound;
	// Default values to return to
	public Vector3 restVector;

	// How long the effect lasts every beat
	public float triggerTime = .2f;
	// Whether or not to include the random range. If set to false, script will use the upper bound vector
	public bool randomRange = true;
	// Whether or not the effects will get set to picked values, or values will be added to current value.
	public bool additive = false;

	// Upper bound for random percentage of color blend on beats
	public float[] colorBlendPercent;

	private float timer = 0;
	private float randomPercent = 0;

	private int select = 1;


	public override void OnUpdate()
	{
		base.OnUpdate();

		if (m_isBeat)
		{
			vfx.SetFloat("blend" + select, Mathf.Lerp(vfx.GetFloat("blend" + select), randomPercent, Time.deltaTime));



			timer += Time.deltaTime;
			if (timer > triggerTime)
			{
				m_isBeat = false;
				timer = 0;
			}

			return;
		}
		if (additive)
		{
			//vfx.SetFloat("intensity", vfx.GetFloat("intensity") - Random.Range(beatVectorLowerBound.x, beatVectorUpperBound.x));
			//vfx.SetFloat("drag", vfx.GetFloat("drag") - Random.Range(beatVectorLowerBound.y, beatVectorUpperBound.y));
			//vfx.SetFloat("frequency", vfx.GetFloat("frequency") - Random.Range(beatVectorLowerBound.z, beatVectorUpperBound.z));
		}
		else
		{
			vfx.SetFloat("intensity", restVector.x);
			vfx.SetFloat("drag", restVector.y);
			vfx.SetFloat("frequency", restVector.z);
		}
		vfx.SetFloat("blend" + select, Mathf.Lerp(vfx.GetFloat("blend" + select), 0, Time.deltaTime));
	}

	public override void OnBeat()
	{
		base.OnBeat();

		if (randomRange)
		{
			if (additive)
			{
				vfx.SetFloat("intensity", vfx.GetFloat("intensity") + Random.Range(beatVectorLowerBound.x, beatVectorUpperBound.x));
				vfx.SetFloat("drag", vfx.GetFloat("drag") + Random.Range(beatVectorLowerBound.y, beatVectorUpperBound.y));
				vfx.SetFloat("frequency", vfx.GetFloat("frequency") + Random.Range(beatVectorLowerBound.z, beatVectorUpperBound.z));
			}
			else
			{
				vfx.SetFloat("intensity", Random.Range(beatVectorLowerBound.x, beatVectorUpperBound.x));
				vfx.SetFloat("drag", Random.Range(beatVectorLowerBound.y, beatVectorUpperBound.y));
				vfx.SetFloat("frequency", Random.Range(beatVectorLowerBound.z, beatVectorUpperBound.z));
			}
			if (colorBlendPercent.Length > 0)
			{
				select = (int)(Random.Range(1, colorBlendPercent.Length + 1));
				Debug.Log(select);
				randomPercent = Random.Range(0, colorBlendPercent[select - 1]);
			}
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
