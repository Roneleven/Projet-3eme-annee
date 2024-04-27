using UnityEngine;

public class TriggerDestroy : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Block") || other.CompareTag("HeartBlock"))
        {
            Destroy(other.gameObject);
        }
    }
}
