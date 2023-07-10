using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


/// <summary>
/// Handles the audio source and sends out spectrumInfo on update.
/// </summary>
public class AudioSourceManager : MonoBehaviour
{
    private AudioSource source;
    [SerializeField] private BandMixScriptableObject bandMix;


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
        spectrumInfo = new SpectrumInfo(new float[bandMix.SpectrumRange], bandMix.BandBorders());
    }

    private void OnDestroy()
    {
        UIFunctions.OnPlayOrPause -= playOrPause;
        UIFunctions.OnTimeSet -= setTime;
    }

    private void Update()
    {
        source.GetSpectrumData(spectrumInfo.Spectrum, 0, FFTWindow.Blackman);

        if (source.isPlaying) OnAudioUpdate?.Invoke(spectrumInfo);
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
