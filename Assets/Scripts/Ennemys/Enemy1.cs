using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy1 : MonoBehaviour
{
    public float speed = 5.0f;
    public GameObject bulletPrefab; 
    public GameObject firePoint;
    private Transform target;

public float chaseSpeed = 5.0f;
public float orbitSpeed = 2.0f;
public float amplitude = 0.01f;
private float timeSinceLastDirectionChange = 0.0f;
private float timeUntilNextDirectionChange = 0.0f;
private int orbitDirectionMultiplier = 1;

public enum EnemyState
{
    ChillMode,
    ChasingMode,
    QuestionMode
}

private EnemyState currentState;
private GameObject currentBlock;

void Start()
{
    target = GameObject.FindGameObjectWithTag("Player").transform;
    StartCoroutine(ShootEveryTwoSeconds());
    timeUntilNextDirectionChange = Random.Range(1.0f, 10.0f);
    currentState = EnemyState.ChillMode;
}

void Update()
{
    Vector3 direction;
    float currentSpeed;
    
    if (Vector3.Distance(transform.position, target.position) > 30)
    {
        // Switch to ChillMode
        currentState = EnemyState.ChillMode;
    }
    else
    {
        // Switch to ChasingMode
        currentState = EnemyState.ChasingMode;
    }

    switch (currentState)
    {
        case EnemyState.ChillMode:
            // Stop moving and rotating
            currentSpeed = 0;
            break;
        case EnemyState.ChasingMode:
            // Rotate towards the player
            direction = (target.position - transform.position).normalized;
            Quaternion rotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, target.position) > 10)
            {
                // Move towards the player
                currentSpeed = chaseSpeed;
                transform.position += direction * currentSpeed * Time.deltaTime;
            }
            else
            {
                // Orbit around the player
                currentSpeed = orbitSpeed;
                Vector3 orbitDirection = Vector3.Cross(direction, Vector3.up) * orbitDirectionMultiplier;
                transform.position += orbitDirection * currentSpeed * Time.deltaTime;

                // Add sinusoidal movement
                transform.position = new Vector3(transform.position.x, transform.position.y + Mathf.Sin(Time.time) * amplitude, transform.position.z);
            }
            break;
        case EnemyState.QuestionMode:
            // Move towards the current block
            direction = (currentBlock.transform.position - transform.position).normalized;
            currentSpeed = chaseSpeed;
            transform.position += direction * currentSpeed * Time.deltaTime;
            break;
    
    }

   
    timeSinceLastDirectionChange += Time.deltaTime;
    if (timeSinceLastDirectionChange >= timeUntilNextDirectionChange)
    {
        orbitDirectionMultiplier *= -1;
        timeSinceLastDirectionChange = 0.0f;
        timeUntilNextDirectionChange = Random.Range(1.0f, 10.0f); // Set the time until the next direction change
    }

        GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
    foreach (GameObject block in blocks)
    {
        if (Vector3.Distance(transform.position, block.transform.position) <= 10)
        {
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            break;
        }
        else
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
    }
}

IEnumerator ShootEveryTwoSeconds()
{
    while (true)
    {
        if (currentState == EnemyState.ChasingMode)
        {
            GameObject[] blocks = GameObject.FindGameObjectsWithTag("Block");
            float waitTime = 2.0f;
            foreach (GameObject block in blocks)
            {
                if (Vector3.Distance(transform.position, block.transform.position) <= 10)
                {
                    waitTime = 1.0f;
                    break;
                }
            }
            yield return new WaitForSeconds(waitTime);
            Shoot();
        }
        else
        {
            yield return null; // Wait for the next frame
        }
    }
}

    void Shoot()
    {
        GameObject bullet = Instantiate(bulletPrefab, firePoint.transform.position, Quaternion.identity);
        Vector3 direction = (target.position - transform.position).normalized;
        bullet.GetComponent<BulletEnnemy1>().Initialize(direction);
    }

    public void SetCurrentBlock(GameObject block)
{
    currentBlock = block;
    currentState = EnemyState.QuestionMode;
}
}