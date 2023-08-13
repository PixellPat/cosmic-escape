using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    public GameObject spikeObject;
    [Tooltip("Time it takes for trap to be set")]
    public float trapTriggerTime;
    [Tooltip("Time Trap is active")]
    public float trapTime;

    private void Start()
    {
        StartCoroutine(PushTrap());
    }
    public IEnumerator PushTrap()
    {
        yield return new WaitForSeconds(trapTriggerTime);
        spikeObject.SetActive(true);
        yield return new WaitForSeconds(trapTime);
        spikeObject.SetActive(false);
        StartCoroutine(PushTrap());
    }
}
