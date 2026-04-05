/*

Animate terrain shader.

*/

using UnityEngine;

public class AnimateTerrain : MonoBehaviour {

	[Header("References")]
	public Material terrainMaterial;
	public Material ribbonLowMaterial;
	public Material ribbonHighMaterial;
	public Transform flyPoint;

	//dynamics
	private float mixerSpeed = 0.1f;
	private float ribbonEaseSpeed = 2f;
	private float feedbackMultiplier = 1f;
	private float[] mixers = new float[5];
	private float feedback = 0f;
	private float timeOffset = 0f;

	private void OnEnable() {
		Nox.OnFlashFeedback += addFeedback;
	}

	private void OnDisable() {
		Nox.OnFlashFeedback -= addFeedback;
	}

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
		//remap mixers range to -1 to 2, and then clamp 0-1 so that mixers are typically in either extreme
		for (int i = 0; i < mixers.Length; i++) {
			mixers[i] += mixerSpeed * Time.deltaTime;
			
			float mix = Mathf.PerlinNoise(mixers[i], 0f);
			mix = Nox.Instance.remap(mix, 0, 1, -1, 2);
			mix = Mathf.Clamp(mix, 0, 1);

			terrainMaterial.SetFloat("_Blend" + i, mix);
		}

		//manage sky feedback energy - update sky distortion offset on CPU because we can't control time in shader without jumping around
		feedback = Mathf.Lerp(feedback, 0f, ribbonEaseSpeed * Time.deltaTime);
		timeOffset += Time.deltaTime * (feedback * feedbackMultiplier);
		ribbonLowMaterial.SetFloat("_TimeOffset", timeOffset);
		ribbonHighMaterial.SetFloat("_TimeOffset", timeOffset);
	}

	public void addFeedback(float intensity) {
		feedback += intensity;
	}
}