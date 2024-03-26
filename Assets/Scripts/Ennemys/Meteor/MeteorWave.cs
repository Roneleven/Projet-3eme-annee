using UnityEngine;

public class MeteorWave : MonoBehaviour
{
    public float activationInterval = 1f;
    private Transform[] children;
    private int currentIndex = 0;
    private float timer = 0f;

    void Start()
    {
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(false);
        }

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
        if (currentIndex >= children.Length)
        {
            return;
        }

        timer += Time.deltaTime;

        if (timer >= activationInterval)
        {
            children[currentIndex].gameObject.SetActive(true);
            currentIndex++;

            timer = 0f;
        }
    }
}