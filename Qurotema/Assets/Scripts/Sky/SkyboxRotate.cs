using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class SkyboxRotate : MonoBehaviour {

	[Header("References")]
	private Volume pp;
	//these need to be public for some reason
	public HDRISky sky;
	public CloudLayer cloud;

	[Header("Dynamics")]
	public float rotateSpeed = 0.01f;
	public float rotateCloudSpeed = 0.01f;

	[Header("States")]
	private float rotate = 0;
	private float rotateCloud;

	void Start() {
		pp = gameObject.GetComponent<Volume>();
		HDRISky temp;
		if (pp.profile.TryGet<HDRISky>(out temp)) sky = temp;
		CloudLayer tempC;
		if (pp.profile.TryGet<CloudLayer>(out tempC)) cloud = tempC;
	}

	void Update() {
		rotate += rotateSpeed;
		rotateCloud += rotateCloudSpeed;
		if (rotate > 360f) rotate = 0f;
		if (rotate < 0f) rotate = 360f;
		if (rotateCloud > 360f) rotateCloud = 0f;
		if (rotateCloud < 0f) rotateCloud = 360f;

		sky.rotation.value = rotate;
		cloud.layerA.rotation.value = rotateCloud;
	}
}
