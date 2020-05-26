using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TimerModule : Module
{
    public Text TimerText;

    void Update()
    {
        UpdateTimer(GameStateController.Instance.TimeLeft);
    }

    private void UpdateTimer(float time)
    {
        TimerText.text = ToTimeFormat(time);
    }

    public string ToTimeFormat(float time)
    {
        //string minutes = Mathf.Floor(time / 60).ToString("00");
        //string seconds = Mathf.Floor(time % 60).ToString("00");
        //string prettyTime = string.Format("{0:0}:{1:00}", minutes, seconds);

        string miliseconds = time.ToString("F2");
        string prettyTime = string.Format("{0}", miliseconds);
        return prettyTime;
    }
}
