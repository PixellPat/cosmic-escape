using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("FreezeCoating")
            || collision.CompareTag("TriggerBox"))
        {
            PuzzleManager.Instance.CheckTrigger();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") || collision.CompareTag("FreezeCoating")
            || collision.CompareTag("TriggerBox"))
            PuzzleManager.Instance.RemoveTrigger();
    }
}
