using System;
using System.Collections;
using UnityEngine;


// DynamicAudioManager
public class DAM : MonoBehaviour
{
    public static DAM Instance;

    [SerializeField] private AudioClipsSO audioClipsSO;

    [SerializeField] private AudioSource musicAudioSource;
    [SerializeField] private AudioSource sfxAudioSource;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    }

    public void PlayMusic(MenuMusic track)
    {
        AudioClip clip = audioClipsSO.GetMusicClip(track);
        if (clip)
        {
            musicAudioSource.clip = clip;
            musicAudioSource.Play();
        }
    }
    
    public void PlayMusic(GameMusic track)
    {
        AudioClip clip = audioClipsSO.GetMusicClip(track);
        if (clip)
        {
            musicAudioSource.clip = clip;
            musicAudioSource.Play();
        }
    }
    
    public void PlayMusic(AmbienceMusic track)
    {
        AudioClip clip = audioClipsSO.GetMusicClip(track);
        if (clip)
        {
            musicAudioSource.clip = clip;
            musicAudioSource.Play();
        }
    }

    public void PlaySFX(UISFX sfx)
    {
        AudioClip clip = audioClipsSO.GetSfxClip(sfx);
        if (clip)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }
    
    public void PlaySFX(PlayerSFX sfx)
    {
        AudioClip clip = audioClipsSO.GetSfxClip(sfx);
        if (clip)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    public void PlaySFX(EnemySFX sfx)
    {
        AudioClip clip = audioClipsSO.GetSfxClip(sfx);
        if (clip)
        {
            sfxAudioSource.PlayOneShot(clip);
        }
    }

    public void PauseMusic()
    {
        if (musicAudioSource.isPlaying)
        {
            musicAudioSource.Pause();
        }
    }

    public void ResumeMusic()
    {
        if (!musicAudioSource.isPlaying)
        {
            musicAudioSource.Play();
        }
    }

    public void StopMusic()
    {
        musicAudioSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicAudioSource.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxAudioSource.volume = volume;
    }

    public float GetMusicVolume()
    {
        return musicAudioSource.volume;
    }

    public float GetSFXVolume()
    {
        return sfxAudioSource.volume;
    }

    public void FadeInMusic(System.Enum trackType, float duration)
    {
        AudioClip clip = null;

        if (trackType is MenuMusic)
        {
            clip = audioClipsSO.GetMusicClip((MenuMusic)trackType);
        }
        else if (trackType is GameMusic)
        {
            clip = audioClipsSO.GetMusicClip((GameMusic)trackType);
        }
        else if (trackType is AmbienceMusic)
        {
            clip = audioClipsSO.GetMusicClip((AmbienceMusic)trackType);
        }

        if (clip != null)
        {
            StartCoroutine(FadeInProcess(musicAudioSource, clip, duration));
        }
    }

    public void FadeOutMusic(float duration)
    {
        StartCoroutine(FadeOutProcess(musicAudioSource, duration));
    }

    public void TransitionTracks(Enum fromTrack, Enum toTrack, float duration)
    {
        StartCoroutine(FadeOutProcess(musicAudioSource, duration, () =>
        {
            FadeInMusic(toTrack, duration);
        }));
    }

    private IEnumerator FadeOutProcess(AudioSource audioSource, float duration, Action onFinished = null)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume; // Reset volume to original after stopping

        onFinished?.Invoke();
    }

    private IEnumerator FadeInProcess(AudioSource audioSource, AudioClip newClip, float duration)
    {
        if (newClip)
        {
            audioSource.clip = newClip;
            audioSource.Play();
        }

        audioSource.volume = 0;
        float targetVolume = 1.0f;
        while (audioSource.volume < targetVolume)
        {
            audioSource.volume += targetVolume * Time.deltaTime / duration;
            yield return null;
        }
    }

    private IEnumerator FadeOutProcess(AudioSource audioSource, float duration)
    {
        float startVolume = audioSource.volume;

        while (audioSource.volume > 0)
        {
            audioSource.volume -= startVolume * Time.deltaTime / duration;
            yield return null;
        }

        audioSource.Stop();
        audioSource.volume = startVolume;
    }


    #region ENUMS

    public enum MenuMusic
    {
        None,
        MainTrack1,
        MainTrack2,
        PauseTrack1,
        PauseTrack2,

    }

    public enum GameMusic
    {
        None,
        IntroTrack,
        Level1Track1,
        Level1Track2,
        Level2Track1,
        Level2Track2,
        CreditsTrack,

    }
    public enum AmbienceMusic
    {
        None,
        Track1,
        Track2,
        Track3,

    }

    public enum UISFX
    {
        None,
        ButtonClick,
        ButtonHover,
        PopupOpen,
        MenuOpen,
        MenuClose,
        Nav1,

    }

    public enum PlayerSFX
    {
        None,
        Walk1,
        Walk2,
        Walk3,
        Shoot1,
        Shoot2,
        Speak1,
        TakeDamage1,
        Death,

    }

    public enum EnemySFX
    {
        None,
        Walk1,
        Walk2,
        Walk3,
        Shoot1,
        Shoot2,
        Speak1,
        TakeDamage1,
        Death,

    }

    #endregion ENUMS
}
