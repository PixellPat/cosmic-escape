using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameEventSO OnSufferDamage;
    bool isNotHazard = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Hazard"))
        {
            Debug.Log("Is Probably crossing bridge");
            isNotHazard = true;
        }

        if (collision.CompareTag("Hazard") && !isNotHazard)
        {
            Debug.Log("suffer Damage by hazard");
            OnSufferDamage.Raise();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.CompareTag("Hazard"))
        {
            Debug.Log("Leaving Bridge");
            isNotHazard = false;
        }
    }
}
