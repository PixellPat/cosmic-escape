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
