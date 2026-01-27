using UnityEngine;

[CreateAssetMenu(fileName = "StoryContent", menuName = "Scriptable Objects/StoryContent")]
public class StoryContent : ScriptableObject {
	public string[] introductionText;
	public string[] monolithText;
	public string[] instrumentText;
	public string[] endText;

	public string[] uniqueTextID;
	public string[] uniqueText;

	public Sprite[] monolithGraphics;
}