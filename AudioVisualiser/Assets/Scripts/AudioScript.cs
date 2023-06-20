using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AudioScript : MonoBehaviour
{
    private AudioSource source;
    [SerializeField]private BandMixScriptableObject bandMix;
    
    private float[] spectrum;

    private int[] bandBorders;

    public static event Action<float[]> OnAudioUpdate;

    private void Start()
    {
        spectrum = new float[bandMix.SpectrumRange];
        source = gameObject.GetComponent<AudioSource>();
        Debug.Log("frequency: " + source.clip.frequency);
        CalculateComponents();
    }

    private void CalculateComponents()
    {
        float songFrequencyStep = bandMix.SpectrumRange / (float)source.clip.frequency;
        Debug.Log($"step size: {songFrequencyStep}");
        bandBorders = new int[bandMix.InputBandRanges.Length];

        for (int i = 0; i < bandBorders.Length; i++)
        {
            bandBorders[i] = (int)(bandMix.InputBandRanges[i] * songFrequencyStep);
            Debug.Log($"step{i}: {bandBorders[i]}");
        }
    }
    

    private void Update()
    {
        source.GetSpectrumData(spectrum,0 ,FFTWindow.Blackman);
        OnAudioUpdate?.Invoke(GetAverages());
    }    

    private float[] GetAverages()
    {
        float[] averages = new float[bandBorders.Length + 1];
        averages[0] = GetBandAverage(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            averages[i] = GetBandAverage(bandBorders[i - 1], bandBorders[i]);
        }

        averages[0] = GetBandAverage(bandBorders[bandBorders.Length -1] , bandMix.SpectrumRange);
        return averages;
    }

    private float GetBandAverage(int pBandBorder1, int pBandBorder2)
    {
        float average = 0;
        int count = 0;

        Debug.Log("1 " + pBandBorder1);
        Debug.Log("2 " + pBandBorder2);

        for (int i = pBandBorder1; i < pBandBorder2; i++)
        {
            average += spectrum[i];
            count++;
        }

        average = average / count;
        return average;
    }
}
