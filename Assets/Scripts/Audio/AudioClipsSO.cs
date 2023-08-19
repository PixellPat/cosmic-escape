using UnityEngine;
using static DAM;


[CreateAssetMenu(fileName = "AudioManager", menuName = "Scriptable Objects/Audio/Audio Clips")]
public class AudioClipsSO : ScriptableObject
{
    [System.Serializable]
    public class AudioTrack<T>
    {
        public T track;
        public AudioClip clip;
    }

    [System.Serializable]
    public class MenuMusicCategory
    {
        public AudioTrack<MenuMusic>[] tracks;
    }

    [System.Serializable]
    public class GameMusicCategory
    {
        public AudioTrack<GameMusic>[] tracks;
    }

    [System.Serializable]
    public class AmbienceMusicCategory
    {
        public AudioTrack<AmbienceMusic>[] tracks;
    }

    [System.Serializable]
    public class UiSfxCategory
    {
        public AudioTrack<UISFX>[] tracks;
    }

    [System.Serializable]
    public class PlayerSfxCategory
    {
        public AudioTrack<PlayerSFX>[] tracks;
    }

    [System.Serializable]
    public class EnemySfxCategory
    {
        public AudioTrack<EnemySFX>[] tracks;
    }


    [Header("Music Tracks")]
    public MenuMusicCategory menuTracks;
    public GameMusicCategory gameTracks;
    public AmbienceMusicCategory ambienceTracks;

    [Header("SFX Tracks")]
    public UiSfxCategory uiTracks;
    public PlayerSfxCategory playerTracks;
    public EnemySfxCategory enemyTracks;


    public AudioClip GetMusicClip(MenuMusic track)
    {
        return GetClipFromCategory(menuTracks.tracks, track);
    }

    public AudioClip GetMusicClip(GameMusic track)
    {
        return GetClipFromCategory(gameTracks.tracks, track);
    }

    public AudioClip GetMusicClip(AmbienceMusic track)
    {
        return GetClipFromCategory(ambienceTracks.tracks, track);
    }

    public AudioClip GetSfxClip(UISFX track)
    {
        return GetClipFromCategory(uiTracks.tracks, track);
    }

    public AudioClip GetSfxClip(PlayerSFX track)
    {
        return GetClipFromCategory(playerTracks.tracks, track);
    }

    public AudioClip GetSfxClip(EnemySFX track)
    {
        return GetClipFromCategory(enemyTracks.tracks, track);
    }


    private AudioClip GetClipFromCategory<T>(AudioTrack<T>[] category, T track)
    {
        foreach (var mt in category)
        {
            if (mt.track.Equals(track))
            {
                return mt.clip;
            }
        }
        Debug.LogWarning($"No audio clip found for the given track: {track}");
        return null;
    }

}
