using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;
using Random = UnityEngine.Random;
using NaughtyAttributes;

public class PuzzlePiece : MonoBehaviour
{

    [Tooltip("The Data this piece will use")]
    public PuzzleData pieceData; // we can have an object house the various Piece datas and then give them randomly
    // so they'll have a different predefined random behavior every relaunch. 

    private Action horizontalMoveEnd = delegate { };
    private Action verticalMoveEnd = delegate { };
    private Action RotationalMoveEnd = delegate { };

    [SerializeField]
    float initialVelocity = 0;
    private void Awake()
    {
        horizontalMoveEnd += HorizontalRestart;
        verticalMoveEnd += VerticalRestart;
    }

    private void Start()
    {
        //initialVelocity = pieceData.velocity * (pieceData.startRight ? 1 : -1);
        initialVelocity = pieceData.velocity;
        BeginMove();
    }

    [Button]
    public void BeginMove()
    {
        switch (pieceData.navigateDirection)
        {
            case PuzzleData.MoveDirection.VERTICAL:
                MoveVertical();
                break;
            case PuzzleData.MoveDirection.HORIZONTAL:
                MoveHorizontal();
                break;
            case PuzzleData.MoveDirection.ROTATIONAL:
                MoveEuler();
                break;
            default:
                break;
        }
    }
    void MoveVertical()
    {
    }
    void MoveHorizontal()
    {
        transform.LeanMoveX(pieceData.maxX.y, initialVelocity).setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
            .setOnComplete(
            () =>
            {
                Timer.Register(.5f, () =>
                {
                    transform.LeanMoveX(pieceData.maxX.x, initialVelocity).setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
                    .setOnComplete(() => horizontalMoveEnd.Invoke());
                });
            }
            );
    }
    void MoveEuler()
    {

    }

    void HorizontalRestart()
    {
        Timer.Register(.5f, () =>
        {
            MoveHorizontal();
        });
    }

    private void VerticalRestart()
    {
        Timer.Register(.5f, () =>
        {
            MoveVertical();
        });
    }


}
