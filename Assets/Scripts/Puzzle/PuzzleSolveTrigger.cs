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

    [Tooltip("Listen to this event for the door object in the scene to unlock")]
    public GameEventSO OnPuzzleSolved;

    [Tooltip("In the event the puzzle needs to be solved again" +
        "like for instance, the freeze object needs to remain on switch" +
        "For e.g. the door should lock itself on this event if it was opened before")]
    public GameEventSO OnPuzzleUnsolved;

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
    public void TriggerDeactivated()
    {
        triggersHit--;
        OnPuzzleUnsolved.Raise();
    }
}
