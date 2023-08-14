using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Enemy : MonoBehaviour
{
    SpriteRenderer renderer;
    public EnemyPiece enemyData;
    bool isFrozen = false;

    private void Awake()
    {
        renderer = GetComponent<SpriteRenderer>();
    }
    void Start()
    {
        // scan for a path when enabled. 

        GetComponent<AIPath>().maxSpeed = enemyData.moveSpeed;
        GetComponent<AIDestinationSetter>().target = GameObject.FindWithTag("Player").transform;
        AstarPath.active.Scan();
    }

    // in the future Optimize code with Interface IFreezeable code structure. 
    private void OnMouseDown()
    {
        Debug.Log("Freeze Enemy for duration if Freeze Cells  and data allows for it");
        if (enemyData.canFreeze && !isFrozen /* && Cost Adequate */ )
            FreezeEnemy();
    }

    public void FreezeEnemy()
    {
        isFrozen = true;
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
        isFrozen = false;
        GetComponent<AIPath>().maxSpeed = enemyData.moveSpeed + (enemyData.deFreezeSpeedBoost / enemyData.moveSpeed);
        float boostTimer = Random.Range(1.5f, enemyData.deFreezeSpeedBoost);
        Timer.Register(boostTimer, () =>
        {
            if (!isFrozen && gameObject != null)
                GetComponent<AIPath>().maxSpeed = enemyData.moveSpeed;
        });
    }
}
