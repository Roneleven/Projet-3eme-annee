using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System;
using System.Runtime.InteropServices;
using UnityEngine.VFX;

public class Player : MonoBehaviour
{
    public static Player instance;

    public int maxHealth = 100;
    public int currentHealth;
    public HealthBar healthBar;

    public float vignetteIntensity = 0.4f;
    public float vignetteFadeSpeed = 1f;
    private VolumeProfile _volume;
    private Vignette _vignette;
    public bool isTimeOut = false;
    public GameObject GameOverCanvas;
    public TimelineInfo timelineInfo = null;
    private GCHandle timelineHandle;
    public VisualEffect knockbackEffect;
    private Coroutine resetClippaCoroutine;
    private bool isResettingClippa = false;

    private FMOD.Studio.EventInstance Loom;
    private FMOD.Studio.EVENT_CALLBACK beatCallback;

    private PlayerMovementsRB playerMovementsRB;

    public delegate void BeatEventHandler();
    public event BeatEventHandler OnBeat;

    [StructLayout(LayoutKind.Sequential)]
    public class TimelineInfo
    {
        public int currentBeat = 0;
        public FMOD.StringWrapper lastMarker = new FMOD.StringWrapper();
    }

    private void Awake()
    {
        instance = this;
    }

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

        playerMovementsRB = FindObjectOfType<PlayerMovementsRB>();

        if (Loom.isValid())
        {
            timelineInfo = new TimelineInfo();
            beatCallback = new FMOD.Studio.EVENT_CALLBACK(BeatEventCallback);
            timelineHandle = GCHandle.Alloc(timelineInfo, GCHandleType.Pinned);
            Loom.setUserData(GCHandle.ToIntPtr(timelineHandle));
            Loom.setCallback(beatCallback, FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT | FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER);
        }
    }

    [AOT.MonoPInvokeCallback(typeof(FMOD.Studio.EVENT_CALLBACK))]
static FMOD.RESULT BeatEventCallback(FMOD.Studio.EVENT_CALLBACK_TYPE type, IntPtr instancePtr, IntPtr parameterPtr)
{
    FMOD.Studio.EventInstance instance = new FMOD.Studio.EventInstance(instancePtr);

    IntPtr timelineInfoPtr;
    FMOD.RESULT result = instance.getUserData(out timelineInfoPtr);

    if (result != FMOD.RESULT.OK)
    {
        Debug.LogError("Timeline Callback error: " + result);
    }
    else if (timelineInfoPtr != IntPtr.Zero)
    {
        GCHandle timelineHandle = GCHandle.FromIntPtr(timelineInfoPtr);
        TimelineInfo timelineInfo = (TimelineInfo)timelineHandle.Target;

        switch (type)
        {
            case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_BEAT:
                {
                    var parameter = (FMOD.Studio.TIMELINE_BEAT_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_BEAT_PROPERTIES));
                    timelineInfo.currentBeat = parameter.beat;
                }
                break;
            case FMOD.Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER:
                {
                    var parameter = (FMOD.Studio.TIMELINE_MARKER_PROPERTIES)Marshal.PtrToStructure(parameterPtr, typeof(FMOD.Studio.TIMELINE_MARKER_PROPERTIES));
                    timelineInfo.lastMarker = parameter.name;

                    if (parameter.name == "beat")
                    {
                        Player.instance?.OnBeat?.Invoke();

                        // Modifier la valeur de Clippa à 0.1
                        if (Player.instance.knockbackEffect != null)
                        {
                            Player.instance.knockbackEffect.SetFloat("Clippa", 0.1f);

                            // Si une coroutine de réinitialisation est en cours, l'arrêter
                            if (Player.instance.isResettingClippa)
                            {
                                Player.instance.StopCoroutine(Player.instance.resetClippaCoroutine);
                                Player.instance.isResettingClippa = false;
                            }

                            // Démarrer une nouvelle coroutine de réinitialisation
                            Player.instance.resetClippaCoroutine = Player.instance.StartCoroutine(Player.instance.ResetClippa());
                        }
                    }
                }
                break;
        }
    }
    return FMOD.RESULT.OK;
}

private IEnumerator ResetClippa()
{
    isResettingClippa = true;

    float currentTime = 0f;
    float duration = .5f; // Durée de la transition (en secondes)

    if (Player.instance != null && Player.instance.knockbackEffect != null)
    {
        float startValue = 0.1f;
        float endValue = 1f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = currentTime / duration;

            // Interpolation linéaire entre startValue et endValue
            float newValue = Mathf.Lerp(startValue, endValue, t);

            // Réglez la valeur de Clippa
            Player.instance.knockbackEffect.SetFloat("Clippa", newValue);

            yield return null;
        }

        // Assurez-vous que la valeur finale est exacte
        Player.instance.knockbackEffect.SetFloat("Clippa", endValue);
    }

    isResettingClippa = false;
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
            Heal(100);
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
            Loom.setUserData(IntPtr.Zero);
            Loom.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
            Loom.release();
            timelineHandle.Free();
            playerMovementsRB.StopJetUse();
            playerMovementsRB.StopPlanning();
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
            Loom.setUserData(IntPtr.Zero);
            FMODUnity.RuntimeManager.PlayOneShot("event:/Character/Behaviours/Dead");
            Loom.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
            Loom.release();
            timelineHandle.Free();
            GameOverCanvas.SetActive(true);
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            //FindObjectOfType<SceneTransition>().ReloadScene();
        }
    }

#if UNITY_EDITOR
    void OnGUI()
    {
        GUILayout.Box($"Current Beat = {timelineInfo.currentBeat}, Last Marker = {(string)timelineInfo.lastMarker}");
    }
#endif

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
            TakeDamage(2);
        }
    }

    public void IncreaseLoomParameter()
    {
        Loom.getParameterByName("Loom", out float currentValue);
        Loom.setParameterByName("Loom", currentValue + 1f);
    }

    public void ResetLoomParameter()
    {
        Loom.setUserData(IntPtr.Zero);
        Loom.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Loom.release();
        //timelineHandle.Free();
    }

        private void OnDestroy()
    {
        Loom.setUserData(IntPtr.Zero);
        Loom.stop(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
        Loom.release();
        timelineHandle.Free();
    }
}