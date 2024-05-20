using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerCageUI : MonoBehaviour
{
    public Animator panelAnimator;
    private FMOD.Studio.EventInstance warning;
    private bool isInsideTrigger = false;

    private void Start()
    {
        warning = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Patterns/Cage_Warning");
    }

    private void Update()
    {
        warning.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            isInsideTrigger = true;
            panelAnimator.Play("Appear");
            warning.setParameterByName("Cage", 0.0F);
            //warning.start();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("TransparentBlock"))
        {
            isInsideTrigger = false;
            StartCoroutine(DelayedExit());
        }
    }

    private IEnumerator DelayedExit()
    {
        yield return new WaitForSeconds(0.1f); // Adjust this delay as needed
        if (!isInsideTrigger)
        {
            warning.setParameterByName("Cage", 3.0F);
            panelAnimator.Play("Disappear");
        }
    }
}