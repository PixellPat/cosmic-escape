using UnityEngine;
using UnityEngine.UI;


public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button playButton;
    [SerializeField] private Button quitButton;


    private void Start()
    {
        playButton.onClick.AddListener(OnPlayButtonClick);
        quitButton.onClick.AddListener(OnQuitButtonClick);

        DAM.Instance.FadeInMusic(DAM.MenuMusic.MainTrack2, 5.0f);
    }


    private void OnPlayButtonClick()
    {
        // Play the click sound
        DAM.Instance.PlaySFX(DAM.UISFX.ButtonClick);
    }

    private void OnQuitButtonClick()
    {
        // Play the click sound
        DAM.Instance.PlaySFX(DAM.UISFX.ButtonClick);
    }


    private void OnDestroy()
    {
        playButton.onClick.RemoveListener(OnPlayButtonClick);
        quitButton.onClick.RemoveListener(OnQuitButtonClick);

        DAM.Instance.FadeOutMusic(2.0f);
    }
}