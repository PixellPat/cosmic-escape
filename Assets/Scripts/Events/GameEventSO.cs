using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "GameEvent", menuName = "Scriptable Objects/Custom Events/Game Event")]
public class GameEventSO : ScriptableObject
{
    [SerializeField]
    [ContextMenuItem("Reset Name", "ResetName")]
    private string Name;

    private List<GameEventListenerSO> listeners = new List<GameEventListenerSO>();


    [ContextMenu("Raise Event")]
    public virtual void Raise()
    {
        for (int i = listeners.Count - 1; i >= 0; i--)
        {
            listeners[i].OnGameEventRaised();
        }
    }

    public virtual void RegisterListener(GameEventListenerSO gameEventListener)
    {
        if (!listeners.Contains(gameEventListener))
            listeners.Add(gameEventListener);
    }

    public virtual void UnregisterListener(GameEventListenerSO gameEventListener)
    {
        if (listeners.Contains(gameEventListener))
            listeners.Remove(gameEventListener);
    }
}