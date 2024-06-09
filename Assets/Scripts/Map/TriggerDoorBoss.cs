using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerDoorBoss : MonoBehaviour
{
    [SerializeField] private List<Animator> doorAnimators = new List<Animator>();
    [SerializeField] private bool isOpen = false;
    public GameObject[] unlinkedBoxSpawnersToRemove;
    private Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            foreach (Animator animator in doorAnimators)
            {
                if (animator != null)
                {
                    if (isOpen)
                    {
                        animator.Play("Bossdoor1Open");
                        animator.Play("Bossdoor2Open");
                        player.IncreaseLoomParameter();
                    }
                    else
                    {
                        animator.Play("Bossdoor1Close");
                        animator.Play("Bossdoor2Close");
                        animator.Play("BlockWall");
                    }
                }
                else
                {
                    //Debug.LogWarning("One of the door animators is not assigned.");
                }
            }

            // Set this GameObject inactive after the door animations start
            gameObject.SetActive(false);

            // Destroy the unlinked box spawners
            if (unlinkedBoxSpawnersToRemove != null)
            {
                foreach (GameObject boxSpawner in unlinkedBoxSpawnersToRemove)
                {
                    if (boxSpawner != null)
                    {
                        Destroy(boxSpawner);
                    }
                    else
                    {
                        //Debug.LogWarning("One of the unlinked box spawners is not assigned.");
                    }
                }
            }
            else
            {
                //Debug.LogWarning("unlinkedBoxSpawnersToRemove array is not assigned.");
            }
        }
    }
}
