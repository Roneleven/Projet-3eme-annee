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

    private void Start()
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
    private void UpdateMaterial()
    {
        int materialIndex = Mathf.Clamp(health - 1, 0, maxVisualStates - 1);
        //cubeRenderer.material = healthMaterials[materialIndex];
        Transform desiredState = visualRoot.Find("state_" + materialIndex);
        if(desiredState != null)
        {
            activeStateVisual.gameObject.SetActive(false);
            desiredState.gameObject.SetActive(true);
            activeStateVisual = desiredState;
        }
    }

    private void Die()
    {
        isDead = true;
        // Gérer la destruction du cube ici
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