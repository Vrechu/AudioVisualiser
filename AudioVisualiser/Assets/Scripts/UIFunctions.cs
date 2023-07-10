using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the on-screen UI elements.
/// </summary>
public class UIFunctions : MonoBehaviour
{
    public static event Action<bool> OnPlayOrPause;
    public static event Action<float> OnTimeSet;

    private bool paused = false;


    [SerializeField] private GameObject pauseButton;
    [SerializeField] private GameObject playButton;

    [SerializeField] private Slider timer;

    [SerializeField] private TextMeshProUGUI clipName;
    [SerializeField] private TextMeshProUGUI ClipFrequency;
    [SerializeField] private TextMeshProUGUI clipTime;
    [SerializeField] private TextMeshProUGUI clipMaxTime;

    private void Awake()
    {
        PlayOrPause(false);

        AudioSourceManager.OnNewClipLoaded += setClipInfo;
        AudioSourceManager.OnAudioUpdate += setTimer;
        timer.onValueChanged.AddListener(delegate { setTimerManually(); });
    }



    private void OnDestroy()
    {
        AudioSourceManager.OnNewClipLoaded -= setClipInfo;
        AudioSourceManager.OnAudioUpdate -= setTimer;
        timer.onValueChanged.RemoveListener(delegate { setTimerManually(); });
    }

    /// <summary>
    /// Sets the ui and sends out pausing/unpausing event.
    /// </summary>
    /// <param name="pIsPlay">Set true if the function should play, false if the function should pause.</param>
    public void PlayOrPause(bool pIsPlay)
    {
        pauseButton.SetActive(pIsPlay);
        playButton.SetActive(!pIsPlay);
        OnPlayOrPause?.Invoke(pIsPlay);
        paused = !pIsPlay;
    }

    /// <summary>
    /// Sets the timer text UI to the clip playback time.
    /// </summary>
    /// <param name="pSpectrumInfo"></param>
    private void setTimer(SpectrumInfo pSpectrumInfo)
    {
        clipTime.text = toMinuteFormat(pSpectrumInfo.Time);
        timer.value = pSpectrumInfo.Time;
    }

    /// <summary>
    /// Sets the UI of the clip name, time length, and frequency.
    /// </summary>
    private void setClipInfo(string pClipName, float pMaxTime, float pClipFrequency)
    {
        clipName.text = pClipName;
        ClipFrequency.text = $"{pClipFrequency} HZ";
        clipMaxTime.text = toMinuteFormat(pMaxTime);

        timer.maxValue = pMaxTime;
    }

    
    /// <returns>A string of the given time in seconds in a minute format.</returns>
    private string toMinuteFormat(float pTimeInSeconds)
    {
        int seconds = (int)pTimeInSeconds % 60;
        int minutes = ((int)pTimeInSeconds - seconds) / 60;

        string secondsString = seconds.ToString();
        string minutesString = minutes.ToString();

        if (seconds < 10) secondsString = "0" + secondsString;
        if (minutes < 10) minutesString = "0" + minutesString;

       
        return $"{minutesString}:{secondsString}";
    }

    private void setTimerManually()
    {
            OnTimeSet?.Invoke(timer.value);
    }


}
