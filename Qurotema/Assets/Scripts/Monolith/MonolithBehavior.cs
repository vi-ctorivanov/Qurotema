using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonolithBehavior : MonoBehaviour {

	[Header("References")]
	public Renderer tear;
	public Renderer eye;
	public GameObject tearGrow;
	public GameObject eyeObject;
	public Image image;

	[Header("Dynamics")]
	private Vector3 eyeStart;
	private Vector3 eyeTarget;
	private Vector3 tearGrowStart;
	private Vector3 tearGrowTarget;
	private MaterialPropertyBlock mPB;

	[Header("States")]
	public bool active = false;

	void Start() {
		eyeStart = new Vector3(eyeObject.transform.localPosition.x + 0.001f, eyeObject.transform.localPosition.y, eyeObject.transform.localPosition.z);
		eyeTarget = eyeObject.transform.localPosition;
		eyeObject.transform.localPosition = eyeStart;

		tearGrowStart = new Vector3(tearGrow.transform.localScale.x, 0f, 0f);
		tearGrowTarget = tearGrow.transform.localScale;
		tearGrow.transform.localScale = tearGrowStart;

		image.color = new Color(1f, 1f, 1f, 0f);
	}

	public void makeActive() {
		active = true;

		Sprite t = Nox.Instance.content.monolithGraphics[Nox.Instance.monolithsRead];
		Nox.Instance.monolithActivated();
		image.sprite = t;

		StartCoroutine(makeVisible());
		Sound.Instance.addEnergy(5f);
		Sound.Instance.shootSound("sparkles");
		Nox.Instance.terrain.flashFeedback();
	}

	public IEnumerator makeVisible() {
		bool done = false;
		float alpha = 0f;

		while (!done) {
			yield return new WaitForSeconds(0.01f);
			alpha = Mathf.Lerp(alpha, 1f, 0.2f * Time.deltaTime);

			image.color = new Color(1f, 1f, 1f, alpha * 0.3f);
			eyeObject.transform.localPosition = Vector3.Lerp(eyeStart, eyeTarget, alpha);
			tearGrow.transform.localScale = Vector3.Lerp(tearGrowStart, tearGrowTarget, alpha);

			if (alpha > 0.99f) done = true;
		}
	}
}