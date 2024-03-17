using UnityEngine;
using UnityEngine.Profiling;

public class CubeHealth : MonoBehaviour
{
    public int health = 1;
    private bool isDead = false;

    // Déclarez les modèles 3D pour chaque niveau ici
    public GameObject lvl1Model;
    public GameObject lvl2Model;
    public GameObject lvl3Model;
    public GameObject lvl4Model;
    public GameObject lvl5Model;

    private int currentLevel;
    private int previousLevel;

    private void Start()
    {
        // Assurez-vous que seul le modèle approprié pour le niveau initial est créé
        UpdateModel();
    }

    private void Update()
    {
        CheckLevelChange();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;

        if (health <= 0)
        {
            FMODUnity.RuntimeManager.PlayOneShot("event:/DestructibleBlock/Behaviours/Destruction", transform.position);
            Destroy(gameObject);
        }
        else
        {
            // Mettez à jour le modèle lorsque le niveau de santé change
            UpdateModel();
        }
    }

    private void UpdateModel()
    {
        // Détruire l'ancien GameObject
        Destroy(gameObject);

        // Instantiatez le nouveau modèle à la même position et rotation que l'ancien GameObject
        GameObject newModel = null;
        if (health <= 1)
        {
            newModel = Instantiate(lvl1Model, transform.position, transform.rotation);
        }
        else if (health <= 2)
        {
            newModel = Instantiate(lvl2Model, transform.position, transform.rotation);
        }
        else if (health <= 3)
        {
            newModel = Instantiate(lvl3Model, transform.position, transform.rotation);
        }
        else if (health <= 4)
        {
            newModel = Instantiate(lvl4Model, transform.position, transform.rotation);
        }
        else if (health <= 5)
        {
            newModel = Instantiate(lvl5Model, transform.position, transform.rotation);
        }

        // Assurez-vous que le nouveau modèle est un enfant du même parent que l'ancien GameObject
        newModel.transform.parent = transform.parent;
    }

    private void CheckLevelChange()
    {
        currentLevel = DetermineLevel(health);

        if (currentLevel != previousLevel)
        {
            // Niveau changé, donc mise à jour du modèle
            UpdateModel();

            // Stockez le niveau actuel comme niveau précédent pour la prochaine itération
            previousLevel = currentLevel;
        }
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
        else if (healthValue <= 3)
        {
            return 3;
        }
        else if (healthValue <= 4)
        {
            return 4;
        }
        else
        {
            return 5;
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
