using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI; // Drag the Panel here.
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    private bool isPaused = false;


    private void Start()
    {
        musicVolumeSlider.onValueChanged.AddListener(AdjustMusicVolume);
        sfxVolumeSlider.onValueChanged.AddListener(AdjustSFXVolume);

        pauseMenuUI.SetActive(false);
    }


    private void AdjustMusicVolume(float volume)
    {
        DAM.Instance.SetMusicVolume(volume);
    }


    private void AdjustSFXVolume(float volume)
    {
        DAM.Instance.SetSFXVolume(volume);
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }


    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;  // Resume game time.
        isPaused = false;
    }


    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;  // Freeze game time.
        isPaused = true;

        // Initialize volume sliders to reflect current volume
        musicVolumeSlider.value = DAM.Instance.GetMusicVolume();
        sfxVolumeSlider.value = DAM.Instance.GetSFXVolume();
    }


    public void QuitGame()
    {
        // Implement your quit logic. Maybe go back to the main menu or quit the application.
        SceneManager.LoadScene("MainMenu"); // if you have a scene named "MainMenu"
        // OR
        // Application.Quit();
    }
}
