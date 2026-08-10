/*

Menu manager.

*/

using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	[Header("Timelines")]
	public PlayableDirector director;
	public PlayableAsset menuStartTimeline;
	public PlayableAsset menuEndTimeline;

	// state
	bool primedQuit = false;
	bool primedStart = false;
	// progression states

	void Start() {
		directorPlay(menuStartTimeline);
	}

	private void directorPlay(PlayableAsset timeline) {
		if (director) {
			director.playableAsset = timeline;
			director.RebuildGraph();
			director.time = 0f;
			director.Play();
		}
	}

	public void startGame() {
		primedStart = true;
		primedQuit = false;
		directorPlay(menuEndTimeline);
	}

	public void endGame() {
		primedStart = false;
		primedQuit = true;
		directorPlay(menuEndTimeline);
	}

	public void menuEndHandler() {
		if (primedStart) SceneManager.LoadScene("Qurotema", LoadSceneMode.Single);
		else if (primedQuit) Application.Quit();
	}
}