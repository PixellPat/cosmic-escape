using UnityEngine;


[RequireComponent(typeof(AudioSource))]
public class SceneAudioSO : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip musicClip;

    [SerializeField] private GameSceneSO gameSceneSO;
    [SerializeField] private AudioClip buttonClickSFX; 


    private void Awake()
    {
        // Keep the audio object alive when a new scene is loaded
        DontDestroyOnLoad(this.gameObject);

        audioSource = GetComponent<AudioSource>();

        if (gameSceneSO.MusicClip != null)
        {
            musicClip = gameSceneSO.MusicClip;

            audioSource.clip = musicClip;
            audioSource.loop = true;
            audioSource.volume = gameSceneSO.MusicVolume;

            audioSource.Play();
        }
    }

    public void PlayButtonClickSFX()
    {
        if (buttonClickSFX != null)
        {
            audioSource.Stop();
            audioSource.PlayOneShot(buttonClickSFX);
        }
    }
}