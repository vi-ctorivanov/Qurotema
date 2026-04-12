using UnityEngine;

public class RingsInstrument : MonoBehaviour {

    //definition
    private float minimumRadius = 0.75f;
    private float maximumRadius = 1.75f;

    //state
    private float defaultMeshRadius;
    private float radius;
    private float targetRadius;
    private float radiusEase = 4f;

    private float resonance = 0f;
    private float resonanceDecay = 0.1f;

    void Start() {
        defaultMeshRadius = transform.localScale.x;
        radius = Random.Range(minimumRadius, maximumRadius);
        resize(radius);
    }

    void Update() {
        //animate scale
        radius = Mathf.Lerp(radius, targetRadius, radiusEase * Time.deltaTime);
        transform.localScale = new Vector3(defaultMeshRadius * radius, defaultMeshRadius * radius, defaultMeshRadius * radius);

        //compute resonance and play audio
    }

    public void resize(float s) {
        targetRadius = s;
        targetRadius = Mathf.Clamp(targetRadius, minimumRadius, maximumRadius);
    }

    public void resonate() {
        
    }
}