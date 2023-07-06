using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioScript : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private BandMixScriptableObject bandMix;
    [SerializeField] private bool useBandMaxes = false;
    [SerializeField] private bool useBuffer = true;


    [SerializeField] private float bufferDecreaseBase = 0.005f;
    [SerializeField] private float bufferDecreaseScale = 1.2f;

    private int[] bandBorders;
    private float[] bands;

    private float[] bandBuffers;
    private float[] bufferDecreases;

    public static event Action<SpectrumInfo> OnAudioUpdate;
    public static event Action<string, float, float> OnNewClipLoaded;


    private SpectrumInfo spectrumInfo;

    private void Start()
    {
        UIFunctions.OnPlayOrPause += playOrPause;
        UIFunctions.OnTimeSet += setTime;

        source = gameObject.GetComponent<AudioSource>();
        source.clip = bandMix.AudioClip;


        OnNewClipLoaded?.Invoke(source.clip.name, source.clip.length, source.clip.frequency);
        calculateComponents();


        spectrumInfo = new SpectrumInfo(new float[bandMix.SpectrumRange], bandBorders);
    }

    private void OnDestroy()
    {
        UIFunctions.OnPlayOrPause -= playOrPause;
        UIFunctions.OnTimeSet -= setTime;
    }

    private void calculateComponents()
    {
        float songFrequencyStep = bandMix.SpectrumRange / (float)bandMix.AudioClip.frequency;
        bandBorders = new int[bandMix.BandRanges().Length];

        for (int i = 0; i < bandBorders.Length; i++)
        {
            bandBorders[i] = (int)(bandMix.BandRanges()[i] * songFrequencyStep);
        }

        bandBuffers = new float[bandBorders.Length + 1];
        bufferDecreases = new float[bandBorders.Length + 1];
    }


    private void Update()
    {
        source.GetSpectrumData(spectrumInfo.Spectrum, 0, FFTWindow.Blackman);


        if (source.isPlaying) OnAudioUpdate?.Invoke(spectrumInfo);
    }


    private void setBandBuffer()
    {
        for (int i = 0; i < bands.Length; i++)
        {
            if (bands[i] > bandBuffers[i])
            {
                bandBuffers[i] = bands[i];
                bufferDecreases[i] = bufferDecreaseBase;
            }
            else
            {
                bandBuffers[i] -= bufferDecreases[i] * Time.deltaTime;
                bufferDecreases[i] *=  1 + (bufferDecreaseScale * Time.deltaTime);                
            }
        }
    }

    private void playOrPause(bool pIsPlay)
    {
        if (pIsPlay) source.Play();
        else source.Pause();
    }

    private void setTime(float pTime)
    {
        source.time = pTime;
    }
}
