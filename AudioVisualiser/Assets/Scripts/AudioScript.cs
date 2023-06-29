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

    private float[] spectrum;

    private int[] bandBorders;
    private float[] bands;

    private float[] bandBuffers;
    private float[] bufferDecreases;

    public static event Action<float[], float> OnAudioUpdate;
    public static event Action<string, float, float> OnNewClipLoaded;

    private void Start()
    {
        UIFunctions.OnPlayOrPause += playOrPause;
        UIFunctions.OnTimeSet += setTime;

        source = gameObject.GetComponent<AudioSource>();
        source.clip = bandMix.AudioClip;
        spectrum = new float[bandMix.SpectrumRange];

        OnNewClipLoaded?.Invoke(source.clip.name, source.clip.length, source.clip.frequency);
        CalculateComponents();
    }

    private void OnDestroy()
    {
        UIFunctions.OnPlayOrPause -= playOrPause;
        UIFunctions.OnTimeSet -= setTime;
    }

    private void CalculateComponents()
    {
        float songFrequencyStep = bandMix.SpectrumRange / (float)bandMix.AudioClip.frequency;
        Debug.Log($"step size: {songFrequencyStep}");
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
        source.GetSpectrumData(spectrum, 0, FFTWindow.Blackman);

        if (useBandMaxes) bands = GetBandMaximums();
        else bands = GetBandAverages();

        SetBandBuffer();
        if (useBuffer) bands = bandBuffers;

        if (source.isPlaying) OnAudioUpdate?.Invoke(bands, source.time);
    }

    private float[] GetBandAverages()
    {
        float[] averages = new float[bandBorders.Length + 1];
        averages[0] = GetBandAverage(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            averages[i] = GetBandAverage(bandBorders[i - 1], bandBorders[i]);
        }

        averages[0] = GetBandAverage(bandBorders[bandBorders.Length - 1], bandMix.SpectrumRange);
        return averages;
    }

    private float GetBandAverage(int pBandBorder1, int pBandBorder2)
    {
        float average = 0;
        int count = 0;

        for (int i = pBandBorder1; i < pBandBorder2; i++)
        {
            average += spectrum[i];
            count++;
        }

        average = average / count;
        return average;
    }

    private float[] GetBandMaximums()
    {
        float[] maximums = new float[bandBorders.Length + 1];
        maximums[0] = GetBandMax(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            maximums[i] = GetBandMax(bandBorders[i - 1], bandBorders[i]);
        }

        maximums[0] = GetBandMax(bandBorders[bandBorders.Length - 1], bandMix.SpectrumRange);
        return maximums;
    }


    private float GetBandMax(int pBandBorder1, int pBandBorder2)
    {
        float max = 0;

        for (int i = pBandBorder1; i < pBandBorder2; i++)
        {
            if (spectrum[i] > max) max = spectrum[i];
        }
        return max;
    }


    private void SetBandBuffer()
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
