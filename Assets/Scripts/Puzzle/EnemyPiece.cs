using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Scriptable Objects/Puzzle System/Enemy Data", fileName = "New Enemy")]
public class EnemyPiece : ScriptableObject
{
    public float moveSpeed;
    public Sprite enemySprite;

    public bool canFreeze;

    [ShowIf("canFreeze")]
    public int freezeCost;
    [ShowIf("canFreeze")]
    public float freezeTime;
    [ShowIf("canFreeze")]
    public Sprite freezeSprite;
    [ShowIf("canFreeze")]
    [Range(2, 4f)]
    public float deFreezeSpeedBoost = 2f;


}
