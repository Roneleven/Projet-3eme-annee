using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Animator panelAnimator;
    private FMOD.Studio.EventInstance warning;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            panelAnimator.Play("Appear");
            warning = FMODUnity.RuntimeManager.CreateInstance("event:/DestructibleBlock/Cage/Warning");
            warning.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            if (warning.isValid())
            {
                warning.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Cage/Escape");
            panelAnimator.Play("Disappear");
        }
    }
}