using UnityEngine;


public static class AudioManager
{
    private static AudioSource _musicAudioSource;
    private static AudioSource _sfxAudioSource;


    public static void Initialize(AudioSource musicAudioSource, AudioSource sfxAudioSource)
    {
        _musicAudioSource = musicAudioSource;
        _sfxAudioSource = sfxAudioSource;
    }

    public static void PlayMusic(AudioClip musicClip)
    {
        _musicAudioSource.clip = musicClip;
        _musicAudioSource.loop = true;
        _musicAudioSource.Play();
    }

    public static void PlaySFX(AudioClip sfxClip)
    {
        _sfxAudioSource.PlayOneShot(sfxClip);
    }

    public static void StopMusic(AudioClip musicClip)
    {
        _musicAudioSource.clip = musicClip;
        _musicAudioSource.Stop();
    }



    // ... Other methods as needed
}
