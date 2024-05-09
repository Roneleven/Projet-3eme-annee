using UnityEngine;
using System.Collections;

public class AerialMinesPattern : MonoBehaviour
{
    public GameObject cubePrefab;
    public int numberOfCubes;
    public float radius;
    public float journeyDuration = 40.0f;

    public void LaunchAerialPattern()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            Transform playerTransform = player.transform;

            for (int i = 0; i < numberOfCubes; i++)
            {
                FMODUnity.RuntimeManager.PlayOneShot("event:/Heart/Patterns/AerialMine_Start");
                Vector3 randomPosition = playerTransform.position + Random.insideUnitSphere * radius;
                GameObject newCube = Instantiate(cubePrefab, transform.position, Quaternion.identity);
                StartCoroutine(MoveToRandomPosition(newCube.transform, randomPosition));
            }
        }
        else
        {
            Debug.LogWarning("Player not found. Make sure you have a GameObject tagged 'Player' in your scene.");
        }
    }

    IEnumerator MoveToRandomPosition(Transform cubeTransform, Vector3 targetPosition)
{
    float startTime = Time.time;
    Vector3 startPosition = cubeTransform.position;
    float journeyLength = Vector3.Distance(startPosition, targetPosition);

    // Tant que le transform du cube existe et qu'il n'a pas atteint la position cible
    while (cubeTransform != null && cubeTransform.position != targetPosition)
    {
        float distanceCovered = (Time.time - startTime) * journeyDuration;
        float fractionOfJourney = distanceCovered / journeyLength;

        // Vérifier à chaque itération si le cube a été détruit
        if (cubeTransform == null)
        {
            yield break; // Sortir de la coroutine si le cube a été détruit
        }

        cubeTransform.position = Vector3.Lerp(startPosition, targetPosition, fractionOfJourney);
        yield return null;
    }
}
}