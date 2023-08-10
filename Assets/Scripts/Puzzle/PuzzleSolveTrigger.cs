using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This controls the events that will take to win the level. 
/// By getting the target triggers and raising the Solve Event, from the puzzle manager. 
/// </summary>
public class PuzzleSolveTrigger : MonoBehaviour
{
    public int hitTriggersTotal;
    public int triggersHit;
    public GameEventSO OnPuzzleSolved;

    public void EstablishSolveTriggers()
    {
        hitTriggersTotal = PuzzleManager.Instance.selectedRoom.totalTriggerToHit;
    }
    public void TriggerActivated()
    {
        triggersHit++;
        if (triggersHit >= hitTriggersTotal)
        {
            // on solved, unlock the door. 
            OnPuzzleSolved.Raise();
        }
    }
}
