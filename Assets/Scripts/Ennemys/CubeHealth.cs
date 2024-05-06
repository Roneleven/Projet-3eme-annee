using UnityEngine;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    private Renderer cubeRenderer;
    private bool isDead = false;

    // Matériaux pour différents niveaux de santé
    public Transform visualRoot;
    Transform activeStateVisual;
    int maxVisualStates = 5;

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
        //Debug.Log($"Updating state for cube {name}", this);
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
        // Gérer la destruction du cube ici
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            Die();
        }
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