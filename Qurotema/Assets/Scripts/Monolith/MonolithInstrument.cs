using UnityEngine;

public class MonolithInstrument : MonoBehaviour {

    private MeshRenderer ren;
    private MaterialPropertyBlock mpb;

    private float intensity = 0f;
    private float targetIntensity = 0f;
    private float intensityFalloff = 100f;
    private float intensityMax = 1000f;

    private float value = 0f;
    private float targetValue = 0f;
    
    private float ease = 0.5f;

    void Start() {
        ren = GetComponent<MeshRenderer>();
        mpb = new MaterialPropertyBlock();
    }

    void Update() {
        targetIntensity = Mathf.Clamp(targetIntensity - intensityFalloff * Time.deltaTime, 0f, intensityMax);

        intensity = Mathf.Lerp(intensity, targetIntensity, ease * Time.deltaTime);
        value = Mathf.Lerp(value, targetValue, ease * Time.deltaTime);

        mpb.SetFloat("_Emission_Intensity", intensity);
        mpb.SetFloat("_Emission_Value", value);

        ren.SetPropertyBlock(mpb);
    }

    public void play(float v) {
        targetIntensity = intensityMax;
        targetValue = v;
    }
}