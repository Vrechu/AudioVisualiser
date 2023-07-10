using UnityEngine;

/// <summary>
/// Scriptable object containing a song file and their specific settings for band mixes. Contains the borders of the frequency bands.
/// </summary>
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
    [SerializeField] private int subdivideCenter = 2;
    [SerializeField] private int centerMultiplier = 3;
    [SerializeField] private int subdivideWidth = 2;
    [SerializeField] private int subdivideFalloff = 1;


    /// <summary>
    /// Calculates the band borders of the clip based on the audioclip frequency, band ranges, and spectrum range.
    /// </summary>
    /// <returns>An int array containing the inner borders of the bands.(So not 0 or the band range max.)</returns>
    public int[] BandBorders()
    {
        float songFrequencyStep = SpectrumRange / (float)AudioClip.frequency;
        int[] borders = new int[BandRanges().Length];

        for (int i = 0; i < borders.Length; i++)
        {
            borders[i] = (int)(BandRanges()[i] * songFrequencyStep);
        }
        return borders;
    }

    /// <summary>
    /// Manages the band range calculations based on mix settings. 
    /// </summary>
    /// <returns></returns>
    public int[] BandRanges()
    {
        if (useAutomaticBandRanges) bandRanges = automaticBandRanges();
        else bandRanges = inputBandRanges;

        if (autoSubdivide) bandRanges = autoSubdivideBands();
        else bandRanges = manualSubdivideBands();

        return bandRanges;
    }

    /// <summary>
    /// Divides the spectrum into an array of bands equal to the band amount input value. 
    /// Each subsequent band length is divided by the division inout value.
    /// </summary>
    /// <returns>An array of the divided bands.</returns>
    private int[] automaticBandRanges()
    {
        int[] bands = new int[bandAmount-1];
        int lastFrequency = AudioClip.frequency;

        for (int i = bands.Length - 1; i > -1; i--)
        {
            lastFrequency /= (int)division;
            bands[i] = lastFrequency;
        }
        return bands;
    }

    /// <summary>
    /// Subdives the existing bands based on the given array of vec2 values. 
    /// The first value of each band is the array position of the band that should be subdivided. 
    /// The second value is the amount of new bands it should be divided into.
    /// </summary>
    /// <returns>An array of bands with subdivisions calculated.</returns>
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

    /// <summary>
    /// Subdivides the bands based on a center point in the array. The center division is subdivided based on the center multiplier input value. 
    /// Then each value to the side of the center is subdivided less based on the distance to the center multiplied by the subdivide falloff input value.
    /// If the distance to the center is higher than the subdivide width input value, the band is not subdivided.
    /// </summary>
    /// <returns>An array of bands with subdivisions calculated.</returns>
    private int[] autoSubdivideBands()
    {
        int[] bands = bandRanges;
        int[] subdivisionPerBand = new int[bands.Length];
        int newBandAmount = 0;

        for (int i = 0; i < bands.Length; i++)
        {
            int subdivisionAmount = 1;
            int distanceToCenter = (int)Mathf.Sqrt(Mathf.Pow(i - subdivideCenter, 2));
            if (distanceToCenter <= (float)subdivideWidth)
            {
                subdivisionAmount = centerMultiplier - distanceToCenter * subdivideFalloff;
                if (subdivisionAmount < 1) subdivisionAmount = 1;
            }
            subdivisionPerBand[i] = subdivisionAmount;
            newBandAmount += subdivisionAmount;
        }

        return subdividedBands(bands, subdivisionPerBand, newBandAmount);
    }

    /// <summary>
    /// Returns an array of band range values based on subdivision input values.
    /// </summary>
    /// <param name="pBandsToSubdivide">The original bands to subdivide.</param>
    /// <param name="pSubdivisionsPerband">Array of subdivision values per band of the original band array.</param>
    /// <param name="pNewBandsLength">Array length of the new band array.</param>
    /// <returns>An array of subdivided bands.</returns>
    private int[] subdividedBands(int[] pBandsToSubdivide, int[] pSubdivisionsPerband, int pNewBandsLength)
    {
        int[] newBands = new int[pNewBandsLength];
        int counter = 0;
        int previousBand = 0;

        for (int i = 0; i < pSubdivisionsPerband.Length; i++)
        {
            int dividedBandwith = (pBandsToSubdivide[i] - previousBand) / pSubdivisionsPerband[i];

            for (int j = 0; j < pSubdivisionsPerband[i]; j++)
            {
                newBands[counter] = dividedBandwith * (j + 1) + previousBand;

                counter++;
            }

            previousBand = pBandsToSubdivide[i];
        }

        return newBands;
    }
}
