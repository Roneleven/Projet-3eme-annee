using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Rendering.Universal;

public class FakeHeart : MonoBehaviour
{
    public int maxHealth;
    public int health;
    private FMOD.Studio.EventInstance Idle;
    private VisualEffect HeartHitInstance;
    public VisualEffect HeartHit;
    public Transform HeartHitSpawnPoint;
    public GameObject parentHeart;
    public GameObject trueHeart;
    public UniversalRendererData urpRendererData;
    private ScriptableRendererFeature xRayFeature;
    public Animator doorAnimator;

    void Start()
    {
        xRayFeature = urpRendererData.rendererFeatures.Find(feature => feature.name == "xRay");
    }

    public void TakeDamage(int damage)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Hitmarker");
        health -= damage;
        HeartHitInstance = Instantiate(HeartHit, HeartHitSpawnPoint.position, HeartHitSpawnPoint.rotation);
        HeartHitInstance.Play();
        HeartHitInstance.gameObject.AddComponent<VFXAutoDestroy>();

        StartCoroutine(ActivateXRay());

        if (health <= 0)
        {
            if (doorAnimator != null)
            {
                doorAnimator.Play("DoorOpening");
            }
            Idle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Teleport");
            Idle.start();
            trueHeart.SetActive(true);
            xRayFeature.SetActive(false);
            Destroy(parentHeart);       
        }
    }

    private IEnumerator ActivateXRay()
    {

        if (xRayFeature != null)
        {
            // Activez la feature xRay
            xRayFeature.SetActive(true);
            yield return new WaitForSeconds(2);
            // Désactivez la feature xRay
            xRayFeature.SetActive(false);
        }
    }
}
