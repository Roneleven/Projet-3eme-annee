using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour
{
    public Transform newParent; // Nouveau parent de la caméra après la transition
    public float transitionDuration = 3f; // Durée de la transition en secondes
    public float newFOV = 60f; // Nouveau champ de vision (FOV) de la caméra

    private Transform originalParent; // Parent original de la caméra
    private Camera mainCamera;
    private float originalFOV;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        originalParent = transform.parent;
        originalFOV = mainCamera.fieldOfView;
    }

    public void PerformTransition()
    {
        StartCoroutine(TransitionCoroutine());
    }

    IEnumerator TransitionCoroutine()
    {
        // Détacher la caméra de son parent
        transform.SetParent(null);
        Debug.Log("ok.");

        // Attendre x secondes
        yield return new WaitForSeconds(2f);

        // Graduellement changer le FOV
        float timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;
            mainCamera.fieldOfView = Mathf.Lerp(originalFOV, newFOV, t);
            yield return null;
        }

        // Réattacher la caméra au nouveau parent
        transform.SetParent(newParent);

        // Revenir au FOV initial
        timer = 0f;
        while (timer < transitionDuration)
        {
            timer += Time.deltaTime;
            float t = timer / transitionDuration;
            mainCamera.fieldOfView = Mathf.Lerp(newFOV, originalFOV, t);
            yield return null;
        }

        // Réattacher la caméra au parent d'origine
        transform.SetParent(originalParent);
        mainCamera.fieldOfView = originalFOV;
    }
}