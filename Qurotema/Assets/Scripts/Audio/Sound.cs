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

	//tracking
	private float musicStart;
	// beats are 16th notes, smallest musical time unit in the game, played for 4 bars for a total of 64 beats
	private int beat = 1;
	private float secPerBeat;

	//queue
	private List<Shot> queue = new List<Shot>();

	//events
	public static event Action<int> OnSet;
	public static event Action<int> OnWhole;
	public static event Action<int> OnHalf;
	public static event Action<int> OnQuarter;
	public static event Action<int> OnEighth;
	public static event Action<int> OnSixteenth;

	private List<EventInstance> allInstances = new List<EventInstance>();

	[Header("Backend Sounds")]
	public EventReference keyEvent;
	public EventInstance keyState;

	[Header("World Sounds")]
	public EventReference ambienceCmEvent;
	public EventInstance ambienceCmState;

	public EventReference counterpointCmEvent;
	public EventInstance counterpointCmState;

	public EventReference bassCmEvent;
	public EventInstance bassCmState;
	public EventReference bassEdEvent;
	public EventInstance bassEdState;

	public EventReference padCmEvent;
	public EventInstance padCmState;
	public EventReference padEdEvent;
	public EventInstance padEdState;

	public EventReference sunDragEvent;
	public EventInstance sunDragState;

	public EventReference sunEnlargeEvent;
	public EventInstance sunEnlargeState;

	[Header("Player Sounds")]
	public EventReference lookEvent;
	public EventInstance lookState;

	public EventReference slowCmEvent;
	public EventInstance slowCmState;
	public EventReference slowEdEvent;
	public EventInstance slowEdState;

	public EventReference sprintCmEvent;
	public EventInstance sprintCmState;
	public EventReference sprintEdEvent;
	public EventInstance sprintEdState;

	public EventReference sprintStartEvent;

	[Header("UI Sounds")]
	public EventReference controlEvent;
	public EventInstance controlState;

	public EventReference flyEvent;
	public EventInstance flyState;

	public EventReference markerEvent;
	public EventInstance markerState;
	public EventReference teleportEvent;

	[Header("Instrument Sounds")]
	public EventReference stringsEvent;
	public EventReference stringsStringEvent;

	public EventReference ringsEvent;
	public EventInstance[] ringsStates;
	public EventReference ringsRescaleEvent;

	public EventReference padsEvent;
	public EventReference padsStepEvent;

	public EventReference terrainEvent;

	public EventReference monolithEvent;
	public EventInstance[] monolithStates;

	[Header("Story")]
	public EventReference gatesEvent;
	public EventInstance gatesState;

	public EventReference progressEvent;
	public EventReference dialogEvent;
	public EventReference titleCardEvent;

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
		//these fmod events run nonstop and handle their parameters like volume
		//independently using internal parameters

		//ambience
		keyState = CreateAndTrack(keyEvent);
		ambienceCmState = CreateAndTrack(ambienceCmEvent);
		counterpointCmState = CreateAndTrack(counterpointCmEvent);
		bassCmState = CreateAndTrack(bassCmEvent);
		bassEdState = CreateAndTrack(bassEdEvent);
		padCmState = CreateAndTrack(padCmEvent);
		padEdState = CreateAndTrack(padEdEvent);
		sunDragState = CreateAndTrack(sunDragEvent);
		sunEnlargeState = CreateAndTrack(sunEnlargeEvent);

		//player
		lookState = CreateAndTrack(lookEvent);
		lookState.setParameterByName("Look", 0);

		slowCmState = CreateAndTrack(slowCmEvent);
		slowEdState = CreateAndTrack(slowEdEvent);
		sprintCmState = CreateAndTrack(sprintCmEvent);
		sprintEdState = CreateAndTrack(sprintEdEvent);

		//ui
		controlState = CreateAndTrack(controlEvent);
		flyState = CreateAndTrack(flyEvent);
		markerState = CreateAndTrack(markerEvent);

		//instruments
		ringsStates = new EventInstance[5];
		for (int i = 0; i < ringsStates.Length; i++)
			ringsStates[i] = CreateAndTrack(ringsEvent);

		monolithStates = new EventInstance[8];
		for (int i = 0; i < monolithStates.Length; i++)
			monolithStates[i] = CreateAndTrack(monolithEvent);

		//story
		gatesState = CreateAndTrack(gatesEvent, autoStart: false);

		//beat tracking
		secPerBeat = 60f / bpm / 4; //16th notes
		musicStart = (float) AudioSettings.dspTime;
	}

	//FMOD events are not tied to gameobjects' lifecycles
	void OnDestroy() {
		foreach(EventInstance i in allInstances) {
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
		if (beat != computedBeat % 64) {
			beat = computedBeat % 64;
			playQueue(); //play queued shots to the rhythm

			//events pass the count of their specific subdivision
			OnSixteenth?.Invoke(beat);
			if (beat % 2 == 0) OnEighth?.Invoke(beat / 2);
			if (beat % 4 == 0) OnQuarter?.Invoke(beat / 4);
			if (beat % 8 == 0) OnHalf?.Invoke(beat / 8);
			if (beat % 16 == 0) OnWhole?.Invoke(beat / 8);
			if (beat % 64 == 0) OnSet?.Invoke(beat / 64);
		}
	}
	
	public void queueShot(string name, EventReference fmodEvent, params(string name, float value)[] parameters) {
		//if shot is closer to the previous beat than the next, just play it to avoid undesireable delay
		float musicPosition = (float) (AudioSettings.dspTime - musicStart);
		float present = (float) musicPosition / secPerBeat;
		float previousBeat = (int) Mathf.Floor(musicPosition / secPerBeat);
		float nextBeat = (int) Mathf.Ceil(musicPosition / secPerBeat);

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

	private EventInstance CreateAndTrack(EventReference evt, bool autoStart = true) {
		EventInstance instance = RuntimeManager.CreateInstance(evt);
		if (autoStart) instance.start();
		allInstances.Add(instance);
		return instance;
	}
}