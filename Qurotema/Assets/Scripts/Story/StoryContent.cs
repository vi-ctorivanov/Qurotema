using UnityEngine;

[CreateAssetMenu(fileName = "StoryContent", menuName = "Scriptable Objects/StoryContent")]
public class StoryContent : ScriptableObject {
	[TextAreaAttribute]
	public string[] introductionText;

	[TextAreaAttribute]
	public string[] monolithFirstText;

	[TextAreaAttribute]
	public string[] monolithSecondText;

	[TextAreaAttribute]
	public string[] instrumentText;

	[TextAreaAttribute]
	public string[] endText;

	[TextAreaAttribute]
	public string[] uniqueText;

	public Sprite[] monolithGraphics;
}