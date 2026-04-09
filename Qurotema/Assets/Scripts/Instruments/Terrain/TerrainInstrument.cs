using UnityEngine;

public class TerrainInstrument : MonoBehaviour {

    [Header("References")]
    public Material terrainMaterial;

    private float intensity = 0f;
    private float targetIntensity = 0f;
    private float intensityFalloff = 1000f;
    private float intensityMax = 500f;

    private float value = 0f;

    private float ease = 500f;

    void Update() {
        targetIntensity = Mathf.Clamp(targetIntensity - intensityFalloff * Time.deltaTime, 0f, intensityMax);
        intensity = Mathf.Lerp(intensity, targetIntensity, ease * Time.deltaTime);

        terrainMaterial.SetFloat("_Play_Intensity", intensity);
        terrainMaterial.SetFloat("_Play_Value", value);
    }

    public void play(float v) {
        targetIntensity = intensityMax;
        value = v;
    }
}