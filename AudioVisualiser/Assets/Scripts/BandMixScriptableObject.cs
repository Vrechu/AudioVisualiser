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

    [SerializeField] private bool spreadCenter = false;
    [SerializeField] private int center = 2;
    [SerializeField] private int centerMultiplier = 3;

    public int[] BandRanges()
    {

        if (useAutomaticBandRanges) bandRanges = automaticBandRanges();
        else bandRanges = inputBandRanges;

        if (spreadCenter) SubdivideBands();

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



    private void SubdivideBands()
    {
        int[] bands = bandRanges;
        int[] bandAmounts = new int[bands.Length];
        int newBandAmount = 0;

        for (int i = 0; i < bands.Length; i++)
        {
            int amount = centerMultiplier - (int)Mathf.Sqrt(Mathf.Pow(i - center, 2));
            if (amount < 1) amount = 1;
            newBandAmount += amount;
            bandAmounts[i] = amount;
        }

        int[] newBands = new int[newBandAmount];
        int counter = 0;
        int previousBand = 0;


        for (int i = 0; i < bandAmounts.Length; i++)
        {
            for (int j = 0; j < bandAmounts[i]; j++)
            {
                newBands[counter] =
                    (bands[i] - previousBand)
                    / bandAmounts[i]
                    * (j + 1)
                    + previousBand;

                counter++;
            }

            previousBand = bands[i];
        }

        bandRanges = newBands;
    }
}
