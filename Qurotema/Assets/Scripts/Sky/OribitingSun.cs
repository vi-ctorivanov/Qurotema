/*

Sun orbits around the map.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OribitingSun : MonoBehaviour {

	[Header("References")]
	public Transform sunSphere;

	[Header("Dynamics")]
	public float orbitSpeed = 0.5f;
	public float underHorizonSpeedMultiplier = 20f;

	[Header("States")]
	public bool sphereProxim = false;
	
	void Update () {
		if (sphereProxim) {
			sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, -1000f, 10f * Time.deltaTime));
		} else {
			sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, -9000f, 10f * Time.deltaTime));
			//make sun rotate faster when under the horizon
			if (transform.localEulerAngles.y > 20 && transform.localEulerAngles.y < 160) transform.Rotate(0.0f, orbitSpeed * underHorizonSpeedMultiplier * Time.deltaTime, 0.0f, Space.Self);
			else transform.Rotate(0.0f, orbitSpeed * Time.deltaTime, 0.0f, Space.Self);
		}
	}
}