using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncScale : AudioSyncer {


	// Allows the scale change to function on an object that has a changing scale, since rest vector and beat vector are prefined scales,
	// the script instead increases the object based on the beatScale alone, using it as a percentage increase.
	// WARNING: Setting to true causes scale to increase by the percentage representation of beatScale
	public bool WorkWithChangingScale = false;
	private Vector3 initScale;
	private float scaleTimer;


	void Start()
    {
		scaleTimer = timeToBeat;
    }

	private IEnumerator MoveToScale(Vector3 _target)
	{
		Vector3 _curr = transform.localScale;
		Vector3 _initial = _curr;
		float _timer = 0;

		// increase the scale based on how much time has passed
		if (WorkWithChangingScale)
		{
			// timer instead based off of time to beat
			while (_timer != timeToBeat)
			{
				_timer += Time.deltaTime;
				_curr = transform.localScale += beatScale / (timeToBeat / Time.deltaTime);

				transform.localScale = _curr;

				yield return null;
			}
		}
		else
		{

			while (_curr != _target)
			{
				_curr = Vector3.Lerp(_initial, _target, _timer / timeToBeat);
				_timer += Time.deltaTime;

				transform.localScale = _curr;

				yield return null;
			}

		}
		scaleTimer = 0;
		m_isBeat = false;
	}

	public override void OnUpdate()
	{
		base.OnUpdate();

		if (m_isBeat) return;
		// decrease the scale based on how much time has passed
		if (WorkWithChangingScale)
		{
			scaleTimer += Time.deltaTime;
			// timer based off of time to beat
			if (scaleTimer < timeToBeat)
			{
				Vector3 _curr = transform.localScale -= beatScale / (timeToBeat / Time.deltaTime);

				transform.localScale = _curr;

			}
		}
		else
		{
			transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
		}
	}

	public override void OnBeat()
	{
		base.OnBeat();

		StopCoroutine("MoveToScale");
		StartCoroutine("MoveToScale", beatScale);
	}

	public Vector3 beatScale;
	public Vector3 restScale;
}
