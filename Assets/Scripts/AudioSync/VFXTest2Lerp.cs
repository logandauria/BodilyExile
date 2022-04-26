using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VFXTest2Lerp : AudioSyncer
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
	public float triggerTime = .2f;

	private float timer = 0f;
	private float randomPercent = 0f;

	private Vector3 randVector = new Vector3(0.1f,0.1f,0.1f);

	private IEnumerator MoveToVector()
	{
		float _timer = 0f;

		while (_timer < triggerTime)
		{
			vfx.SetFloat("intensity", Mathf.Lerp(vfx.GetFloat("intensity"), randVector.x, Time.deltaTime/triggerTime));
			vfx.SetFloat("drag", Mathf.Lerp(vfx.GetFloat("drag"), randVector.y, Time.deltaTime/triggerTime));
			vfx.SetFloat("frequency", Mathf.Lerp(vfx.GetFloat("frequency"), randVector.z, Time.deltaTime/triggerTime));

			yield return null;
		}

		m_isBeat = false;
	}


	public override void OnUpdate()
	{
		base.OnUpdate();

		//if (!m_isBeat)
		//{
		//	vfx.SetFloat("intensity", Mathf.Lerp(vfx.GetFloat("intensity"), restVector.x, Time.deltaTime ));
		//	vfx.SetFloat("drag", Mathf.Lerp(vfx.GetFloat("drag"), restVector.y, Time.deltaTime));
		//	vfx.SetFloat("frequency", Mathf.Lerp(vfx.GetFloat("frequency"), restVector.z, Time.deltaTime));
		//}

	}

	public override void OnBeat()
	{
		base.OnBeat();
		
		randVector = new Vector3(Random.Range(beatVectorLowerBound.x, beatVectorUpperBound.x), Random.Range(beatVectorLowerBound.y, beatVectorUpperBound.y), Random.Range(beatVectorLowerBound.z, beatVectorUpperBound.z));
		StartCoroutine("MoveToVector");
		
	}

	public void Start()
	{
		vfx = GetComponent<VisualEffect>();
	}

}
