using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    SpriteRenderer renderer;
    public EnemyPiece enemyData;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        // scan for a path when enabled. 
        AstarPath.active.Scan();

        GetComponent<AIPath>().maxSpeed = enemyData.moveSpeed;
    }

    // in the future Optimize code with Interface IFreezeable code structure. 
    private void OnMouseDown()
    {
        Debug.Log("Freeze Enemy for duration if Freeze Cells  and data allows for it");
        if (enemyData.canFreeze /* && Cost Adequate */ )
            FreezeEnemy();
    }

    public void FreezeEnemy()
    {
        renderer.sprite = enemyData.freezeSprite;
        GetComponent<AIPath>().maxSpeed = 0;
        Timer.Register(enemyData.freezeTime, () =>
        {
            BoostEnemySpeed();
            renderer.sprite = enemyData.enemySprite;
        });
    }

    void BoostEnemySpeed()
    {
        // give enemy a slight speed boost on unfreeze
        GetComponent<AIPath>().maxSpeed = enemyData.moveSpeed + (enemyData.deFreezeSpeedBoost / enemyData.moveSpeed);
        float boostTimer = Random.Range(1.5f, enemyData.deFreezeSpeedBoost);
        Timer.Register(boostTimer, () =>
        {
            GetComponent<AIPath>().maxSpeed = enemyData.moveSpeed;
        });
    }
}
