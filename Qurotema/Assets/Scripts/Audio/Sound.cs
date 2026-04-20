/*

Creates sound events for each sound, tracks bpm.

*/

using System;
using System.Collections.Generic;
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
	public FMODUnity.EventReference ambienceEvent;
	public FMOD.Studio.EventInstance ambienceState;
	
	public FMODUnity.EventReference lookEvent;
	public FMOD.Studio.EventInstance lookState;

	public FMODUnity.EventReference momentEvent;
	
	[Header("Movement Sounds")]
	public FMODUnity.EventReference flyPointEvent;
	public FMOD.Studio.EventInstance flyPointState;

	public FMODUnity.EventReference padEvent;
	public FMOD.Studio.EventInstance padState;

	public FMODUnity.EventReference percussionEvent;
	public FMOD.Studio.EventInstance percussionState;

	public FMODUnity.EventReference dropletEvent;
	public FMOD.Studio.EventInstance dropletState;

	public FMODUnity.EventReference rhythmEvent;
	public FMOD.Studio.EventInstance rhythmState;

	public FMODUnity.EventReference whipEvent;

	[Header("Instrument Sounds")]
	public FMODUnity.EventReference stringsEvent;
	public FMODUnity.EventReference ringsEvent;
	public FMODUnity.EventReference padsEvent;

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
		ambienceState = FMODUnity.RuntimeManager.CreateInstance(ambienceEvent);
		ambienceState.start();

		lookState = FMODUnity.RuntimeManager.CreateInstance(lookEvent);
		lookState.start();
		lookState.setParameterByName("Look", 0);

		flyPointState = FMODUnity.RuntimeManager.CreateInstance(flyPointEvent);
		flyPointState.start();
		flyPointState.setParameterByName("Volume", 0);

		padState = FMODUnity.RuntimeManager.CreateInstance(padEvent);
		padState.start();
		padState.setParameterByName("Volume", 0);

		percussionState = FMODUnity.RuntimeManager.CreateInstance(percussionEvent);
		percussionState.start();
		percussionState.setParameterByName("Volume", 0);

		dropletState = FMODUnity.RuntimeManager.CreateInstance(dropletEvent);
		dropletState.start();
		dropletState.setParameterByName("Volume", 0);

		rhythmState = FMODUnity.RuntimeManager.CreateInstance(rhythmEvent);
		rhythmState.start();
		rhythmState.setParameterByName("Volume", 0);

		//beat tracking
		secPerBeat = 60f / bpm;
		secPerGranularBeat = 60f / temporalResolution / bpm;
		musicStart = (float) AudioSettings.dspTime;
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
	
	public void queueShot(string name, FMODUnity.EventReference fmodEvent, params(string name, float value)[] parameters) {
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

	public void playOneShotWithParameters(FMODUnity.EventReference fmodEvent, params(string name, float value)[] parameters) {
		FMOD.Studio.EventInstance instance = FMODUnity.RuntimeManager.CreateInstance(fmodEvent);

		foreach(var (name, value) in parameters) {
			instance.setParameterByName(name, value);
		}

		instance.start();
		instance.release();
	}
}