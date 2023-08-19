using UnityEngine;


public class AudioController : MonoBehaviour
{

    void Start()
    {
        DAM.Instance.FadeInMusic(DAM.GameMusic.Level1Track1, 5.0f);
    }

}
