using UnityEngine;
using System;

public static class AudioEvents
{
    public static event Action<string> OnPlaySFX;

    public static event Action<string> OnPlayBGM;

    public static event Action OnStopBGM;


    public static void TriggerPlaySFX(string sfxName)
    {
        OnPlaySFX?.Invoke(sfxName);
    }

    public static void TriggerPlayBGM(string bgmName)
    {
        OnPlayBGM?.Invoke(bgmName);
    }

    public static void TriggerStopBGM()
    {
        OnStopBGM?.Invoke();
    }
}