using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class HomingCube : MonoBehaviour
{
    public string targetTag = "Player"; 
    public float speed = 5f; 
    public Player playerScript;
    public float destroyDistance = 1.5f; // Distance à partir de laquelle les losanges se détruisent

    public VisualEffect explosionEffect;
    public Transform explosionEffectSpawnPoint;
    private VisualEffect explosionEffectInstance;

    private Transform target; 

    private static List<HomingCube> allCubes = new List<HomingCube>();

    void Awake()
    {
        allCubes.Add(this);
    }

    void OnDestroy()
    {
        allCubes.Remove(this);
    }

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
        for (int i = allCubes.Count - 1; i >= 0; i--)
        {
            HomingCube cube = allCubes[i];
            if (cube != this)
            {
                float distance = Vector3.Distance(transform.position, cube.transform.position);
                if (distance < destroyDistance)
                {
                    explosionEffectInstance = Instantiate(explosionEffect, explosionEffectSpawnPoint.position, explosionEffectSpawnPoint.rotation);
                    explosionEffectInstance.Play();
                    explosionEffectInstance.gameObject.AddComponent<VFXAutoDestroy>();

                    if (speed >= cube.speed)
                    {
                        speed += cube.speed;
                        speed /= 1.2f;
                        Destroy(cube.gameObject);
                    }
                    else
                    {
                        cube.speed += speed;
                        cube.speed /= 1.2f;
                        Destroy(gameObject);
                        return; // Return immediately to avoid further processing after destruction
                    }
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            playerScript.TakeDamage(1);
            Destroy(gameObject);
        }
    }
}