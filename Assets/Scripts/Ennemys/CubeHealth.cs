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

    private void Start()
    {
        cubeRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        UpdateMaterial(); 
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
        else if (health <= 2)
        {
            cubeRenderer.material = lvl2;
        }
        else if (health <= 5)
        {
            cubeRenderer.material = lvl3;
        }
         else if (health <= 10)
        {
            cubeRenderer.material = lvl4;
        }
         else if (health <= 25)
        {
            cubeRenderer.material = lvl5;
        }
    }
}