/*

Sun orbits around the map, faster when under horizon,
hides behind the gates sphere when they're present,
and can be made to move into the map.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingSun : MonoBehaviour {

	[Header("References")]
	public Transform sunSphere;

	[Header("Dynamics")]
	public float orbitSpeed = 0.5f;
	public float underHorizonSpeedMultiplier = 20f;
	public float proximDistanceToGround = 200f;
	public float proximSizeMultiplier = 2f;
	public float transitionAnimationSpeedMultiplier = 5f;
	private Vector3 startSize;

	[Header("States")]
	public bool proxim = false;
	public bool gates = false;

	void Start() {
		startSize = sunSphere.localScale;
	}
	
	void Update () {
		if (gates) {
			proxim = false;

			//we don't care about saving the old sun orientation, we never turn gates off once it's on
			Vector3 targetDirection = transform.position - Nox.Instance.gatesSphere.transform.position;
			Vector3 newDirection = Vector3.Lerp(transform.forward, targetDirection, transitionAnimationSpeedMultiplier * Time.deltaTime);
			transform.rotation = Quaternion.LookRotation(newDirection);
			transform.position = Vector3.Lerp(transform.position, Nox.Instance.player.transform.position, transitionAnimationSpeedMultiplier * Time.deltaTime);
		}

		if (proxim) {
			sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, proximDistanceToGround, transitionAnimationSpeedMultiplier * 0.1f * Time.deltaTime));
			sunSphere.localScale = Vector3.Lerp(sunSphere.localScale, startSize * proximSizeMultiplier, transitionAnimationSpeedMultiplier * 0.1f * Time.deltaTime);
		} else {
			sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, -9000f, transitionAnimationSpeedMultiplier * Time.deltaTime));
			if (transform.localEulerAngles.y > 20 && transform.localEulerAngles.y < 160) transform.Rotate(0.0f, orbitSpeed * underHorizonSpeedMultiplier * Time.deltaTime, 0.0f, Space.Self);
			else transform.Rotate(0.0f, orbitSpeed * Time.deltaTime, 0.0f, Space.Self);
		}

		if (!proxim) {
			sunSphere.localScale = Vector3.Lerp(sunSphere.localScale, startSize, transitionAnimationSpeedMultiplier * Time.deltaTime);
		}
	}
}