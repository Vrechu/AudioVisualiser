using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct SpectrumInfo
{
    public float[] Spectrum;
    private int[] bandBorders;
    public float Time;

    public SpectrumInfo(float[] cSpectrum, int[] cBandBorders)
    { Spectrum = cSpectrum; bandBorders = cBandBorders; Time = 0; }
    
    public SpectrumInfo(SpectrumInfo cSpectrumInfo) 
    { Spectrum = cSpectrumInfo.Spectrum; bandBorders = cSpectrumInfo.bandBorders; Time = cSpectrumInfo.Time; }

    #region Band getters

    public float[] GetBandAverages()
    {
        float[] averages = new float[bandBorders.Length + 1];
        averages[0] = GetBandAverage(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            averages[i] = GetBandAverage(bandBorders[i - 1], bandBorders[i]);
        }

        averages[0] = GetBandAverage(bandBorders[bandBorders.Length - 1], Spectrum.Length);
        return averages;
    }

    private float GetBandAverage(int pBandBorder1, int pBandBorder2)
    {
        float average = 0;
        int count = 0;

        for (int i = pBandBorder1; i < pBandBorder2; i++)
        {
            average += Spectrum[i];
            count++;
        }

        average = average / count;
        return average;
    }

    public float[] GetBandMaximums()
    {
        float[] maximums = new float[bandBorders.Length + 1];
        maximums[0] = GetBandMax(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            maximums[i] = GetBandMax(bandBorders[i - 1], bandBorders[i]);
        }

        maximums[0] = GetBandMax(bandBorders[bandBorders.Length - 1], Spectrum.Length);
        return maximums;
    }


    private float GetBandMax(int pBandBorder1, int pBandBorder2)
    {
        float max = 0;

        for (int i = pBandBorder1; i < pBandBorder2; i++)
        {
            if (Spectrum[i] > max) max = Spectrum[i];
        }
        return max;
    }

    #endregion
}
