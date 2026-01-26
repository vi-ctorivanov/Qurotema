using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GatesStory : MonoBehaviour {

	public GameObject Quro;
	public GameObject Tema;
	public GameObject sphere;
	public Story s;

	void Start() {
		Quro.SetActive(false);
		Tema.SetActive(false);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") s.endGame();
	}

	public void activateEnd() {
		Quro.SetActive(true);
		Tema.SetActive(true);
		Color c = new Color(1f, 1f, 1f, 1f);
		GetComponent<Renderer>().material.SetColor("_EmissiveColor", c * 60);
		sphere.GetComponent<Renderer>().material.SetColor("_EmissiveColor", c * 60);
	}
}