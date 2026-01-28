using UnityEngine;

[CreateAssetMenu(fileName = "AudioClips", menuName = "Scriptable Objects/AudioClips")]
public class AudioClips : ScriptableObject {
	[System.Serializable]
	public class Ambient { public AudioClip audio; }
	public Ambient[] ambiences;

   	[System.Serializable]
   	public class Sonic { public bool oneShot; public AudioClip[] audiosLo; public AudioClip[] audiosHi; public string name; }
   	public Sonic[] dynamics;
}