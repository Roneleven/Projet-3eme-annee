using UnityEngine;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    private Renderer cubeRenderer;
    private bool isDead = false;

    // Matériaux pour différents niveaux de santé
    public Material[] healthMaterials;

    private void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
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

    private void UpdateMaterial()
    {
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