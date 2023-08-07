using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


[CreateAssetMenu(fileName = "NewGameManager", menuName = "Scriptable Objects/Game Data/Game Manager")]
public class GameManagerSO : ScriptableObject
{
    [SerializeField] private List<LevelSO> levels = new List<LevelSO>();
    [SerializeField] private List<MenuSceneSO> menus = new List<MenuSceneSO>();

    [Header("               Current Level")]
    [SerializeField] private int CurrentLevelIndex = 1;

    [Header("               Events")]
    [SerializeField] private GameEventSO OnUnloadPauseScene;


    public LevelSO GetCurrentLevel()
    {
        //Debug.Log(CurrentLevelIndex);
        return levels[CurrentLevelIndex - 1];
    }


    #region LEVELS

    // Load a scene with a given index   
    public void LoadLevelWithIndex(int index)
    {
        if (index <= levels.Count)
        {
            //Load the level
            if (index == 1)
            {
                SceneManager.LoadSceneAsync(levels[CurrentLevelIndex - 1].SceneName);
            }
        }
        //reset the index if we have no more levels or overflows during testing
        else
        {
            CurrentLevelIndex = 1;
        }
    }

    // Start next level
    public void NextLevel()
    {
        CurrentLevelIndex++;
        LoadLevelWithIndex(CurrentLevelIndex);
    }

    // Start previous level
    public void PreviousLevel()
    {
        CurrentLevelIndex--;
        LoadLevelWithIndex(CurrentLevelIndex);
    }

    // Restart current level
    public void RestartLevel()
    {
        LoadLevelWithIndex(CurrentLevelIndex);
    }

    // Main Menu = 0, New game = 1, so load level 1
    public void NewGame()
    {
        CurrentLevelIndex = 1;
        LoadLevelWithIndex(CurrentLevelIndex);
    }

    // Quit game
    public void QuitGame()
    {
        // Set index to Main menu index '0'
        CurrentLevelIndex = 0;

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
        Application.Quit();
    }

    #endregion LEVELS

    #region MENUS

    // Load Main Menu
    public void LoadMainMenu()
    {
        SceneManager.LoadSceneAsync(menus[(int)Type.Main_Menu].SceneName);
    }

    // Load Pause Menu additively on top of level scene
    public void LoadPauseMenu()
    {
        SceneManager.LoadSceneAsync(menus[(int)Type.Pause_Menu].SceneName, LoadSceneMode.Additive);
    }

    // Unload Pause Menu when click 'Go Back' button
    public void UnloadPauseMenu()
    {
        SceneManager.UnloadSceneAsync(menus[(int)Type.Pause_Menu].SceneName);

        // Raise event to InputManager to update controls state
        OnUnloadPauseScene.Raise();
    }

    // Unload Pause Menu when Pause key or gamepad button is pressed
    public void UnloadPauseMenuWithKey()
    {
        SceneManager.UnloadSceneAsync(menus[(int)Type.Pause_Menu].SceneName);
    }

    #endregion MENUS
}