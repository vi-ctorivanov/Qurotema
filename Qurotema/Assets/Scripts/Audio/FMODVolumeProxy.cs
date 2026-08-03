using UnityEngine;

public class FMODVolumeProxy : MonoBehaviour
{
    [Range(0f, 1f)]
    public float masterVol = 0f;
    private float prev = -1f;

    void Update()
    {
        if (!Mathf.Approximately(masterVol, prev))
        {
            FMODUnity.RuntimeManager.StudioSystem.setParameterByName("MasterVolume", masterVol);
            prev = masterVol;
        }
    }
}