using UnityEngine;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    public Material lvl1;
    public Material lvl2;
    public Material lvl3;
    public Material lvl4;
    public Material lvl5;
    public GameObject destructionParticlesPrefab; // Ajout de la référence au système de particules

    private Renderer cubeRenderer;
    private int previousHealth;

    private void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
        previousHealth = health;
    }

    private void Update()
    {
        UpdateMaterial();
        CheckLevelDown();
        CheckLevelUp();
        previousHealth = health;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/Destruction", transform.position);
            SpawnDestructionParticles();
            Destroy(gameObject);
        }
    }

    private void UpdateMaterial()
    {
        if (health <= 1)
        {
            cubeRenderer.material = lvl1;
        }
        else if (health <= 6)
        {
            cubeRenderer.material = lvl2;
        }
        else if (health <= 11)
        {
            cubeRenderer.material = lvl3;
        }
        else if (health <= 16)
        {
            cubeRenderer.material = lvl4;
        }
        else if (health <= 25)
        {
            cubeRenderer.material = lvl5;
        }
    }

    private void CheckLevelDown()
    {
        int currentLevel = DetermineLevel(health);
        int previousLevel = DetermineLevel(previousHealth);

        if (currentLevel < previousLevel)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/LevelLess", transform.position);
        }
    }

    private void CheckLevelUp()
    {
        if (health > previousHealth)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/MoreHP", transform.position);
        }

        previousHealth = health;
    }

    private int DetermineLevel(int healthValue)
    {
        if (healthValue <= 1)
        {
            return 1;
        }
        else if (healthValue <= 2)
        {
            return 2;
        }
        else if (healthValue <= 5)
        {
            return 3;
        }
        else if (healthValue <= 10)
        {
            return 4;
        }
        else
        {
            return 5;
        }
    }
    private void SpawnDestructionParticles()
    {
        if (destructionParticlesPrefab != null)
        {
            GameObject particles = Instantiate(destructionParticlesPrefab, transform.position, Quaternion.identity);
            Destroy(particles, 1f); // Détruire les particules après 1 seconde
        }
    }

}
