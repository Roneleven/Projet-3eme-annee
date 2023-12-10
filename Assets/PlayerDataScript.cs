using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator panelAnimator;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            panelAnimator.Play("Appear");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            panelAnimator.Play("Disappear");
        }
    }
}