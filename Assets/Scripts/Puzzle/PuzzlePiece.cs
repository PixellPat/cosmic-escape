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
    private Action rotationalMoveEnd = delegate { };

    [BoxGroup("Game Events")]
    public GameEventSO OnPieceFrozen;
    [BoxGroup("Game Events")]
    public GameEventSO OnDefreeze;

    internal bool isMoving; // can only freeze a moving object. 
    internal bool isFrozen;
    private SpriteRenderer _renderer;

    float initialVelocity = 0;

    private void Awake()
    {
        horizontalMoveEnd += HorizontalRestart;
        verticalMoveEnd += VerticalRestart;
        rotationalMoveEnd += RotationRestart;

        _renderer = GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        //initialVelocity = pieceData.velocity * (pieceData.startRight ? 1 : -1);
        initialVelocity = pieceData.velocity;
        _renderer.sprite = pieceData.pieceSprite;
        BeginMove();
    }

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
        isMoving = true;
        transform.LeanMoveY(pieceData.maxY.y, initialVelocity).setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
             .setOnComplete(
             () =>
             {
                 isMoving = false;
                 Timer.Register(.5f, () =>
                 {
                     isMoving = true;
                     transform.LeanMoveY(pieceData.maxY.x, initialVelocity).setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
                     .setOnComplete(() => verticalMoveEnd.Invoke());
                 });
             }
             );
    }
    void MoveHorizontal()
    {
        isMoving = true;
        transform.LeanMoveX(pieceData.maxX.y, initialVelocity).setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
             .setOnComplete(
             () =>
             {
                 isMoving = false;
                 Timer.Register(.5f, () =>
                 {
                     isMoving = true;
                     transform.LeanMoveX(pieceData.maxX.x, initialVelocity).setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
                     .setOnComplete(() => horizontalMoveEnd.Invoke());
                 });
             }
             );
    }
    void MoveEuler()
    {
        // rotate the object 360. 
        isMoving = true;
        transform.LeanRotateAround(Vector3.forward, 306, initialVelocity).setOnComplete(rotationalMoveEnd.Invoke);
    }
    void HorizontalRestart()
    {
        isMoving = false;
        Timer.Register(.5f, () =>
        {
            MoveHorizontal();
        });
    }
    void VerticalRestart()
    {
        isMoving = false;
        Timer.Register(.5f, () =>
        {
            MoveVertical();
        });
    }

    void RotationRestart()
    {
        isMoving = false;
        MoveEuler();
    }
    public void ActivateFreeze()
    {
        Debug.Log("Freeze Piece");
        isFrozen = true;
        _renderer.sprite = pieceData.frozenSprite;
        LeanTween.pause(gameObject);
        OnPieceFrozen.Raise();

        Timer.Register(pieceData.freezeDuration, () =>
        {
            _renderer.sprite = pieceData.pieceSprite;
            LeanTween.resume(gameObject);
            isFrozen = false;
            OnDefreeze.Raise();
        });
    }
    private void OnMouseDown()
    {
        //using bool isMoving for ease of understanding.There are other ways to sort the issue.
        if (pieceData.canFreeze && !isFrozen && isMoving)
        {
            ActivateFreeze();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && pieceData.isObstaclePiece)
        {
            collision.GetComponent<Player>().OnSufferDamage.Raise();
        }
    }

}
