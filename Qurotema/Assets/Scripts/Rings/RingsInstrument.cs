using UnityEngine;

public class RingsInstrument : MonoBehaviour {

    //definition
    private float minimumRadius = 0.75f;
    private float maximumRadius = 1.5f;

    //state
    private float defaultMeshRadius;
    private float radius;
    private float resonance = 0f;
    private float resonanceDecay = 0.1f;

    void Start() {
        defaultMeshRadius = transform.localScale.x;
        radius = Random.Range(minimumRadius, maximumRadius);
        transform.localScale = new Vector3(defaultMeshRadius * radius, defaultMeshRadius * radius, defaultMeshRadius * radius);
        resize();
    }

    void Update() {
        //compute resonance and play audio
    }

    public void resize() {
        
    }

    public void resonate() {
        
    }
}