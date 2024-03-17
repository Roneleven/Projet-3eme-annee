using UnityEngine;

public class MeteorWave : MonoBehaviour
{
    public float activationInterval = 1f; // Intervalle entre chaque activation d'enfant
    private Transform[] children; // Tableau pour stocker les enfants
    private int currentIndex = 0; // Index de l'enfant actuellement activé
    private float timer = 0f; // Timer pour suivre l'intervalle entre chaque activation

    void Start()
    {
        // Désactiver tous les enfants
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

        // Initialiser le tableau avec les enfants
        children = new Transform[transform.childCount];
        int index = 0;
        foreach (Transform child in transform)
        {
            children[index] = child;
            index++;
        }
    }

    void Update()
    {
        // Vérifier si tous les enfants ont été activés
        if (currentIndex >= children.Length)
        {
            return; // Sortir de la fonction si tous les enfants ont été activés
        }

        // Compteur pour suivre l'intervalle entre chaque activation
        timer += Time.deltaTime;

        // Vérifier si l'intervalle est écoulé
        if (timer >= activationInterval)
        {
            // Activer l'enfant suivant dans la liste
            children[currentIndex].gameObject.SetActive(true);
            currentIndex++;

            // Réinitialiser le compteur
            timer = 0f;
        }
    }
}