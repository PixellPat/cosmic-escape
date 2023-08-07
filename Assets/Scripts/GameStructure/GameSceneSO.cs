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

    [Header("Audio")]
    [SerializeField] private AudioClip musicClip;

    [Range(0.0f, 1.0f)]
    [SerializeField] private float musicVolume;


    public string SceneName { get => sceneName; set => sceneName = value; }
    public float MusicVolume { get => musicVolume; set => musicVolume = value; }
    public AudioClip MusicClip { get => musicClip; set => musicClip = value; }
}