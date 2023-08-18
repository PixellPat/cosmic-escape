using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DefaultMapLayout", menuName = "Scriptable Objects/Default Map Layout")]
public class DefaultMapLayoutSO : ScriptableObject
{
    public List<RandomMapGenerator.RoomData> roomPathData;
}