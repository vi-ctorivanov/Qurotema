using UnityEngine;

public class RingsInstrument : MonoBehaviour {

	//references
	private MeshRenderer holo;

	//dynamics
	private MaterialPropertyBlock mpb;

	//definition
	private float minimumRadius = 0.75f;
	private float maximumRadius = 1.75f;
	private float minimumAlpha = 0.2f;

	//state
	private float defaultMeshRadius;
	private float radius;
	private float targetRadius;
	private float radiusEase = 4f;

	private float resonance = 0f;
	private float resonanceMax = 1f;
	private float resonanceAccumulation = 0.5f;
	private float resonanceDecay; 

	void Start() {
		resonanceDecay = resonanceAccumulation / 3f; //must be lower than accumulation

		defaultMeshRadius = transform.localScale.x;
		radius = Random.Range(minimumRadius, maximumRadius);
		resize(radius);

		holo = GetComponent<MeshRenderer>();
		mpb = new MaterialPropertyBlock();
	}

	void Update() {
		//animate scale
		radius = Mathf.Lerp(radius, targetRadius, radiusEase * Time.deltaTime);
		transform.localScale = new Vector3(defaultMeshRadius * radius, defaultMeshRadius * radius, defaultMeshRadius * radius);

		//play resonance
		resonance -= resonanceDecay * Time.deltaTime;
		resonance = Mathf.Clamp(resonance, 0f, resonanceMax);
		//radius determines pitch,
		//Sound.Instance.playAppropriateSound

		//visual feedback
		mpb.SetFloat("_Alpha", Nox.Instance.remap(resonance, 0f, resonanceMax, minimumAlpha, 1f));
		holo.SetPropertyBlock(mpb);
	}

	public void resize(float s) {
		targetRadius = s;
		targetRadius = Mathf.Clamp(targetRadius, minimumRadius, maximumRadius);
	}

	public void resonate() {
		resonance += resonanceAccumulation * Time.deltaTime;
	}
}