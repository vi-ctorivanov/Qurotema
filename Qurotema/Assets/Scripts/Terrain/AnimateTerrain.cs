/*

Animate terrain shader.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateTerrain : MonoBehaviour {

	[Header("References")]
	public Material terrainMaterial;
	public Material ribbonLowMaterial;
	public Material ribbonHighMaterial;
	public Transform flyPoint;

	[Header("Dynamics")]
	public float mixerSpeed = 0.1f;
	public float ribbonEaseSpeed = 0.1f;

	[Header("States")]
	private float[] mixers = new float[5];

	[Header("Coroutines")]
	private Coroutine feedbackCoroutine;

	void Start() {
		for (int i = 0; i < mixers.Length; i++) {
			mixers[i] = Random.Range(0f, 1000f);
		}
	}

    void Update() {
		//move flypoint upwards for fading effect
		if (flyPoint.position.y < 1000) flyPoint.Translate((Vector3.up * 50) * Time.deltaTime);

		//communicate flypoint to terrain shader
		terrainMaterial.SetVector("Fly_Point", flyPoint.position);

		//update mixers
		//remap mixers from range to -1 to 2, and then clamp 0-1 so that mixers are typically in either extreme
		for (int i = 0; i < mixers.Length; i++) {
			mixers[i] += mixerSpeed * Time.deltaTime;
			
			float mix = Mathf.PerlinNoise(mixers[i], 0f);
			mix = Nox.remap(mix, 0, 1, -1, 2);
			mix = Mathf.Clamp(mix, 0, 1);

			terrainMaterial.SetFloat("_Blend" + i, mix);
		}
    }

    public void flashFeedback() {
		if (feedbackCoroutine != null) StopCoroutine(feedbackCoroutine);
		feedbackCoroutine = StartCoroutine(Feedback());
	}

	IEnumerator Feedback() {
		//set initial state instead of easing for added impact
		float UV = 10f;
		ribbonLowMaterial.SetVector("_DistortionUV", new Vector2(UV, 1f));

		bool done = false;
		while (!done) {
			yield return new WaitForSeconds(0.01f);
			UV = Mathf.Lerp(UV, 1f, ribbonEaseSpeed * Time.deltaTime);
			ribbonLowMaterial.SetVector("_DistortionUV", new Vector2(UV, 1f));
			ribbonHighMaterial.SetVector("_DistortionUV", new Vector2(UV, 1f));
			if (UV < 1.01f) done = true;
		}
	}
}