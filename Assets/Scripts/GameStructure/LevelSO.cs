using UnityEngine;


[CreateAssetMenu(fileName = "NewLevel", menuName = "Scriptable Objects/Game Data/Level")]
public class LevelSO : GameSceneSO
{
    [Header("Level specific")]
    [SerializeField] private bool levelSpecific;
}