using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(AudioClipsSO))]
public class AudioClipsSOEditor : Editor
{
    // Music categories
    private SerializedProperty menuMusicTracksProperty;
    private SerializedProperty gameMusicTracksProperty;
    private SerializedProperty ambienceMusicTracksProperty;

    // SFX categories
    private SerializedProperty uiSFXTracksProperty;
    private SerializedProperty playerSFXTracksProperty;
    private SerializedProperty enemySFXTracksProperty;

    // Foldouts for categories
    private bool showMenuMusicTracks = true;
    private bool showGameMusicTracks = true;
    private bool showAmbienceMusicTracks = true;
    private bool showUISFXTracks = true;
    private bool showPlayerSFXTracks = true;
    private bool showEnemySFXTracks = true;

    private void OnEnable()
    {
        menuMusicTracksProperty = serializedObject.FindProperty("menuTracks").FindPropertyRelative("tracks");
        gameMusicTracksProperty = serializedObject.FindProperty("gameTracks").FindPropertyRelative("tracks");
        ambienceMusicTracksProperty = serializedObject.FindProperty("ambienceTracks").FindPropertyRelative("tracks");

        uiSFXTracksProperty = serializedObject.FindProperty("uiTracks").FindPropertyRelative("tracks");
        playerSFXTracksProperty = serializedObject.FindProperty("playerTracks").FindPropertyRelative("tracks");
        enemySFXTracksProperty = serializedObject.FindProperty("enemyTracks").FindPropertyRelative("tracks");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // Music Tracks
        EditorGUILayout.LabelField("Music Tracks", EditorStyles.whiteLargeLabel);

        ShowCategory(ref showMenuMusicTracks, menuMusicTracksProperty, typeof(DAM.MenuMusic), "Menu Music");
        ShowCategory(ref showGameMusicTracks, gameMusicTracksProperty, typeof(DAM.GameMusic), "Game Music");
        ShowCategory(ref showAmbienceMusicTracks, ambienceMusicTracksProperty, typeof(DAM.AmbienceMusic), "Ambience");

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        // SFX Tracks
        EditorGUILayout.LabelField("SFX Tracks", EditorStyles.whiteLargeLabel);

        ShowCategory(ref showUISFXTracks, uiSFXTracksProperty, typeof(DAM.UISFX), "UI SFX");
        ShowCategory(ref showPlayerSFXTracks, playerSFXTracksProperty, typeof(DAM.PlayerSFX), "Player SFX");
        ShowCategory(ref showEnemySFXTracks, enemySFXTracksProperty, typeof(DAM.EnemySFX), "Enemy SFX");

        serializedObject.ApplyModifiedProperties();
    }

    private void ShowCategory(ref bool foldout, SerializedProperty tracksProperty, System.Type enumType, string categoryName)
    {
        foldout = EditorGUILayout.BeginFoldoutHeaderGroup(foldout, categoryName + " (" + tracksProperty.arraySize + ")");
        if (foldout)
        {
            DrawTracks(tracksProperty, enumType);
        }
        EditorGUILayout.EndFoldoutHeaderGroup();
    }

    private void DrawTracks(SerializedProperty tracksProperty, System.Type enumType)
    {
        EditorGUILayout.BeginHorizontal();

        int oldSize = tracksProperty.arraySize;
        string newSizeStr = EditorGUILayout.TextField("Size", oldSize.ToString());

        // Check if the user enters a positive number
        if (int.TryParse(newSizeStr, out int parsedSize) && parsedSize >= 0)
        {
            if (GUILayout.Button("-", GUILayout.Width(20)) && parsedSize > 0)
            {
                parsedSize--;
            }
            else if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                parsedSize++;
            }

            if (parsedSize != oldSize)
            {
                if (parsedSize < tracksProperty.arraySize)
                {
                    if (EditorUtility.DisplayDialog("Confirm Reduction",
                        "Are you sure you want to reduce the array size? This will result in data loss.", "Yes", "No"))
                    {
                        tracksProperty.arraySize = parsedSize;
                    }
                }
                else
                {
                    int prevSize = tracksProperty.arraySize;
                    tracksProperty.arraySize = parsedSize;
                    for (int i = prevSize; i < parsedSize; i++)
                    {
                        SerializedProperty newTrackProperty = tracksProperty.GetArrayElementAtIndex(i);
                        SerializedProperty enumProperty = newTrackProperty.FindPropertyRelative("track");
                        enumProperty.intValue = 0; // Assuming "None" corresponds to 0 in your enum
                    }
                }
            }
        }
        else
        {
            if (GUILayout.Button("-", GUILayout.Width(20)) && oldSize > 0)
            {
                oldSize--;
                tracksProperty.arraySize = oldSize;
            }
            else if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                oldSize++;
                tracksProperty.arraySize = oldSize;
            }
        }


        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();

        for (int i = 0; i < tracksProperty.arraySize; i++)
        {
            SerializedProperty trackProperty = tracksProperty.GetArrayElementAtIndex(i);
            SerializedProperty enumProperty = trackProperty.FindPropertyRelative("track");
            SerializedProperty clipProperty = trackProperty.FindPropertyRelative("clip");

            enumProperty.intValue = EditorGUILayout.Popup("Track", enumProperty.intValue, System.Enum.GetNames(enumType));
            EditorGUILayout.PropertyField(clipProperty);

            EditorGUILayout.Space();
        }
    }
}
