using System.Collections;
using UnityEngine;

public class StringsInstrument : MonoBehaviour {

	[Header("References")]
	public Vector3 start;
	public Vector3 end;
	private Material mat;

	//states
	private float frequency;
	private float distance;
	private float offset = 0f;
	private float offsetSpeed = 10f;
	private bool ready = true;
	private float waitTime = 1f;
	private float ringTick = 0.01f;
	private float sustainDecay = 0.0004f;
	private float maxAmplitude = 0.15f;

	//coroutines
	private Coroutine ringRoutine;

	void Start () {
		mat = GetComponent<MeshRenderer>().material;
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

			if(frequency < 300f) Sound.Instance.queueShot("string 0", Sound.Instance.stringsEvent, ("KeyNote", 0));
			else if (frequency < 400f) Sound.Instance.queueShot("string 1", Sound.Instance.stringsEvent, ("KeyNote", 1));
			else if (frequency < 500f) Sound.Instance.queueShot("string 2", Sound.Instance.stringsEvent, ("KeyNote", 2));
			else if (frequency < 600f) Sound.Instance.queueShot("string 3", Sound.Instance.stringsEvent, ("KeyNote", 3));
			else if (frequency < 700f) Sound.Instance.queueShot("string 4", Sound.Instance.stringsEvent, ("KeyNote", 4));
			else if (frequency < 800f) Sound.Instance.queueShot("string 5", Sound.Instance.stringsEvent, ("KeyNote", 5));
			else if (frequency < 900f) Sound.Instance.queueShot("string 6", Sound.Instance.stringsEvent, ("KeyNote", 6));
			else Sound.Instance.queueShot("string 7", Sound.Instance.stringsEvent, ("KeyNote", 7));

			Nox.Instance.stringPlayed();
		}
	}

	IEnumerator Refresh() {
		yield return new WaitForSeconds(waitTime);
		ready = true;
	}

	IEnumerator Ring() {
		float a = maxAmplitude;

		while (a > 0f) {
			yield return new WaitForSeconds(ringTick);

			offset += offsetSpeed;
			mat.SetFloat("_Offset", offset);
			mat.SetFloat("_Amplitude", a);
			a -= sustainDecay;

		}
	}
}