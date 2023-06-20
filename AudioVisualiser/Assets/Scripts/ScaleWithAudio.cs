using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleWithAudio : MonoBehaviour
{
    [SerializeField] private int band;
    [SerializeField] private float scaleIntensity = 1;


    private void Start()
    {
        AudioScript.OnAudioUpdate += AudioUpdate;
    }

    private void AudioUpdate(float [] pBandAverages)
    { 
        transform.localScale = new Vector3(1, scaleIntensity * pBandAverages[band], 1);
    }
}
