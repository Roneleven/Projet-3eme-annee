using UnityEngine;

public class CubeHealth : MonoBehaviour
{
    public int health = 5;
    public Material lvl1;
    public Material lvl2; 
    public Material lvl3;
    public Material lvl4;
    public Material lvl5; 

    private Renderer renderer; 

    private void Start()
    {
        renderer = GetComponent<Renderer>();
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
        Destroy(gameObject);
    }
}

    private void UpdateMaterial()
    {
        
        if (health <= 5)
        {
            renderer.material = lvl1;
        }
        else if (health <= 10)
        {
            renderer.material = lvl2;
        }
        else if (health <= 20)
        {
            renderer.material = lvl3;
        }
         else if (health <= 30)
        {
            renderer.material = lvl4;
        }
         else if (health <= 50)
        {
            renderer.material = lvl5;
        }
    }
}