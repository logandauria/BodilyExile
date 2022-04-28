using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

// Allows the alpha to change on a vfx graph based on beats
public class VFXSyncAlpha : AudioSyncer
{
	private Vector3 initScale;
	private float scaleTimer;
	public float beatAlpha = 255;
	public float restAlpha = 0;
	public float triggerTime = 0.2f;
	public float fadeSpeed;
	VisualEffect vfx = new VisualEffect();
	// number alpha targeted of the vfx graph... so multiple alpha syncs can be used with different beats
	public float alphaNum = 1;

	private float curAlpha = 0;

	void Start()
	{
		vfx = GetComponent<VisualEffect>();
	}

	private IEnumerator MoveToAlpha()
	{
		curAlpha = vfx.GetFloat("alpha" + alphaNum);

		float timer = 0;
		// increase the scale based on how much time has passed

		while (timer < triggerTime)
		{
			curAlpha = Mathf.Lerp(curAlpha, beatAlpha, timer / triggerTime); 
			timer += Time.deltaTime;

			vfx.SetFloat("alpha" + alphaNum, curAlpha);

			yield return null;
		}

		
		scaleTimer = 0;
		m_isBeat = false;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		if (m_isBeat) return;
		// decrease the scale based on how much time has passed
		vfx.SetFloat("alpha" + alphaNum, Mathf.Lerp(curAlpha, restAlpha, Time.deltaTime * fadeSpeed));
		curAlpha = vfx.GetFloat("alpha" + alphaNum);
	}

	public override void OnBeat()
	{
		base.OnBeat();

		StopCoroutine("MoveToAlpha");
		StartCoroutine("MoveToAlpha");
	}
}
