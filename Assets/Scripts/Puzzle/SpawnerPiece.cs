using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SpawnerPiece
{
    public GameObject spawnPrefab;

    [Tooltip("The Speed of the spawned Object ")]
    public float spawnSpeed;
    
    [Tooltip("The Time it takes to spawn next Object"), Range(.1f, 2.5f)]
    public float spawnTime;
    public enum ShotDirection
    {
        LEFT, RIGHT, UP, DOWN
    }
    public ShotDirection direction;
}

