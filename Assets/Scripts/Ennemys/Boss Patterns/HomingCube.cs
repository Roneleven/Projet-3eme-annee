using UnityEngine;

public class HomingCube : MonoBehaviour
{
    public string targetTag = "Player"; 
    public float speed = 5f; 
    public Player playerScript;
    public float destroyDistance = 1.5f; // Distance à partir de laquelle les losanges se détruisent

    private Transform target; 

    void Start()
    {
        playerScript = FindObjectOfType<Player>();
        GameObject targetObject = GameObject.FindGameObjectWithTag(targetTag);
        if (targetObject != null)
        {
            target = targetObject.transform;
        }
        else
        {
            Debug.LogError("Aucune cible avec le tag '" + targetTag + "' trouvée.");
        }
    }

    void Update()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            Quaternion rotationToTarget = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotationToTarget, Time.deltaTime * speed);

            transform.position += transform.forward * Time.deltaTime * speed;

            CheckDestroyAndMerge();
        }
    }

    private void CheckDestroyAndMerge()
    {
        HomingCube[] allCubes = FindObjectsOfType<HomingCube>(); // Récupérer tous les losanges dans la scène
        foreach (HomingCube cube in allCubes)
        {
            if (cube != this) // Éviter de comparer le même cube
            {
                float distance = Vector3.Distance(transform.position, cube.transform.position);
                if (distance < destroyDistance)
                {
                    if (speed >= cube.speed)
                    {
                        speed += cube.speed; // Ajouter la vitesse du cube détruit à la vitesse du cube restant
                        Destroy(cube.gameObject);
                    }
                    else
                    {
                        cube.speed += speed; // Ajouter la vitesse du cube restant à la vitesse du cube détruit
                        Destroy(gameObject);
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.TakeDamage(10);
            Destroy(gameObject);
        }
    }
}