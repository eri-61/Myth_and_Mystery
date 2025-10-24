using System;
using UnityEngine;
using UnityEngine.Video;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    public event Action<float> OnVFXVolumeChanged;
    public event Action<float> OnMusicVolumeChanged;

    [Range(0f, 1f)]
    [SerializeField] float vfxVolume = 1f;
    [Range(0f, 1f)]
    [SerializeField] float musicVolume = 1f;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    public float VFXVolume
    {
        get => vfxVolume;
        set
        {
            vfxVolume = Mathf.Clamp01(value);
            OnVFXVolumeChanged?.Invoke(vfxVolume);
        }
    }

    public float MusicVolume
    {
        get => musicVolume;
        set
        {
            musicVolume = Mathf.Clamp01(value);
            OnMusicVolumeChanged?.Invoke(musicVolume);
        }
    }

    // Helper setters for UI or other callers (normalized)
    public void SetVFXVolume(float v) => VFXVolume = v;
    public void SetMusicVolume(float v) => MusicVolume = v;

    // Percent helpers (0..100) to match your slider values
    public void SetVFXVolumePercent(float percent)
    {
        percent = Mathf.Clamp(percent, 0f, 100f);

        VFXVolume = percent / 100f;
    }

    public void SetMusicVolumePercent(float percent)
    {
        percent = Mathf.Clamp(percent, 0f, 100f);
        MusicVolume = percent / 100f;
    }

    public float GetVFXVolumePercent() => VFXVolume * 100f;
    public float GetMusicVolumePercent() => MusicVolume * 100f;

    void UpdateVideoPlayersVolume(float normalizedVolume)
    {
        var players = FindObjectsOfType<VideoPlayer>();
        for (int i = 0; i < players.Length; i++)
        {
            var vp = players[i];
            try
            {
                if (vp.audioOutputMode == VideoAudioOutputMode.AudioSource)
                {
                    try
                    {
                        var target = vp.GetTargetAudioSource(0);
                        if (target != null)
                            target.volume = normalizedVolume;
                    }
                    catch
                    {
                        var audios = vp.GetComponentsInChildren<AudioSource>();
                        foreach (var a in audios)
                            a.volume = normalizedVolume;
                    }
                }
                else if (vp.audioOutputMode == VideoAudioOutputMode.Direct)
                {
                    try
                    {
                        vp.SetDirectAudioVolume(0, normalizedVolume);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    var audios = vp.GetComponentsInChildren<AudioSource>();
                    foreach (var a in audios)
                        a.volume = normalizedVolume;
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"AudioManager: failed to set volume on VideoPlayer '{vp.name}': {ex.Message}");
            }
        }
    }
}