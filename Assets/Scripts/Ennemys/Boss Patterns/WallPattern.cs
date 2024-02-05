using UnityEngine;
using System.Collections;

public class WallPattern : MonoBehaviour
{
    public float wallSpawnInterval;
    public GameObject wallPrefab;
    public float wallDistance;
    public float wallWidth;
    public float wallHeight;
    public float wallSpeed;
    public MouseLook mouseLookScript;

    private void Start()
    {
        StartCoroutine(SpawnWallPattern());
    }

    private IEnumerator SpawnWallPattern()
    {
        while (true)
        {
            yield return new WaitForSeconds(wallSpawnInterval);

            SpawnWall();
        }
    }

    private void SpawnWall()
    {
        Quaternion wallRotation = Quaternion.Euler(0f, mouseLookScript.transform.eulerAngles.y, 0f);
        Vector3 wallPosition = mouseLookScript.transform.position +
                               mouseLookScript.transform.forward * wallDistance;

        wallPosition.y = mouseLookScript.transform.position.y;

        GameObject wall = Instantiate(wallPrefab, wallPosition, wallRotation);
        wall.transform.localScale = new Vector3(wallWidth, wallHeight, 1f);

        StartCoroutine(MoveWall(wall.transform));
    }

    private IEnumerator MoveWall(Transform wallTransform)
    {
        float moveDuration = wallSpawnInterval;
        float elapsedTime = 0f;

        Vector3 initialPosition = wallTransform.position;
        Vector3 targetPosition = initialPosition - wallTransform.forward * wallDistance;

        while (elapsedTime < moveDuration)
        {
            float t = elapsedTime / moveDuration;
            wallTransform.position = Vector3.Lerp(initialPosition, targetPosition, t);

            elapsedTime += Time.deltaTime;

            yield return null;
        }

        wallTransform.position = targetPosition;
    }

    // Ajoutez cette méthode pour mettre à jour le pattern du mur
    public void UpdateWallPattern()
    {
        // Ajoutez le code nécessaire pour mettre à jour le pattern du mur ici
    }
}
