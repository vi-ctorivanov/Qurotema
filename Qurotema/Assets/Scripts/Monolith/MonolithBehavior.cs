using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonolithBehavior : MonoBehaviour {

	[Header("References")]
	private Story s;
	private Nox n;
	private Sound soundSystem;
	public Renderer tear;
	public Renderer eye;
	public CanvasGroup canvas;
	public Image image;

	[Header("States")]
	public bool active = false;

	void Start() {
		s = GameObject.Find("Nox").GetComponent<Story>();
		n = GameObject.Find("Nox").GetComponent<Nox>();
		soundSystem = GameObject.Find("Nox").GetComponent<Sound>();
	}

	public void makeActive() {
		active = true;

		Sprite t = s.monolithTexts[s.monolithsRead];
		s.monolithActivated();
		image.material.SetTexture("_MainTex", t.texture);
		image.sprite = t;

		StartCoroutine(makeVisible());

		soundSystem.addEnergy(5f);
		soundSystem.shootSound("sparkles");

		n.terrain.flashFeedback();
	}

	IEnumerator makeVisible() {
		bool done = false;
		float alpha = 0f;
		while (!done) {
			yield return new WaitForSeconds(0.05f);

			alpha = Mathf.Lerp(alpha, 1f, 2f * Time.deltaTime);

			canvas.alpha = alpha;

			Color c = new Color(alpha, alpha, alpha, alpha);
			Color e = new Color(alpha * 25f, alpha * 25f, alpha * 500f);

			tear.material.SetColor("_BaseColor", c);
			tear.material.SetColor("_EmissiveColor", e);

			eye.material.SetColor("_BaseColor", c);
			eye.material.SetColor("_EmissiveColor", e);

			if (alpha > 0.99f) done = true;
		}
	}
}