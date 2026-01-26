using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmbientClip : MonoBehaviour {

	public string clipName;
	public AudioClip clip;
	public AudioSource source;
	private Coroutine mod;

	void Awake() {
		source = GetComponent<AudioSource>();
	}

	public void init(AudioClip audio, string n) {
		clip = audio;
		clipName = n;

		source.clip = clip;
		source.volume = 0f;
	}

	public void kickstart() {
		source.Play();
	}

	public void toggleSound(bool on, float speed) {
		if (mod != null) StopCoroutine(mod);
		mod = StartCoroutine(ToggleSoundRoutine(on, speed));
	}

   IEnumerator ToggleSoundRoutine(bool on, float speed) {
	if (on) {
		while (source.volume < 1f) {
			yield return new WaitForSeconds(0.01f);
			source.volume += speed * Time.deltaTime;
		}
		source.volume = 1f;
	  	} else {
			while (source.volume > 0f) {
				yield return new WaitForSeconds(0.01f);
				source.volume -= speed * Time.deltaTime;
			}
		 source.volume = 0f;
	 	}
  	}
}
