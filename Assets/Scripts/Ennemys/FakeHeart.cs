using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

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
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
            Idle.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Behaviours/Teleport");
            Idle.start();
            trueHeart.SetActive(true);
            Destroy(parentHeart);

        }
    }
}
