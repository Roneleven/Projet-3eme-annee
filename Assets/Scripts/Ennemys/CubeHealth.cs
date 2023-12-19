using UnityEngine;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    public Material lvl1;
    public Material lvl2; 
    public Material lvl3;
    public Material lvl4;
    public Material lvl5; 

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
        FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/Destruction", GetComponent<Transform>().position);
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
                FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/MoreHP", GetComponent<Transform>().position);
        }

        previousHealth = health;
    }

// Fonction pour déterminer le niveau en fonction de la santé
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
}