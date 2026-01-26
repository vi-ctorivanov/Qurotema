/*

Holds and triggers different story beats when gameplay conditions are met.
Also manages monolith text interaction.

Needs to be more or less redone, maintainability is bad.

*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class Story : MonoBehaviour {

	[Header("References")]
	public Nox n;
	public GameObject sun;
	public GameObject gates;
	public Sound soundSystem;
	public Text storyText;
	public GameObject storyTextCanvas;
	public GameObject storyBackground;

	[Header("States")]
	public bool introductionFinished = false;

	[Header("Text Animation")]
	public float textTime = 3f;
	public float opacityChangeSpeed = 0.01f;
	private float backgroundOpacity = 0f;

	[Header("Trackers")]
	public int monolithsRead = 0;
	public int stringsPlayed = 0;
	public int ringsPlayed = 0;
	public int padsPlayed = 0;

	[Header("Text")]
	public Sprite[] monolithTexts;

	//text
	private string[] talk;
	private int talkTracker = 0;
	private int instrumentDiscoveryTracker = 0;
	private Coroutine routine;

	void Start() {
		talk = new string[20];

		//intro
		talk[0] = "We have a stable port to Qurotema. RO, your vision should come in soon.";
		talk[1] = "We should remind you that because of the Gates, this is only a one-way communication channel.";
		talk[2] = "We will keep watch and study Qurotema alongside you, Operator.";
		talk[3] = "Your vessel should be equipped with a control module.\nLearn its controls through experimentation.";
		talk[4] = "Make Sino proud.";

		//first monolith discovery
		talk[5] = "What you see in front of you is one of the Monoliths Dr. Sino spoke about. There should be a writing 'eminating' from it. Do you see anything?";

		//first monolith interaction
		talk[6] = "We have a feed of the characters. Sino called this script 'i-tema'.\nHe claims there's no way to speak it, as it is a written-only language.";
		talk[7] = "This set of symbols in particular were already translated by Sino.\nThey say: 'The night will flow when black time's sand knows foreign touch.'";
		talk[8] = "Nothing else was translated other than the writing on the gates, 'Quro' and 'Tema'.";
		talk[9] = "We are not sure how Sino felt so confident in his understanding of i-tema,\nbut finding more Monoliths will help us fill a database for linguistic analysis.";

		//first instrument discovery
		talk[10] = "What is that? Nothing like that was documented in Sino's notes.";

		//instrument playtime story progress
		talk[11] = "We hav. signals of faint acit.vity aw.y from your vessel's current pos.tion.";
		talk[12] = "We've l.st sight .f your fe.d, but we are st..l getting r.adings about your p.s.tion. Ke.p up the research.";
		talk[13] = "Our co..ection is bec.ming unreli.ble. We ar. g.ing to pu.l you out s..n.";

		//ending
		talk[14] = "Lo.ing co..ection fas., wh.t's ..ing on?";
		talk[15] = "RO? Di. B.dy .. ... la... .. ..resp.n.ive. ........ ret.r. pro.ocol ........... .. .. ... .....cit.te.";
		talk[16] = "Qur..ema .. ...., .... ... ..... ... .... ves.el .. .. ... Vo.d.";
		talk[17] = "Aban.on.ng ...... ... so.ry. ...";
		talk[18] = "";

		if (!introductionFinished) {
			backgroundOpacity = 1f;
			storyBackground.GetComponent<CanvasGroup>().alpha = backgroundOpacity;
			talkTracker = 0;
			if (routine != null) StopCoroutine(routine);
			routine = StartCoroutine(PlayText(talkTracker));
		} else {
			//skip intro
		}
	}

	//set methods
	public void monolithDiscovered() {
		talkTracker = 5;
		if (routine != null) StopCoroutine(routine);
		routine = StartCoroutine(PlayText(talkTracker));
	}

	public void monolithActivated() {
		if (monolithsRead == 0) {
			talkTracker = 6;
			if (routine != null) StopCoroutine(routine);
			routine = StartCoroutine(PlayText(talkTracker));
		}

		monolithsRead++;
	}

	public void stringPlayed() {
		checkForInstrumentDiscovery();

		stringsPlayed++;
		if (stringsPlayed == 50) instrumentDiscoveryMessage();
	}

	public void ringPlayed() {
		checkForInstrumentDiscovery();

		ringsPlayed++;
		if (ringsPlayed == 40) instrumentDiscoveryMessage();
	}

	public void padPlayed() {
		checkForInstrumentDiscovery();

		padsPlayed++;
		if (padsPlayed == 30) instrumentDiscoveryMessage();
	}

	public void endGame() {
		talkTracker = 14;
		if (routine != null) StopCoroutine(routine);
		routine = StartCoroutine(PlayText(talkTracker));
	}

	private void instrumentDiscoveryMessage() {
		switch (instrumentDiscoveryTracker) {
			case 0:
				talkTracker = 11;
				break;

			case 1:
				talkTracker = 12;
				break;

			case 2:
				talkTracker = 13;
				break;
		}

		instrumentDiscoveryTracker++;

		soundSystem.shootSound("sparkles");
		n.terrain.flashFeedback();
		if (routine != null) StopCoroutine(routine);
		routine = StartCoroutine(PlayText(talkTracker));
	}

	private void checkForInstrumentDiscovery() {
		if (stringsPlayed == 0 && ringsPlayed == 0 && padsPlayed == 0) {
			talkTracker = 10;
			if (routine != null) StopCoroutine(routine);
			routine = StartCoroutine(PlayText(talkTracker));
		}
	}

	IEnumerator PlayText(int id) {
		//special actions
		switch (id) {
			//wait extra second for intro
			case 0:
				yield return new WaitForSeconds(2f);
				break;

			//allow vision
			case 1:
				StartCoroutine(FadeBackground(0f));
				break;

			//allow movement
			case 2:
				introductionFinished = true;
				break;

			//show gates
			case 13:
				gates.SetActive(true);
				gates.GetComponent<GatesStory>().activateEnd();
				break;

			//slow player down and fade screen
			case 14:
				Nox.getPlayer().GetComponent<PlayerMove>().sprintSpeed = 0.1f;
				Nox.getPlayer().GetComponent<PlayerMove>().walkSpeed = 0.05f;
				StartCoroutine(FadeBackground(1f));
				break;

			//stop music
			case 15:
				//decrease animation time for added intensity
				textTime = 1f;
				opacityChangeSpeed = 0.05f;
				soundSystem.silence();
				break;

			//end game
			case 18:
				Application.Quit();
				break;
		}

		float opacity = 0f;
		storyTextCanvas.GetComponent<CanvasGroup>().alpha = opacity;
		storyText.text = talk[id];

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

		bool end = false;
		switch (talkTracker) {
			case 4:
			case 5:
			case 9:
			case 10:
			case 11:
			case 12:
			case 13:
			case 18:
				end = true;
				break;
		}

		talkTracker++;

		yield return new WaitForSeconds(2f);

		if (!end) StartCoroutine(PlayText(talkTracker));
		else storyText.text = "";
	}

	IEnumerator FadeBackground(float target) {
		while (Mathf.Abs(backgroundOpacity - target) > 0.02f) {
			yield return new WaitForSeconds(0.01f);
			backgroundOpacity = Mathf.Lerp(backgroundOpacity, target, 0.1f * Time.deltaTime);
			storyBackground.GetComponent<CanvasGroup>().alpha = backgroundOpacity;
		}

		backgroundOpacity = target;
		storyBackground.GetComponent<CanvasGroup>().alpha = backgroundOpacity;
	}
}