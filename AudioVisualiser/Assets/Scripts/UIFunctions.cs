using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

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

        AudioScript.OnNewClipLoaded += setClipInfo;
        AudioScript.OnAudioUpdate += setTimer;
        timer.onValueChanged.AddListener(delegate { setTimerManually(); });
    }



    private void OnDestroy()
    {
        AudioScript.OnNewClipLoaded -= setClipInfo;
        AudioScript.OnAudioUpdate -= setTimer;
        timer.onValueChanged.RemoveListener(delegate { setTimerManually(); });
    }

    /// <summary>
    /// Sets the ui and sends out pauseing/unpausing event.
    /// </summary>
    /// <param name="Set true if the function should play, false if the function should pause."></param>
    public void PlayOrPause(bool pIsPlay)
    {
        pauseButton.SetActive(pIsPlay);
        playButton.SetActive(!pIsPlay);
        OnPlayOrPause?.Invoke(pIsPlay);
        paused = !pIsPlay;
    }

    private void setTimer(float[] pStuff, float pTime)
    {
        clipTime.text = ToMinuteFormat(pTime);
        timer.value = pTime;
    }

    private void setClipInfo(string pClipName, float pMaxTime, float pClipFrequency)
    {
        clipName.text = pClipName;
        ClipFrequency.text = $"{pClipFrequency} HZ";
        clipMaxTime.text = ToMinuteFormat(pMaxTime);

        timer.maxValue = pMaxTime;
    }

    private string ToMinuteFormat(float pTimeInSeconds)
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
