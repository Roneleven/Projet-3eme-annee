using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float delayInSeconds;

    void Start()
    {
        // Appel de la fonction Destroy apr�s le d�lai sp�cifi�
        Destroy(gameObject, delayInSeconds);
    }
}