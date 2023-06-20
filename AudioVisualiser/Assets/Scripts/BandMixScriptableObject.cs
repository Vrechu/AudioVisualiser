using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BandMix", menuName = "ScriptableObjects/BandMix")]
public class BandMixScriptableObject : ScriptableObject
{
    public int SpectrumRange = 512;
    public int[] InputBandRanges = new int[7];
}
