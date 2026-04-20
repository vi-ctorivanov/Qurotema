using UnityEngine;

public class GatesStory : MonoBehaviour {

	[Header("References")]
	public Material m;

	[Header("States")]
	public float alphaClip = 0f;
	public float glow = 0f;
	public float glowIntensity = 0f;

	void Update() {
		//animating through this script to animate the master material
		//which is shared with the gates' sphere
		m.SetFloat("_AlphaClip", alphaClip);
		m.SetFloat("_Glow", glow);
		m.SetFloat("_Glow_Intensity", glowIntensity);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") Nox.Instance.endGame();
	}
}