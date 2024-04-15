using UnityEngine;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    private Renderer cubeRenderer;
    private bool isDead = false;

    // Matériaux pour différents niveaux de santé
    public Material[] healthMaterials;

    private void Awake()
    {
        cubeRenderer = GetComponent<Renderer>();
        if (cubeRenderer == null)
        {
            Debug.LogError("CubeHealth: Renderer component not found!");
        }
        else
        {
            UpdateMaterial();
        }
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

    public void UpdateMaterial()
    {
        //Debug.Log($"Updating material for cube {name}", this);
        int materialIndex = Mathf.Clamp(health - 1, 0, healthMaterials.Length - 1);
        cubeRenderer.material = healthMaterials[materialIndex];
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