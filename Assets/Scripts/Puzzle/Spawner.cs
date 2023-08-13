using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Spawner : MonoBehaviour
{
    public SpawnerPiece piece;
    public bool isSpawning = false;
    Vector2 spawnDirection;

    private void Start()
    {
        ConfigurePiece();
    }

    void ConfigurePiece()
    {
        isSpawning = true;
        switch (piece.direction)
        {
            case SpawnerPiece.ShotDirection.LEFT:
                spawnDirection = Vector2.left;
                break;
            case SpawnerPiece.ShotDirection.RIGHT:
                spawnDirection = Vector2.right;
                break;
            case SpawnerPiece.ShotDirection.UP:
                spawnDirection = Vector2.up;
                break;
            case SpawnerPiece.ShotDirection.DOWN:
                spawnDirection = Vector2.down;
                break;
            default:
                break;
        }
        StartCoroutine(FirePiece());
    }
    public IEnumerator FirePiece()
    {
        while (isSpawning)
        {
            yield return new WaitForSeconds(piece.spawnTime);
            var spawnObj = Instantiate(piece.spawnPrefab, transform.position, Quaternion.identity, gameObject.transform);
            spawnObj.GetComponent<Rigidbody2D>().velocity = spawnDirection * piece.spawnSpeed;
        }
    }

    [Button]
    public void StopSpawn()
    {
        isSpawning = false;
    }
}
