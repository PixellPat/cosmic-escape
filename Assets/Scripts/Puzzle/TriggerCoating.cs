using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCoating : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Switch"))
        {
            Debug.Log("Frozen in switch territory");
            PuzzleManager.Instance.CheckTrigger();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Switch"))
        {
            Debug.Log("Leaving Switch Territory");
            PuzzleManager.Instance.RemoveTrigger();
        }
    }
}
