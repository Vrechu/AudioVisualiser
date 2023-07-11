using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BandMixScriptableObject))]
public class BandMixEditor : Editor
{
    BandMixScriptableObject bandMix;
    public override void OnInspectorGUI()
    {
        // base.OnInspectorGUI();
        bandMix = target as BandMixScriptableObject;

        bandMix.AudioClip = (AudioClip)EditorGUILayout.ObjectField("Audio clip", bandMix.AudioClip, typeof(AudioClip), true);
        GUILayout.Space(10);

        spectrumRangeEditor();
        GUILayout.Space(10);

        EditorGUILayout.IntField("Total bands", bandMix.BandBorders().Length + 1);
        GUILayout.Space(10);

        bandRangeEditor();
        GUILayout.Space(10);

        subdivideEditor();
        GUILayout.Space(5);
        serializedObject.ApplyModifiedPropertiesWithoutUndo();
    }

    private void spectrumRangeEditor()
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.Label("Spectrum range");

            if (GUILayout.Button("-") && isValid(bandMix.SpectrumRange / 2))
            {
                bandMix.SpectrumRange /= 2;
            }

            bandMix.SpectrumRange = EditorGUILayout.IntField(bandMix.SpectrumRange);

            if (GUILayout.Button("+") && isValid(bandMix.SpectrumRange * 2))
            {
                bandMix.SpectrumRange *= 2;
            }
        }
        GUILayout.EndHorizontal();

        if (!isValid(bandMix.SpectrumRange))
        {
            EditorGUILayout.HelpBox("Length of sample buffer must be a power of two between 64 and 8192.", MessageType.Error);
        }
    }

    private bool isValid(int pNumber)
    {
        if (pNumber >= 8192 || pNumber <= 64) return false;

        float number = pNumber;
        while (number > 2)
        {
            number /= 2;
        }
        if (number != 2) return false;

        return true;
    }

    private void bandRangeEditor()
    {
        GUILayout.Label("Base band options");
        bandMix.useAutomaticBandRanges = EditorGUILayout.Toggle("Use automatic bands", bandMix.useAutomaticBandRanges);

        if (bandMix.useAutomaticBandRanges)
        {
            bandMix.bandAmount = (uint)EditorGUILayout.IntField("Base band amount", (int)bandMix.bandAmount);
            bandMix.division = (uint)EditorGUILayout.IntField("Base band division", (int)bandMix.division);
        }
        else
        {
            SerializedProperty inputBaseRanges = serializedObject.FindProperty("inputBandRanges");
            EditorGUILayout.PropertyField(inputBaseRanges);
        }
    }

    private void subdivideEditor()
    {
        bandMix.subdivide = EditorGUILayout.Toggle("Subdivide", bandMix.subdivide);
        if (!bandMix.subdivide) return;

        GUILayout.Label("Band subdivision options");
        bandMix.autoSubdivide = EditorGUILayout.Toggle("Auto subdivide", bandMix.autoSubdivide);

        if (bandMix.autoSubdivide)
        {
            bandMix.subdivideCenter = EditorGUILayout.IntField("Subdivide center", bandMix.subdivideCenter);
            bandMix.centerMultiplier = EditorGUILayout.IntField("Subdivide multiplier", bandMix.centerMultiplier);
            bandMix.subdivideWidth = EditorGUILayout.IntField("Subdivide width", bandMix.subdivideWidth);
            bandMix.subdivideFalloff = EditorGUILayout.IntField("Subdivide falloff", bandMix.subdivideFalloff);
        }
        else
        {
            SerializedProperty subdivideBands = serializedObject.FindProperty("subdivideBands");
            EditorGUILayout.PropertyField(subdivideBands);
        }
    }
}
