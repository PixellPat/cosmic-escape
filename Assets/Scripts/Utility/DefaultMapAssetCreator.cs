using UnityEditor;
using System.Collections.Generic;
using UnityEngine;


public class DefaultMapAssetCreator
{
    private readonly string folderPath = "Assets/Data/MapLayouts/";

    public DefaultMapAssetCreator()
    {
        // Ensure the folder exists upon instantiation
        EnsureFolderExists();
    }

    private void EnsureFolderExists()
    {
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets/Data", "MapLayouts");
        }
    }

    private string DetermineUniqueAssetName()
    {
        int counter = 1;
        string assetName = "DefaultMapLayout" + counter;
        while (AssetDatabase.LoadAssetAtPath<DefaultMapLayoutSO>(folderPath + assetName + ".asset"))
        {
            counter++;
            assetName = "DefaultMapLayout" + counter;
        }

        return assetName;
    }

    public void CreateDefaultMapLayout(List<RandomMapGenerator.RoomData> roomPathData)
    {
        string assetName = DetermineUniqueAssetName();

        DefaultMapLayoutSO defaultMapLayout = ScriptableObject.CreateInstance<DefaultMapLayoutSO>();
        defaultMapLayout.roomPathData = new List<RandomMapGenerator.RoomData>();

        foreach (RandomMapGenerator.RoomData room in roomPathData)
        {
            RandomMapGenerator.RoomData newRoomData = new()
            {
                Location = room.Location,
                IsEndOfPathRoom = room.IsEndOfPathRoom,
                PreviousRoomLocation = room.PreviousRoomLocation,
                PreviousRoomDirection = room.PreviousRoomDirection
            };

            defaultMapLayout.roomPathData.Add(newRoomData);
        }

        AssetDatabase.CreateAsset(defaultMapLayout, folderPath + assetName + ".asset");
    }
}