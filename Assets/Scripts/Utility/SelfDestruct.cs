using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float delayInSeconds;

    void Start()
    {
        // Appel de la fonction Destroy après le délai spécifié
        Destroy(gameObject, delayInSeconds);
    }
}