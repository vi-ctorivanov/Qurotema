using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringCord : MonoBehaviour {

	[Header("References")]
	public Vector3 start;
	public Vector3 end;
	private Sound soundSystem;
	private Story s;
	private BoxCollider col;
	private Material mat;

	[Header("States")]
	public float frequency;
	private float distance;
	private float offset = 0f;
	private float offsetSpeed = 10f;
	private bool ready = true;
	private float waitTime = 1f;
	private float ringTick = 0.01f;
	private float sustainDecay = 0.0006f;

	[Header("Coroutines")]
	private Coroutine ringRoutine;

	void Start () {
		soundSystem = GameObject.Find("Nox").GetComponent<Sound>();
		col = GetComponent<BoxCollider>();
		mat = GetComponent<MeshRenderer>().material;
		s = GameObject.Find("Nox").GetComponent<Story>();
	}

	public void init (Vector3 s, Vector3 e) {
		start = s;
		end = e;
		distance = Vector3.Distance(s, e);
		frequency = distance * 30f;
	}
	
	public void playSound() {
		if (ready) {
			ready = false;

			if (ringRoutine != null) StopCoroutine(ringRoutine);
			ringRoutine = StartCoroutine(Ring());
			StartCoroutine(Refresh());
			soundSystem.addEnergy(0.2f);

			if(frequency < 300f) soundSystem.shootSound("strings", 1);
			else if (frequency < 400f) soundSystem.shootSound("strings", 2);
			else if (frequency < 500f) soundSystem.shootSound("strings", 3);
			else if (frequency < 600f) soundSystem.shootSound("strings", 4);
			else if (frequency < 700f) soundSystem.shootSound("strings", 5);
			else if (frequency < 800f) soundSystem.shootSound("strings", 6);
			else if (frequency < 900f) soundSystem.shootSound("strings", 7);
			else soundSystem.shootSound("strings", 8);

			s.stringPlayed();
		}
	}

	IEnumerator Refresh() {
		yield return new WaitForSeconds(waitTime);
		ready = true;
	}

	IEnumerator Ring() {
		float a = 0.1f;

		while (a > 0f) {
			yield return new WaitForSeconds(ringTick);

			offset += offsetSpeed;
			mat.SetFloat("_Offset", offset);
			mat.SetFloat("_Amplitude", a);
			a -= sustainDecay;

		}
	}
}