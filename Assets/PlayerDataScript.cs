using UnityEngine;
using UnityEngine.UI;

public class PlayerCage : MonoBehaviour
{
    public Animator panelAnimator;
    private FMOD.Studio.EventInstance warning;
    private FMOD.Studio.EventInstance currentTransparentCubeEvent; // Ajout d'une référence globale à l'événement du cube transparent

    void Start ()
    {
        warning = FMODUnity.RuntimeManager.CreateInstance("event:/DestructibleBlock/Cage/Warning");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            panelAnimator.Play("Appear");
            currentTransparentCubeEvent = FMODUnity.RuntimeManager.CreateInstance("event:/DestructibleBlock/Cage/Warning");
            currentTransparentCubeEvent.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            if (currentTransparentCubeEvent.isValid())
            {
                currentTransparentCubeEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            }
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Cage/Escape");
            panelAnimator.Play("Disappear");
        }
    }

    // Ajout d'une méthode pour arrêter le son du cube transparent si nécessaire
    public void StopTransparentCubeWarningSound()
    {
        if (currentTransparentCubeEvent.isValid())
        {
            currentTransparentCubeEvent.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }
}
