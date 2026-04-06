using UnityEngine;

public class MonolithInstrument : MonoBehaviour {

    private MeshRenderer ren;
    private MaterialPropertyBlock mpb;
    private float intensity = 0f;
    private float value = 0f;
    private float intensityMax = 1000f;
    private float ease = 0.5f;
    private float fastEase = 1f;

    void Start() {
        ren = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update() {
        intensity = Mathf.Lerp(intensity, 0f, ease * Time.deltaTime);
        mpb.SetFloat("_Emission_Intensity", intensity);

        ren.SetPropertyBlock(mpb);
    }

    public void play(float v) {
        intensity = Mathf.Lerp(intensity, intensityMax, fastEase * Time.deltaTime);
        value = Mathf.Lerp(value, v, ease * Time.deltaTime);

        mpb.SetFloat("_Emission_Intensity", intensity);
        mpb.SetFloat("_Emission_Value", value);
    }
}