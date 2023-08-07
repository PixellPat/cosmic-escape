using UnityEngine;


[CreateAssetMenu(fileName = "NewMenu", menuName = "Scriptable Objects/Game Data/Menu")]
public class MenuSceneSO : GameSceneSO
{
    // Choose which type of menu from the editor
    [Header("Menu specific")]
    [SerializeField] private Type type;
}


public enum Type
{
    Main_Menu,
    Pause_Menu
    // Add more menus here
}