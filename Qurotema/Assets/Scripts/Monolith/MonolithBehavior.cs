using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MonolithBehavior : MonoBehaviour {

	[Header("References")]
	public Image image;
	public Animation anim;

	[Header("States")]
	public bool active = false;

	void Start() {
		anim.Play();
		AnimationState state = anim[anim.clip.name];
		state.time = 0f;
		state.speed = 0f;
	}

	public void makeActive() {
		active = true;

		Sprite t = Nox.Instance.content.monolithGraphics[Nox.Instance.monolithsRead];
		Nox.Instance.monolithActivated();
		image.sprite = t;

		FMODUnity.RuntimeManager.PlayOneShot(Sound.Instance.momentEvent);

		//use animation system in favor of easier maintenacne
		//despite losing on a small performance benefit of material property blocks to keep material batching intact
		AnimationState state = anim[anim.clip.name];
    	state.speed = 1f;
		anim.Play();
	}
}