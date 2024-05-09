using UnityEngine;

public class BigWall : MonoBehaviour
{
    public float interval = 0.1f;
    public float moveSpeed = 1f;

    private Transform[] positions;
    private bool allCubesSpawned = false;
    private bool isSpawning = true;

    private FMOD.Studio.EventInstance wall;

    private void Start()
    {
        positions = new Transform[transform.childCount];
        for (int i = 0; i < transform.childCount; i++)
        {
            positions[i] = transform.GetChild(i);
            positions[i].gameObject.SetActive(false);
        }

        InvokeRepeating("SpawnCube", interval, interval);

        wall = FMODUnity.RuntimeManager.CreateInstance("event:/Heart/Patterns/BigWall_Start");
        wall.setParameterByName("ToGoing", 0.0F);
        wall.start();
    }

    private void Update()
    {
        wall.set3DAttributes(FMODUnity.RuntimeUtils.To3DAttributes(gameObject));

        if (allCubesSpawned && !GetComponent<WallPatternMovement>())
        {
            gameObject.AddComponent<WallPatternMovement>().Initialize(moveSpeed);
            wall.setParameterByName("ToGoing", 1.0F);
            Invoke("DestroyWallPattern", 10f);
        }
    }

    private void DestroyWallPattern()
    {
        wall.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        Destroy(gameObject);
    }

    private void SpawnCube()
    {
        if (!isSpawning)
            return;

        for (int i = 0; i < positions.Length; i++)
        {
            if (!positions[i].gameObject.activeSelf)
            {
                positions[i].gameObject.SetActive(true);
                return;
            }
        }

        isSpawning = false;
        CancelInvoke("SpawnCube");
        allCubesSpawned = true;
    }

    // Add this method to handle cube destruction
    public void CubeDestroyed()
    {
        // If cubes are still spawning, stop spawning process
        if (isSpawning)
        {
            isSpawning = false;
            CancelInvoke("SpawnCube");
        }
    }
}

public class WallPatternMovement : MonoBehaviour
{
    private float speed;

    public void Initialize(float moveSpeed)
    {
        speed = moveSpeed;
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}