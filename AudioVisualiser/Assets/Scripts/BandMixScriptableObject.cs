using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BandMix", menuName = "ScriptableObjects/BandMix")]
public class BandMixScriptableObject : ScriptableObject
{
    public int SpectrumRange = 512;
    public AudioClip AudioClip;

    private int[] bandRanges;

    [SerializeField]
    private int[] inputBandRanges = new int[7];



    [SerializeField] private bool useAutomaticBandRanges;
    [SerializeField] private uint bandAmount;
    [SerializeField] private uint division;

    [SerializeField] private Vector2Int[] subdivideBands; 
    [SerializeField] private bool autoSubdivide = false;
    [SerializeField] private int center = 2;
    [SerializeField] private int centerMultiplier = 3;

    public int[] BandRanges()
    {

        if (useAutomaticBandRanges) bandRanges = automaticBandRanges();
        else bandRanges = inputBandRanges;

        if (autoSubdivide) bandRanges = autoSubdivideBands();
        else bandRanges = manualSubdivideBands();

        return bandRanges;
    }

    private int[] automaticBandRanges()
    {
        int[] bands = new int[bandAmount];
        int lastFrequency = AudioClip.frequency;

        for (int i = bands.Length - 1; i > -1; i--)
        {
            lastFrequency /= (int)division;
            bands[i] = lastFrequency;
        }
        return bands;
    }

    private int[] manualSubdivideBands()
    {
        int[] subdivisionPerBand = new int[bandRanges.Length];
        int newBandAmount = bandRanges.Length;

        for (int i = 0; i < bandRanges.Length; i++)
        {
            subdivisionPerBand[i] = 1;

            for (int j = 0; j < subdivideBands.Length; j++)
            {
                if (i == subdivideBands[j].x)
                {
                    subdivisionPerBand[i] = subdivideBands[j].y;
                    newBandAmount += subdivideBands[j].y - 1;
                    break;
                }
            }
        }

        return subdividedBands(bandRanges, subdivisionPerBand, newBandAmount);
    }

    private int[] autoSubdivideBands()
    {
        int[] bands = bandRanges;
        int[] subdivisionPerBand = new int[bands.Length];
        int newBandAmount = 0;

        for (int i = 0; i < bands.Length; i++)
        {
            int subdivisionAmount = centerMultiplier - 
                (int)Mathf.Sqrt(Mathf.Pow(i - center, 2));

            if (subdivisionAmount < 1) subdivisionAmount = 1;

            subdivisionPerBand[i] = subdivisionAmount;
            newBandAmount += subdivisionAmount;
        }

        return subdividedBands(bands, subdivisionPerBand, newBandAmount);
    }

    private int[] subdividedBands(int[] pBandToSubdivide, int[] pSubdivisionsPerband, int pNewBandsLength)
    {
        int[] newBands = new int[pNewBandsLength];
        int counter = 0;
        int previousBand = 0;

        for (int i = 0; i < pSubdivisionsPerband.Length; i++)
        {
            int dividedBandwith = (pBandToSubdivide[i] - previousBand) / pSubdivisionsPerband[i];

            for (int j = 0; j < pSubdivisionsPerband[i]; j++)
            {
                newBands[counter] = dividedBandwith * (j + 1) + previousBand;

                counter++;
            }

            previousBand = pBandToSubdivide[i];
        }

        return newBands;
    }
}
