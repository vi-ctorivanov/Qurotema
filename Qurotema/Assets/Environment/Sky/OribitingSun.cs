/*

Sun orbits around the map.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OribitingSun : MonoBehaviour {

	public float orbitSpeed = 1.2f;
	public Transform sunSphere;
	public bool sphereProxim = false;
	
	void Update () {
		if (sphereProxim) {
			sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, -1000f, 10f * Time.deltaTime));
		} else {
			sunSphere.localPosition = new Vector3(sunSphere.localPosition.x, sunSphere.localPosition.y, Mathf.Lerp(sunSphere.localPosition.z, -9000f, 10f * Time.deltaTime));
			transform.Rotate(0.0f, orbitSpeed * Time.deltaTime, 0.0f, Space.Self);
		}
	}
}