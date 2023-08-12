using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPiece : MonoBehaviour
{
    // OPTIMIZE WITH POOLING LATER. 
    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Piece"))
        {
            Destroy(gameObject);
        }
        if (collision.CompareTag("Player"))
        {
            GameObject.FindWithTag("Player").GetComponent<Player>().OnSufferDamage.Raise();
        }
    }
}
