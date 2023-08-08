using UnityEngine.Audio;
using UnityEngine;
using System;

public class AudioManager : MonoBehaviour
{
    public void Play(AudioClip clip, AudioSource source, float volume, float pitch)
    {
        if (clip != null && source != null && volume > 0 && pitch > 0)
        {  
            source.volume = volume;
            source.pitch = pitch;
            source.clip = clip;
            source.Play();
        }else{
            Debug.LogError("Audio clip quota not met.");
        }
    }
}