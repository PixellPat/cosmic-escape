using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

[CreateAssetMenu(menuName = "Scriptable Objects/Puzzle System/Piece", fileName = "Puzzle Piece")]
public class PuzzleData : ScriptableObject
{
    [ShowAssetPreview(15, 15)]
    public Sprite pieceSprite;

    [Tooltip("How fast it moves")]
    public float traversalTime;

    public bool startRight;

    public bool canFreeze;
    [ShowIf("canFreeze"), Tooltip("How much cells it'll cost to freeze this piece")]
    public int freezeCellCost;
    [ShowIf("canFreeze"), ShowAssetPreview(15, 15)]
    public Sprite frozenSprite;
    [Range(2f, 4f), ShowIf("canFreeze")]
    public float freezeDuration;
    public enum MoveDirection
    {
        VERTICAL, HORIZONTAL, ROTATIONAL
    }
    public MoveDirection navigateDirection;

    [Tooltip("The Max Y distance this piece can move on both sides from start position"), MinMaxSlider(-5f, 5f)]
    public Vector2 maxX;
    [Tooltip("The Max Y distance this piece can move on both sides from start position"), MinMaxSlider(-5f, 5f)]
    public Vector2 maxY;

    public bool useEaseCurves;

    [ShowIf("useEaseCurves"), Tooltip("Can be used to add some randomness to movements of piece")]
    public AnimationCurve[] moveCurves = new AnimationCurve[3];

    [Tooltip("Can the piece harm our player")]
    public bool isObstaclePiece;

    public GameObject puzzlePiecePrefab;
}
