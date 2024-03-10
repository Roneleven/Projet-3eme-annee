using UnityEngine;

public class ExplosivePillarPattern : MonoBehaviour
{
    public GameObject emptyPrefab; // Le prefab de l'objet vide à créer
    public float spawnRadius = 10f; // Le rayon autour du joueur où l'objet vide sera créé
    private string playerTag = "Player"; // Le tag pour identifier le joueur
    public float spawnInterval = 7f; // L'intervalle de temps entre chaque création d'objet vide

    private GameObject playerObject; // La référence au joueur

    void Start()
    {
        InvokeRepeating("SpawnEmptyObject", 0f, spawnInterval);
        playerObject = GameObject.FindGameObjectWithTag(playerTag);
    }

    void SpawnEmptyObject()
    {
        if (playerObject == null)
        {
            Debug.LogError("Player not found with tag: " + playerTag);
            return;
        }

        // Générer un point aléatoire dans le rayon autour du joueur sur l'axe horizontal
        Vector2 randomPoint2D = Random.insideUnitCircle * spawnRadius;
        Vector3 randomPoint = new Vector3(randomPoint2D.x, 0f, randomPoint2D.y) + playerObject.transform.position; // Convertir en Vector3 et ajouter la position du joueur

        // Ajouter une variation aléatoire sur l'axe Z
        randomPoint.z += Random.Range(-spawnRadius, spawnRadius);

        // Vérifier si aucun objet "Ground" n'est trouvé dans le rayon spécifié autour du joueur
        Collider[] colliders = Physics.OverlapSphere(randomPoint, 20f, LayerMask.GetMask("Ground"));
        if (colliders.Length == 0)
        {
            Debug.Log("No ground found near player.");
            return;
        }

        // Trouver la position de la surface du sol en dessous du point aléatoire
        RaycastHit hit;
        if (Physics.Raycast(randomPoint, Vector3.down, out hit, Mathf.Infinity, LayerMask.GetMask("Ground")))
        {
            randomPoint = hit.point;
        }
        else
        {
            Debug.LogError("Failed to find ground surface.");
            return;
        }

        // Créer l'objet vide à cet emplacement
        GameObject emptyObject = Instantiate(emptyPrefab, randomPoint, Quaternion.identity);

        // Orienter l'objet vide pour regarder vers le joueur sur l'axe horizontal
        Vector3 direction = playerObject.transform.position - randomPoint;
        direction.y = 0f; // Ignorer la composante y pour l'orientation
        emptyObject.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}