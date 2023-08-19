using UnityEngine;

/// <summary>
/// Stores data relating to a scene within a level
/// </summary>
/// 
public class GameSceneSO : ScriptableObject
{
    [Header("Description")]
    [SerializeField] private string sceneName;
    [SerializeField] [TextArea] private string shortDescription;


    public string SceneName { get => sceneName; set => sceneName = value; }
}