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

        DAM.Instance.FadeInMusic(DAM.GameMusic.Level1Track1, 2.0f);

    }


    private void OnPlayButtonClick()
    {
        // Play the click sound
        DAM.Instance.PlaySFX(DAM.UISFX.ButtonClick);

        DAM.Instance.TransitionTracks(DAM.GameMusic.Level1Track1, DAM.GameMusic.Level1Track1, 5.0f);

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

        //DAM.Instance.FadeOutMusic(2.0f);
    }
}