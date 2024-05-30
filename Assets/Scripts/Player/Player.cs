using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class Player : MonoBehaviour
{

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public float vignetteIntensity = 0.4f;
    public float vignetteFadeSpeed = 1f;
    private VolumeProfile _volume;
    private Vignette _vignette;
    public bool isTimeOut = false;
    public GameObject GameOverCanvas;

    private FMOD.Studio.EventInstance Loom;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        healthBar.SetMaxHealth(maxHealth);
        Loom = FMODUnity.RuntimeManager.CreateInstance("event:/UX/Ambience/LoomingTheHeart");
        Loom.start();

        Volume volume = FindObjectOfType<Volume>();
        if (volume != null)
        {
            _volume = volume.profile;

            // Ensure the vignette effect is present in the volume profile
            if (_volume.TryGet(out _vignette))
            {
                _vignette.intensity.value = 0f;
            }
            else
            {
                Debug.LogError("Vignette effect is not added to the Volume Profile.");
            }
        }
        else
        {
            Debug.LogError("Volume component not found in the scene.");
        }
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            TakeDamage(20);
        }*/

        if (Input.GetKeyDown(KeyCode.E))
      {
          Heal(20);
      }


      if (_vignette != null && _vignette.intensity.value > 0)
        {
            _vignette.intensity.value -= vignetteFadeSpeed * Time.deltaTime;
            if (_vignette.intensity.value < 0)
            {
                _vignette.intensity.value = 0;
            }
        }

        if (isTimeOut)
        {
            Loom.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        healthBar.SetHealth(currentHealth);
        FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Behaviours/Hit");

        if (_vignette != null)
        {
            _vignette.intensity.value = vignetteIntensity;
        }

        if (currentHealth <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Behaviours/Dead");
            Loom.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            GameOverCanvas.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            //FindObjectOfType<SceneTransition>().ReloadScene();
        }
    }

    public void CheckTimeout()
    {
        
    }

    public void Heal(int heal)
    {
        currentHealth += heal;
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        healthBar.SetHealth(currentHealth);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("DestroyableBlock"))
        {
            TakeDamage(5);
        }
    }
    
    public void IncreaseLoomParameter()
    {
        Loom.getParameterByName("Loom", out float currentValue);
        Loom.setParameterByName("Loom", currentValue + 1f);
    }

    public void ResetLoomParameter()
    {
        Loom.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
    }
}