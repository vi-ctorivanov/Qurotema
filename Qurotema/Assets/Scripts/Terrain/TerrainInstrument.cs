using UnityEngine;

public class TerrainInstrument : MonoBehaviour {

	[Header("References")]
	public Material terrainMaterial;

	[Header("Dynamics")]
	public AnimationCurve flashCurve;
	private float playHead = 100f;
	private float speed = 5f;

	private float intensityMultiplier = 20f;
	private float value = 0f;

	void Update() {
		playHead = playHead + speed * Time.deltaTime;

		terrainMaterial.SetFloat("_Play_Intensity", flashCurve.Evaluate(playHead) * intensityMultiplier);
		terrainMaterial.SetFloat("_Play_Value", value);
	}

	public void play(float v) {
		playHead = 0f;
		value = v;
	}
}