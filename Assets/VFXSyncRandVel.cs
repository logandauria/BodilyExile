using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


public class VFXSyncRandVel : AudioSyncer
{

	VisualEffect vfx = new VisualEffect();

	// upper bound for random velocity
	public Vector3 beatVelocityUB;
	// lower bound for random velocity
	public Vector3 beatVelocityLB;
	public Vector3 restVelocity;

	// how long the effect will trigger for
	public float triggerTime;

	private float timer = 0;


	public override void OnUpdate()
	{
		base.OnUpdate();
		timer += Time.deltaTime;
		if (timer > triggerTime)
		{
			vfx.SetVector3("randvel", restVelocity);
			timer = 0;
		}
	}

	public override void OnBeat()
	{
		base.OnBeat();
		// create new random velocity vector based on the upper and lower bound vectors
		Vector3 vel = new Vector3(Random.Range(beatVelocityLB.x, beatVelocityUB.x), Random.Range(beatVelocityLB.x, beatVelocityUB.x), Random.Range(beatVelocityLB.x, beatVelocityUB.x));
		vfx.SetVector3("randvel", vel);

	}

	public void Start()
	{
		vfx = GetComponent<VisualEffect>();
	}
}
