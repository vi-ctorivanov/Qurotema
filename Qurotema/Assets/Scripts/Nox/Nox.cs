/*

Holds and triggers different story beats (cutscenes and text) when tracked gameplay conditions are met.
Also holds some global variables and functions.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Nox : MonoBehaviour {

	//can't be set in the inspector, because we use lazy instantiation
	[Header("References")]
	public GameObject player;
	public AnimateTerrain terrain;
	public GameObject gates;
	public GameObject storyTextCanvas;
	public Text storyText;
	public GameObject sun;
	public StoryContent content;

	[Header("States")]
	public bool introductionFinished = false;

	[Header("Text Animation")]
	public float textTime = 3f;
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
	private static Nox instance;
	public static Nox Instance {
		//here we use the ?? operator, to return 'instance' if 'instance' does not equal null
		//otherwise we assign instance to a new component and return that
		get { return instance ?? (instance = new GameObject("Nox").AddComponent<Nox>()); }
	}

	void Start() {
		player = GameObject.Find("Player");
		terrain = GetComponent<AnimateTerrain>();
		gates = GameObject.Find("Gates");
		storyTextCanvas = GameObject.Find("Story - Text");
		storyText = GameObject.Find("Story - Text/Text").GetComponent<Text>();
		sun = GameObject.Find("Sun");
		content = Resources.Load("StoryContent") as StoryContent;

		if (!introductionFinished) {
			//start intro cutscene
		} else {
			//skip intro cutscene
		}
	}

	public void monolithDiscovered() {
		playText("unique", "monolith discovery");
	}

	public void monolithActivated() {
		if (monolithsRead == 0) //start monoliths cutscene
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
		playText("instrument", instrumentsDiscovered);
		instrumentsDiscovered++;
	}

	private void checkForInstrumentDiscovery() {
		if (stringsPlayed == 0 && ringsPlayed == 0 && padsPlayed == 0) playText("unique", "instrument discovery");
	}

	public void endGame() {
		//start end cutscene
	}

	public float remap(float val, float min1, float max1, float min2, float max2) {
		if (val < min1) val = min1;
		if (val > max1) val = max1;

		return (val - min1) / (max1 - min1) * (max2 - min2) + min2;
	}

	public void playText<T>(string category, T index) {
		if (routine != null) StopCoroutine(routine);
		routine = StartCoroutine(PlayText(category, index));
	}

	IEnumerator PlayText<T>(string category, T index, bool playNext = false) {
		//select text
		string text = "";
		string[] textCategory;

		switch (category) {
			case "introduction":
				break;

			case "monolith":
				break;

			case "instrument":
				break;

			case "end":
				break;

			case "unique":
				break;
		}
		
		//execute unique actions

		//allow movement
		//introductionFinished = true;

		//make gates visible
		//gates.SetActive(true);
		//gates.GetComponent<GatesStory>().activateEnd();

		//quit application
		//Application.Quit();
		
		float opacity = 0f;
		storyTextCanvas.GetComponent<CanvasGroup>().alpha = opacity;
		storyText.text = text;

		while (opacity < 0.99f) {
			yield return new WaitForSeconds(0.01f);
			opacity += opacityChangeSpeed;
			storyTextCanvas.GetComponent<CanvasGroup>().alpha = opacity;
		}

		yield return new WaitForSeconds(textTime);

		while (opacity > 0.01f) {
			yield return new WaitForSeconds(0.01f);
			opacity -= opacityChangeSpeed;
			storyTextCanvas.GetComponent<CanvasGroup>().alpha = opacity;
		}

		yield return new WaitForSeconds(2f);

		//play other texts in this set if they exist
		if (playNext) {
			//StartCoroutine(playText());
		} else storyText.text = "";
	}
}