/*

Creates sound events for each sound, tracks bpm.

*/

using System;
using System.Collections.Generic;
using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class Sound : MonoBehaviour {

	//timing
	private float bpm = 120f; //time signature is 4/4
	private int temporalResolution = 2; //number of divisions of a quarter note for granular beats, should be multiples of 2

	//tracking
	private float musicStart;
	public int beat = 1;
	private float secPerBeat;
	public int granularBeat = 1;
	private float secPerGranularBeat;

	//queue
	private List<Shot> queue = new List<Shot>();

	//events
	public static event Action<int> OnBeat;

	[Header("Atmosphere Sounds")]
	public EventReference ambienceEvent;
	public EventInstance ambienceState;
	
	public EventReference lookEvent;
	public EventInstance lookState;

	public EventReference momentEvent;
	
	[Header("Movement Sounds")]
	public EventReference flyPointEvent;
	public EventInstance flyPointState;

	public EventReference padEvent;
	public EventInstance padState;

	public EventReference percussionEvent;
	public EventInstance percussionState;

	public EventReference dropletEvent;
	public EventInstance dropletState;

	public EventReference rhythmEvent;
	public EventInstance rhythmState;

	public EventReference whipEvent;

	[Header("Instrument Sounds")]
	public EventReference stringsEvent;
	public EventReference ringsEvent;
	public EventReference padsEvent;

	//create static singleton to act as a globally accessible Sound
	//if instance is null (it is at first), set it to this object so all references point to it
	private static Sound instance;
	public static Sound Instance {
		get { 
			if (instance == null) instance = GameObject.Find("Nox").GetComponent<Sound>();
			return instance;
		}
	}

	void Start() {
		//activate ambient sound events
		ambienceState = RuntimeManager.CreateInstance(ambienceEvent);
		ambienceState.start();
	
		lookState = RuntimeManager.CreateInstance(lookEvent);
		lookState.start();
		lookState.setParameterByName("Look", 0);

		flyPointState = RuntimeManager.CreateInstance(flyPointEvent);
		flyPointState.start();
		flyPointState.setParameterByName("Volume", 0);

		padState = RuntimeManager.CreateInstance(padEvent);
		padState.start();
		padState.setParameterByName("Volume", 0);

		percussionState = RuntimeManager.CreateInstance(percussionEvent);
		percussionState.start();
		percussionState.setParameterByName("Volume", 0);

		dropletState = RuntimeManager.CreateInstance(dropletEvent);
		dropletState.start();
		dropletState.setParameterByName("Volume", 0);

		rhythmState = RuntimeManager.CreateInstance(rhythmEvent);
		rhythmState.start();
		rhythmState.setParameterByName("Volume", 0);

		//beat tracking
		secPerBeat = 60f / bpm;
		secPerGranularBeat = 60f / temporalResolution / bpm;
		musicStart = (float) AudioSettings.dspTime;
	}

	//FMOD events are not tied to gameobjects' lifecycles
	void OnDestroy() {
		foreach(EventInstance i in new[]{ambienceState, lookState, flyPointState, padState, percussionState, dropletState, rhythmState}) {
			if (i.isValid()) {
            	i.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            	i.release();
        	}
		}
    }

	void Update() {
		float musicPosition = (float) (AudioSettings.dspTime - musicStart);

		//standard beats
		int computedBeat = (int) Mathf.Floor(musicPosition / secPerBeat);
		if (beat != computedBeat % 16) {
			beat = computedBeat % 16;
			OnBeat?.Invoke(beat);
		}

		//granular beats
		computedBeat = (int) Mathf.Floor(musicPosition / secPerGranularBeat);
		if (granularBeat != computedBeat % (16 * temporalResolution)) {
			granularBeat = 	computedBeat % (16 * temporalResolution);
			playQueue();
		}
	}
	
	public void queueShot(string name, EventReference fmodEvent, params(string name, float value)[] parameters) {
		//if shot is closer to the previous beat than the next, just play it to avoid undesireable delay
		float musicPosition = (float) (AudioSettings.dspTime - musicStart);
		float present = (float) musicPosition / secPerGranularBeat;
		float previousBeat = (int) Mathf.Floor(musicPosition / secPerGranularBeat);
		float nextBeat = (int) Mathf.Ceil(musicPosition / secPerGranularBeat);

		if (present - previousBeat < nextBeat - present) {
			playOneShotWithParameters(fmodEvent, parameters);
			return;
		}

		//only allow one slot for each shot of a specific name
		if (!queue.Exists(x => x.name == name)) queue.Add(new Shot(name, fmodEvent, parameters));
	}

	private void playQueue() {
		foreach (Shot shot in queue) {
			playOneShotWithParameters(shot.fmodEvent, shot.parameters);
		}
		queue = new List<Shot>();
	}

	public void playOneShotWithParameters(EventReference fmodEvent, params(string name, float value)[] parameters) {
		EventInstance instance = RuntimeManager.CreateInstance(fmodEvent);

		foreach(var (name, value) in parameters) {
			instance.setParameterByName(name, value);
		}

		instance.start();
		instance.release();
	}
}