using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Struct that contains the spectum data of an audioclip at a certain playback time. Contains methods that return processed versions of itself meant for visualisers.
/// </summary>
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

    /// <summary>
    /// Calculates the average value of all frequency steps withtin the band.
    /// </summary>
    /// <returns>An array of average values for each band.</returns>
    public float[] GetBandAverages()
    {
        float[] averages = new float[bandBorders.Length + 1];
        averages[0] = getBandAverage(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            averages[i] = getBandAverage(bandBorders[i - 1], bandBorders[i]);
        }

        averages[0] = getBandAverage(bandBorders[bandBorders.Length - 1], Spectrum.Length);
        return averages;
    }

    /// <summary>
    /// Calculates the average value of the whole band.
    /// </summary>
    /// <param name="pBandBorder1">The lower border of the band.</param>
    /// <param name="pBandBorder2">The higher border of the band.</param>
    /// <returns>The average value of the band.</returns>
    private float getBandAverage(int pBandBorder1, int pBandBorder2)
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

    /// <summary>
    /// Calculates the max value of all frequency steps withtin the band.
    /// </summary>
    /// <returns>An array of max values for each band.</returns>
    public float[] GetBandMaximums()
    {
        float[] maximums = new float[bandBorders.Length + 1];
        maximums[0] = getBandMax(0, bandBorders[0]);

        for (int i = 1; i < bandBorders.Length; i++)
        {
            maximums[i] = getBandMax(bandBorders[i - 1], bandBorders[i]);
        }

        maximums[0] = getBandMax(bandBorders[bandBorders.Length - 1], Spectrum.Length);
        return maximums;
    }

    /// <summary>
    /// Calculates the max value of the whole band.
    /// </summary>
    /// <param name="pBandBorder1">The lower border of the band.</param>
    /// <param name="pBandBorder2">The higher border of the band.</param>
    /// <returns>The max value of the band.</returns>
    private float getBandMax(int pBandBorder1, int pBandBorder2)
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
