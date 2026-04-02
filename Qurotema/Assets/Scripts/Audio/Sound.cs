/*

Creates sound events for each sound, tracks bpm, manages 'energy'.

*/

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Sound : MonoBehaviour {

	[Header("States")]
	public float energy = 20f; //range: 0 - 100
	public float energyFalloff = 0.6f;
	private int energyState = 1;
	private float highEnergyTreshold = 60f;
	private float lowEnergyThreshold = 10f;

	[Header("Timing")]
	public int beat = 1;
	public bool beatChange = false;
	public float bpm = 120f;

	private float musicStart;
	private float secPerBeat;
	private float musicPosition;
	private float musicPositionInBeats;
	private int bars = 0;
	
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
		musicStart = (float) AudioSettings.dspTime;
	}

	void Update() {
		ambienceState.setParameterByName("Energy", energy);

		if (energy < lowEnergyThreshold && energyState != 0) energyState = 0;

		//energy falloff
		energy -= energyFalloff * Time.deltaTime;
		energy = Mathf.Clamp(energy, 0f, 100f);

		//beats
		musicPosition = (float) (AudioSettings.dspTime - musicStart);
		int currentBeat = (int) Mathf.Floor(musicPosition / secPerBeat);

		beatChange = false;

		if (beat + (bars * 16) != currentBeat) {
			beatChange = true;
			beat = currentBeat - (bars * 16);
		}
		
		if (beatChange && beat == 16) bars++;

		//play appropriate sound set for different energy
		if (energy > highEnergyTreshold) {
			if (energyState != 2) energyState = 2;
		} else if (energy > lowEnergyThreshold) {
			if (energyState != 1) energyState = 1;
		}
	}

	public void addEnergy(float amount) {
		energy += amount * Time.deltaTime;
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