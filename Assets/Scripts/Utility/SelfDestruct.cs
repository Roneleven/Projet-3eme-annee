using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float delayInSeconds;

    void Start()
    {
        Destroy(gameObject, delayInSeconds);
    }
}