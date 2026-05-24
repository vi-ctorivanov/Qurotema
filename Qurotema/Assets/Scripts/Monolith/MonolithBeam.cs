/*

Manages monolith beam drawing.

*/

using UnityEngine;

public class MonolithBeam : MonoBehaviour {

	[Header("References")]
	public LineRenderer lr;

	[Header("Definition")]
	public AnimationCurve noiseInfluenceCurve;
	private Vector3[] basePositions = new Vector3[512];
	private Vector3[] modifiedPositions = new Vector3[512];
	private float noiseOffsetSpeed = 0.002f;
	private float noiseRange = 1.5f;

	[Header("States")]
	public float alpha = 0f;

	[Header("Internals")]
	private Vector3 noiseOffset = Vector3.zero;
	private MaterialPropertyBlock mpb;

	void Start() {
		mpb = new MaterialPropertyBlock();
		
		lr.positionCount = basePositions.Length;

		for (int i = 0; i < basePositions.Length; i++) {
			basePositions[i] = new Vector3(0, 0, i / 15f);
		}

		//randomize noise parameters between instances
		noiseOffset = new Vector3(Random.Range(0f, 100f), Random.Range(0f, 100f), Random.Range(0f, 100f));
		noiseOffsetSpeed *= Random.Range(0.9f, 1.1f);
	}

	void Update() {
		//animate alpha
		mpb.SetFloat("_Alpha", 1f);
		lr.SetPropertyBlock(mpb);

		//animate positions - 3D noise based on point position, slightly offset over time
		noiseOffset -= new Vector3(noiseOffsetSpeed, noiseOffsetSpeed, noiseOffsetSpeed);

		for (int i = 0; i < modifiedPositions.Length; i++) {
			Vector3 noise = new Vector3(
				Nox.Instance.remap(Mathf.PerlinNoise(basePositions[i].z + noiseOffset.x, 0), 0f, 1f, -noiseRange, noiseRange),
				Nox.Instance.remap(Mathf.PerlinNoise(basePositions[i].z + noiseOffset.y, 0), 0f, 1f, -noiseRange, noiseRange),
				Nox.Instance.remap(Mathf.PerlinNoise(basePositions[i].z + noiseOffset.z, 0), 0f, 1f, -noiseRange, noiseRange)
			);

			//increase noise influence for later points, i.e., points higher up in the sky
			float influence = noiseInfluenceCurve.Evaluate((float)i / modifiedPositions.Length);

			modifiedPositions[i].x = basePositions[i].x + Mathf.Lerp(0f, noise.x, influence);
			modifiedPositions[i].y = basePositions[i].y + Mathf.Lerp(0f, noise.y, influence);
			modifiedPositions[i].z = basePositions[i].z;
		}

		lr.SetPositions(modifiedPositions);
	}
}