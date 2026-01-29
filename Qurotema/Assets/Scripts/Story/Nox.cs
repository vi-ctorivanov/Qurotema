/*

Holds and triggers different story beats (cutscenes and text) when tracked gameplay conditions are met.
Also holds some global variables and functions.

*/
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Playables;
using TMPro;

public class Nox : MonoBehaviour {

	[Header("References")]
	public GameObject player;
	public AnimateTerrain terrain;
	public GameObject gates;
	public GameObject gatesSphere;
	public GameObject storyTextCanvas;
	public TMP_Text storyText;
	public StoryContent content;
	public PlayableDirector director;
	public PlayableAsset introductionTimeline;
	public PlayableAsset introductionEndTimeline;
	public PlayableAsset monolithTimeline;
	public PlayableAsset gatesTimeline;
	public PlayableAsset endTimeline;

	[Header("States")]
	public bool introductionFinished = false;

	[Header("Text Animation")]
	public float textTime = 1f;
	public float opacityChangeSpeed = 0.01f;

	[Header("Trackers")]
	public int monolithsRead = 0;
	public int instrumentsDiscovered = 0;
	public int stringsPlayed = 0;
	public int ringsPlayed = 0;
	public int padsPlayed = 0;

	[Header("Coroutines")]
	private Coroutine routine;

	//create static singleton to act as a globally accessible Nox
	//if instance is null (it is at first), set it to this object so all references point to it
	private static Nox instance;
	public static Nox Instance {
		get { 
			if (instance == null) instance = GameObject.Find("Nox").GetComponent<Nox>();
			return instance;
		}
	}

	void Start() {
		gates.SetActive(false);

		if (!introductionFinished) {
			directorPlay(introductionTimeline);
		} else {
			//skip intro cutscene
			directorPlay(introductionEndTimeline);
		}
	}

	public void monolithDiscovered() {
		playText("unique" + "_" + "monolith discovery");
	}

	public void monolithActivated() {
		if (monolithsRead == 0) directorPlay(monolithTimeline);
		monolithsRead++;
	}

	public void stringPlayed() {
		checkForInstrumentDiscovery();
		stringsPlayed++;
		if (stringsPlayed == 50) instrumentMasteryMessage();
	}

	public void ringPlayed() {
		checkForInstrumentDiscovery();
		ringsPlayed++;
		if (ringsPlayed == 40) instrumentMasteryMessage();
	}

	public void padPlayed() {
		checkForInstrumentDiscovery();
		padsPlayed++;
		if (padsPlayed == 30) instrumentMasteryMessage();
	}

	private void instrumentMasteryMessage() {
		Sound.Instance.shootSound("sparkles");
		terrain.flashFeedback();
		playText("instrument" + "_" + instrumentsDiscovered);
		instrumentsDiscovered++;
	}

	private void checkForInstrumentDiscovery() {
		if (stringsPlayed == 0 && ringsPlayed == 0 && padsPlayed == 0) playText("unique" + "_" + "instrument discovery");
	}

	public void endGame() {
		directorPlay(endTimeline);
	}

	public float remap(float val, float min1, float max1, float min2, float max2) {
		if (val < min1) val = min1;
		if (val > max1) val = max1;

		return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
	}

	//text id is defined as category_index, as signal system only accepts methods with maximum 1 parameter,
	//and we sometimes use integers and strings as the index
	public void playText(string id) {
		if (routine != null) StopCoroutine(routine);
		routine = StartCoroutine(PlayText(id));
	}

	IEnumerator PlayText(string id) {
		//parse id
		string category = id.Split("_")[0];
		string i = id.Split("_")[1];
		int index = -1;
		int.TryParse(i, out index);

		//select text
		string text = "";

		switch (category) {
			case "introduction":
				text = content.introductionText[index];
				break;

			case "monolith":
				text = content.monolithText[index];
				break;

			case "instrument":
				text = content.instrumentText[index];
				break;

			case "end":
				text = content.endText[index];
				break;

			case "unique":
				text = content.uniqueText[index];
				break;
		}
		
		//write in
		storyText.text = "";

		float opacity = 1f;
		storyTextCanvas.GetComponent<CanvasGroup>().alpha = opacity;

		int textTracker = 0;
		storyText.maxVisibleCharacters = textTracker;
		storyText.text = text;

		while (storyText.maxVisibleCharacters < text.Length) {
			yield return new WaitForSeconds(0.03f);
			textTracker++;
			storyText.maxVisibleCharacters = textTracker;
		}

		yield return new WaitForSeconds(textTime);

		//fade out
		while (opacity > 0.01f) {
			yield return new WaitForSeconds(0.01f);
			opacity -= opacityChangeSpeed;
			storyTextCanvas.GetComponent<CanvasGroup>().alpha = opacity;
		}

		storyTextCanvas.GetComponent<CanvasGroup>().alpha = 0f;
	}

	private void directorPlay(PlayableAsset timeline) {
		if (director) {
			director.playableAsset = timeline;
			director.RebuildGraph();
			director.time = 0f;
			director.Play();
		}
	}

	//special cutscene actions, executed through signals
	public void endIntroduction() {
		directorPlay(introductionEndTimeline);
	}

	public void allowMovement() {
		introductionFinished = true;
	}

	public void makeGatesVisible() {
		directorPlay(gatesTimeline);
		//gates.SetActive(true);
		//quro.SetActive(false);
		//tema.SetActive(false);

		// quro.SetActive(true);
		// tema.SetActive(true);
		// Color c = new Color(1f, 1f, 1f, 1f);
		// GetComponent<Renderer>().material.SetColor("_EmissiveColor", c * 60);
		// sphere.GetComponent<Renderer>().material.SetColor("_EmissiveColor", c * 60);
	}

	public void quitApplication() {
		Application.Quit();
	}
}