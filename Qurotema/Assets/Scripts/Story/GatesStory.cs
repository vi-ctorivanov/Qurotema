using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatesStory : MonoBehaviour {

	[Header("References")]
	public Material m;

	[Header("States")]
	public float alpha = 0f;
	public float glow = 0f;

	void Update() {
		m.SetFloat("_AlphaClip", alpha);
		m.SetFloat("_Glow", glow);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") Nox.Instance.endGame();
	}
}