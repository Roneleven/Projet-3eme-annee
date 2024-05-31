using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public TMP_Text text;
    public float fadeInDuration = 0.5f;
    public float fadeOutDuration = 0.5f;
    private bool isFading = false;

    private void Start()
    {
        // Au démarrage, réglons l'alpha du texte à 0 pour le rendre transparent
        SetTextAlpha(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Si le joueur entre dans le trigger, affichons le texte et lançons le fondu
            text.gameObject.SetActive(true);
            StartCoroutine(FadeIn());
        }
    }

    private IEnumerator FadeIn()
    {
        // Empêchons l'appel simultané de fonctions de fondu
        if (isFading)
            yield break;

        isFading = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeInDuration)
        {
            SetTextAlpha(Mathf.Lerp(0f, 1f, elapsedTime / fadeInDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetTextAlpha(1f);
        isFading = false;
    }

    private IEnumerator FadeOut()
    {
        // Empêchons l'appel simultané de fonctions de fondu
        if (isFading)
            yield break;

        isFading = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            SetTextAlpha(Mathf.Lerp(1f, 0f, elapsedTime / fadeOutDuration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        SetTextAlpha(0f);
        text.gameObject.SetActive(false); // Désactivons le texte après le fondu de sortie
        Destroy(gameObject); // Détruisons ce GameObject après le fondu de sortie
        isFading = false;
    }

    private void SetTextAlpha(float alpha)
    {
        // Réglons l'alpha du texte
        Color color = text.color;
        color.a = alpha;
        text.color = color;
    }

    private void Update()
    {
        Weapon weapon = FindObjectOfType<Weapon>();
        if (weapon != null && weapon._held)
        {
            // Si le joueur ramasse l'arme, lançons le fondu de sortie puis détruisons ce GameObject
            StartCoroutine(FadeOut());
        }
    }
}
