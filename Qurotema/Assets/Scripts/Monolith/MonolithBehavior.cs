using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonolithBehavior : MonoBehaviour {

	[Header("References")]
	public Renderer tear;
	public Renderer eye;
	public CanvasGroup canvas;
	public Image image;

	[Header("States")]
	public bool active = false;

	public void makeActive() {
		active = true;

		Sprite t = Nox.Instance.content.monolithGraphics[Nox.Instance.monolithsRead];
		Nox.Instance.monolithActivated();
		image.material.SetTexture("_MainTex", t.texture);
		image.sprite = t;

		StartCoroutine(makeVisible());

		Sound.Instance.addEnergy(5f);
		Sound.Instance.shootSound("sparkles");

		Nox.Instance.terrain.flashFeedback();
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