using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioSyncScale : AudioSyncer {

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


		if (WorkWithChangingScale)
		{
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
		if (WorkWithChangingScale)
		{
			scaleTimer += Time.deltaTime;
			if(scaleTimer < timeToBeat) { 
				Vector3 _curr = transform.localScale -= beatScale / (timeToBeat / Time.deltaTime);

				transform.localScale = _curr;

			}
		}
		transform.localScale = Vector3.Lerp(transform.localScale, restScale, restSmoothTime * Time.deltaTime);
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
