using UnityEngine;
using UnityEngine.Events;


public class GameEventListenerSO : MonoBehaviour
{
    [SerializeField] private string description;
    [Tooltip("Specify the game event (scriptable object) which will raise the event")]
    [SerializeField] private GameEventSO Event;
    [SerializeField] private UnityEvent Response;


    private void OnEnable() => Event.RegisterListener(this);

    private void OnDisable() => Event.UnregisterListener(this);

    public virtual void OnGameEventRaised() => Response?.Invoke();
}