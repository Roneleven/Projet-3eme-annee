using UnityEngine;
using UnityEngine.VFX;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    private Renderer cubeRenderer;
    private bool isDead = false;

    // Matériaux pour différents niveaux de santé
    public Transform visualRoot;
    Transform activeStateVisual;
    int maxVisualStates = 3;

    public VisualEffect CubeHit;
    public Transform CubeHitSpawnPoint;
    private VisualEffect CubeHitInstance;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        activeStateVisual = visualRoot.Find("state_0");
        maxVisualStates = visualRoot.childCount;
        UpdateMaterial();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        CubeHitInstance = Instantiate(CubeHit, CubeHitSpawnPoint.position, CubeHitSpawnPoint.rotation);
        CubeHitInstance.Play();
        CubeHitInstance.gameObject.AddComponent<VFXAutoDestroy>();
        
        if (health <= 0)
        {
            Die();
        }
        else
        {
            UpdateMaterial();
        }
    }

    [ContextMenu("Update Visual")]
    public void UpdateMaterial()
    {
        if (activeStateVisual == null)
        {
            activeStateVisual = visualRoot.Find("state_0");
            if (activeStateVisual == null)
            {
                Debug.LogWarning("No initial visual state found in CubeHealth.UpdateMaterial().");
                return;
            }
        }

        int materialIndex = Mathf.Clamp(health - 1, 0, maxVisualStates - 1);
        Transform desiredState = visualRoot.Find("state_" + materialIndex);
        if (desiredState != null)
        {
            activeStateVisual.gameObject.SetActive(false);
            desiredState.gameObject.SetActive(true);
            activeStateVisual = desiredState;
        }
        else
        {
            Debug.LogWarning("Desired visual state not found in CubeHealth.UpdateMaterial().");
        }
    }

    private void Die()
    {
        isDead = true;

        Destroy(gameObject);
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void SetDead(bool value)
    {
        isDead = value;
    }
}