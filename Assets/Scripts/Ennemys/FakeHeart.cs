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
    public Animator doorAnimator;
    private Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    public void TakeDamage(int damage)
    {
        FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Hitmarker");
        health -= damage;
        HeartHitInstance = Instantiate(HeartHit, HeartHitSpawnPoint.position, HeartHitSpawnPoint.rotation);
        HeartHitInstance.Play();
        HeartHitInstance.gameObject.AddComponent<VFXAutoDestroy>();

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
            player.IncreaseLoomParameter();
            Destroy(parentHeart);       
        }
    }
}
