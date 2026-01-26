using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
DynamicClip is a class for playing sounds either as
	1. one-shots (either randomly or a select clip) from an array of clips
	2. loop whose volume can be modulated over time

These sounds must *always* have a low energy and high energy variant.

These are separate functionalities as a this script is attached to an object
with only two single audio sources (one for low energy, another for high).
This means that only one clip per source can be attached if the audio is to be looped.

In addition, a single-shot clip should never have its volume
modulated, as a single audio source can play multiple clips,
and a loop should never be toggled as a one-shot,
as it will be desynchronized from the rest of the track.
*/

public class DynamicClip : MonoBehaviour {

	[Header("References")]
	public AudioSource sourceLo;
	public AudioSource sourceHi;

	[Header("Clip Definition")]
	public string clipName;
	public bool isOneShot = false;

	[Header("Tracking")]
	public bool playingLo = false;
	public bool playingHi = false;

	[Header("Clip Sounds")]
	private AudioClip[] clipsLo;
	private AudioClip[] clipsHi;

	[Header("Coroutines")]
	private Coroutine mod;
	private Coroutine secondaryMod;

	public void init(bool oneShot, AudioClip[] audLo, AudioClip[] audHi, string n) {
		isOneShot = oneShot;
		clipsLo = audLo;
		clipsHi = audHi;
		clipName = n;

		sourceLo.volume = 0f;
		sourceHi.volume = 0f;

		//fill audio source
		if (!isOneShot) {
			sourceLo.loop = true;
			sourceLo.clip = clipsLo[0];
			if (clipsHi.Length > 0) {
				sourceHi.loop = true;
				sourceHi.clip = clipsHi[0];
			}
		} else {
			//enable volume if one-shots
			sourceLo.volume = 1f;
			sourceHi.volume = 1f;
		}
	}

	//play random clip from array unless specified
	public void shootClip(int index = 100, bool energy = false) {
		if (isOneShot) {
			if (index == 100) {
				int randomIndex = Random.Range(0, clipsLo.Length);
				if (!energy) sourceLo.PlayOneShot(clipsLo[randomIndex], 1f);
				else sourceHi.PlayOneShot(clipsHi[randomIndex], 1f);
			} else {
				if (!energy) sourceLo.PlayOneShot(clipsLo[index], 1f);
				else sourceHi.PlayOneShot(clipsHi[index], 1f);
			}
		}
	}

	//start loop audio
	public void kickstart() {
		if (!isOneShot) {
			sourceLo.Play();
			sourceHi.Play();
		}
	}

	public void toggleSound(bool on, float speed, bool energy = false) {
		if (!isOneShot) {
			if (mod != null) StopCoroutine(mod);
			if (secondaryMod != null) StopCoroutine(secondaryMod);

			//make sure sound of opposite energy level is off and sound of proper energy level is active
			if (energy) {
				secondaryMod = StartCoroutine(ToggleSoundRoutine(false, sourceLo, speed));
				mod = StartCoroutine(ToggleSoundRoutine(on, sourceHi, speed));
				if (on) playingHi = true;
				else playingHi = false;
				playingLo = false;
				
			} else {
				secondaryMod = StartCoroutine(ToggleSoundRoutine(false, sourceHi, speed));
				mod = StartCoroutine(ToggleSoundRoutine(on, sourceLo, speed));
				if (on) playingLo = true;
				else playingLo = false;
				playingHi = false;
			}
		}
	}

	IEnumerator ToggleSoundRoutine(bool on, AudioSource source, float speed) {
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
