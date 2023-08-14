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

    public Transform leftBound;
    public Transform rightBound;

    private Transform startPos;
    private Transform endPos;

    [BoxGroup("Game Events")]
    public GameEventSO OnPieceFrozen;
    [BoxGroup("Game Events")]
    public GameEventSO OnDefreeze;

    internal bool isMoving; // can only freeze a moving object. 
    internal bool isFrozen;
    private SpriteRenderer _renderer;

    [Tooltip("Use to Trigger Objects in Scene based on Frozen State of this object")]
    public GameObject freezeCoating;

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
        initialVelocity = pieceData.traversalTime;

        #region SETTING STARTING POSITION WITH RESPECT TO PIECE DATA 
        if (pieceData.navigateDirection == PuzzleData.MoveDirection.HORIZONTAL)
        {
            if (pieceData.startRight)
            {
                transform.position = new Vector2(rightBound.position.x, rightBound.position.y);
                startPos = rightBound;
                endPos = leftBound;
            }
            else
            {
                transform.position = new Vector2(leftBound.position.x, leftBound.position.y);
                startPos = leftBound;
                endPos = rightBound;
            }
        }
        else if (pieceData.navigateDirection == PuzzleData.MoveDirection.VERTICAL)
        {
            if (pieceData.startRight)
            {
                transform.position = new Vector2(rightBound.position.x, rightBound.position.y);
                startPos = rightBound;
                endPos = leftBound;
            }
            else
            {
                transform.position = new Vector2(leftBound.position.x, leftBound.position.y);
                startPos = leftBound;
                endPos = rightBound;
            }
        }
        #endregion

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
        Debug.Log("Should Move Vertical");
        isMoving = true;

        transform.LeanMoveLocalY(endPos.localPosition.y, initialVelocity).
          setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
           .setOnComplete(
           () =>
           {
               isMoving = false;
               Timer.Register(Random.Range(0, .5f), () =>
               {
                   isMoving = true;
                   transform.LeanMoveLocalY(startPos.localPosition.y, initialVelocity).
                   setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
                   .setOnComplete(() => verticalMoveEnd.Invoke());
               });
           }
           );
    }
    void MoveHorizontal()
    {
        isMoving = true;

        transform.LeanMoveLocalX(endPos.localPosition.x, initialVelocity).
           setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
            .setOnComplete(
            () =>
            {
                isMoving = false;
                Timer.Register(Random.Range(0, .5f), () =>
                {
                    isMoving = true;
                    transform.LeanMoveLocalX(startPos.localPosition.x, initialVelocity).
                    setEase(pieceData.moveCurves[Random.Range(0, pieceData.moveCurves.Length)])
                    .setOnComplete(() => horizontalMoveEnd.Invoke());
                });
            }
            );
    }

    int zAngle = 90;

    void MoveEuler()
    {
        isMoving = true;
        transform.LeanRotateZ(zAngle, initialVelocity).setOnComplete(() =>
        {
            isMoving = false;
            Timer.Register(Random.Range(0f, initialVelocity), () =>
                {
                    if (!isFrozen)
                        RotationRestart();
                });
        });
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
        zAngle += 90;
        if (zAngle > 360)
            zAngle = 90;
        MoveEuler();
    }
    public void ActivateFreeze()
    {
        freezeCoating.SetActive(true);
        _renderer.sprite = pieceData.frozenSprite;
        Color newColor = new Color(.2f, .85f, .98f, 1);
        newColor.a = 1;
        _renderer.color = newColor;

        LeanTween.pause(gameObject);

        isFrozen = true;
        OnPieceFrozen.Raise();

        Timer.Register(pieceData.freezeDuration, () =>
        {
            _renderer.sprite = pieceData.pieceSprite;
            _renderer.color = Color.white;

            LeanTween.resume(gameObject);
            freezeCoating.SetActive(false);
            isFrozen = false;
            if (pieceData.navigateDirection == PuzzleData.MoveDirection.ROTATIONAL)
                RotationRestart();

            OnDefreeze.Raise();
        });
    }
    private void OnMouseDown()
    {
        //using bool isMoving for ease of understanding.There are other ways to sort the issue.
        if ((pieceData.canFreeze && !isFrozen && isMoving && !(pieceData.navigateDirection == PuzzleData.MoveDirection.ROTATIONAL)) ||
            (pieceData.navigateDirection == PuzzleData.MoveDirection.ROTATIONAL && !isMoving))
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Traverse the opposite direction of movement. 
        // get direction of movement then move again. 
        transform.DOPunchPosition(new Vector2(0, 0.2f), .4f, 5).SetEase(Ease.OutBounce);
    }
}
