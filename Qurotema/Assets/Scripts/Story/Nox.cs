/*

Holds and triggers different story beats (cutscenes and text) when tracked gameplay conditions are met.
Also holds some global variables and functions.

*/
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using TMPro;
using UnityEngine.Rendering;

public class Nox : MonoBehaviour {

	[Header("Settings")]
	public bool skipIntro = false;

	[Header("References")]
	public GameObject player;
	public GameObject cam;
	public AnimateTerrain terrain;
	public GameObject gates;
	public GameObject gatesSphere;
	public GameObject gatesCollider;
	public GameObject pillar;
	public GameObject storyTextCanvas;
	public GameObject fadeOut;
	public TMP_Text storyText;
	public StoryContent content;
	public static event Action<int> OnActivateMonolith;
	public static event Action<int> OnMasterInstrument;
	public static event Action<float> OnFlashFeedback;
	public static event Action OnIntroductionFinished;
	public static event Action OnGatesAppear;

	[Header("Timelines")]
	public PlayableDirector director;
	public PlayableAsset introductionTimeline;
	public PlayableAsset introductionEndTimeline;
	public PlayableAsset monolithTimeline;
	public PlayableAsset gatesTimeline;
	public PlayableAsset endTimeline;

	//dynamics
	private Vector3 targetPillarSize;

	[Header("Text Animation")]
	public float textLetterTime = 0.03f;
	private float textTime = 1f;
	private float opacityChangeSpeed = 0.01f;

	[Header("Trackers")]
	public int monolithsRead = 0;
	private int instrumentsMastered = 0;
	private int stringsPlayed = 0;
	private int ringsPlayed = 0;
	private int padsPlayed = 0;

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
		SupportedRenderingFeatures.active.rendersUIOverlay = false; //todo: document why we are doing this

		gates.SetActive(false);
		gatesCollider.SetActive(false);
		fadeOut.SetActive(false);
		targetPillarSize = pillar.transform.localScale;

		if (skipIntro) directorPlay(introductionEndTimeline);
		else directorPlay(introductionTimeline);
	}

	void Update() {
		pillar.transform.localScale = Vector3.Lerp(pillar.transform.localScale, targetPillarSize, 0.8f * Time.deltaTime);
	}

	public void monolithDiscovered() {
		playText("unique_0");
	}

	public void monolithActivated() {
		OnFlashFeedback?.Invoke(3f);
		if (monolithsRead == 0) directorPlay(monolithTimeline);
		monolithsRead++;
		OnActivateMonolith?.Invoke(monolithsRead);
	}

	public void stringPlayed() {
		terrain.addFeedback(2.0f);
		checkForInstrumentDiscovery();
		stringsPlayed++;
		if (stringsPlayed == 30) instrumentMastered();
	}

	public void ringPlayed() {
		terrain.addFeedback(2.0f);
		checkForInstrumentDiscovery();
		ringsPlayed++;
		if (ringsPlayed == 30) instrumentMastered();
	}

	public void padPlayed() {
		terrain.addFeedback(2.0f);
		checkForInstrumentDiscovery();
		padsPlayed++;
		if (padsPlayed == 30) instrumentMastered();
	}

	private void instrumentMastered() {
		FMODUnity.RuntimeManager.PlayOneShot(Sound.Instance.momentEvent);
		OnFlashFeedback?.Invoke(3f);
		playText("instrument" + "_" + instrumentsMastered);
		targetPillarSize = new Vector3(pillar.transform.localScale.x * 0.5f, pillar.transform.localScale.y, pillar.transform.localScale.z * 0.5f);
		if (instrumentsMastered >= 2) {
			targetPillarSize = new Vector3(0f, pillar.transform.localScale.y, 0f); //make beam invisible when gates appear in the end
			makeGatesVisible();
		}
		instrumentsMastered++;
		OnMasterInstrument?.Invoke(instrumentsMastered);
	}

	private void checkForInstrumentDiscovery() {
		if (stringsPlayed == 0 && ringsPlayed == 0 && padsPlayed == 0) playText("unique_1");
	}

	public void endGame() {
		directorPlay(endTimeline);
	}

	public float remap(float val, float min1, float max1, float min2, float max2) {
		if (val < min1) val = min1;
		if (val > max1) val = max1;

		return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
	}

	public int cardinalDirection(Vector2 cameraForward, Vector2 toObject) {
		float angle = Vector2.SignedAngle(toObject, cameraForward);
		
		//normalize to [0, 360] since SignedAngle returns [-180, 180]
		float normalizedAngle = (angle + 360f) % 360f;
		
		if (normalizedAngle < 45f || normalizedAngle >= 315f) return 0;
		else if (normalizedAngle < 135f) return 1;
		else if (normalizedAngle < 225f) return 2;
		else return 3;
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
			yield return new WaitForSeconds(textLetterTime);
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

	public void makeGatesVisible() {
		directorPlay(gatesTimeline);
		OnGatesAppear?.Invoke();
	}

	//special cutscene actions, executed through signals
	public void endIntroduction() {
		directorPlay(introductionEndTimeline);
	}

	public void allowMovement() {
		OnIntroductionFinished?.Invoke();
	}

	public void quitApplication() {
		Application.Quit();
	}
}