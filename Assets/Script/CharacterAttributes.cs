using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAttributes : MonoBehaviour
{
    public PlayerMovement playerMovement;
    public float maxSpeedIncrement = 10f; 
    public int maxIncrement = 3; 

    private int speedIncrement = 0; 
    private int gravityIncrement = 0; 

    private void Start()
    {
        playerMovement = GetComponent < PlayerMovement>();
        UpdateSpeed();
        UpdateGravity();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P) && speedIncrement < maxIncrement)
        {
            speedIncrement++;
            UpdateSpeed();
        }
        else if (Input.GetKeyDown(KeyCode.M) && speedIncrement > 0)
        {
            speedIncrement--;
            UpdateSpeed();
        }

        if (Input.GetKeyDown(KeyCode.O) && gravityIncrement < maxIncrement)
        {
            gravityIncrement++;
            UpdateGravity();
        }
        else if (Input.GetKeyDown(KeyCode.L) && gravityIncrement > 0)
        {
            gravityIncrement--;
            UpdateGravity();
        }
    }

    void UpdateSpeed()
    {
        playerMovement.speed = maxSpeedIncrement - speedIncrement * 2.0f; //-2 par incrément
    }

    void UpdateGravity()
    {
        playerMovement.gravity = baseGravity - gravityIncrement * 2.0f; //-2 par incrément
    }

    public float baseSpeed
    {
        get { return playerMovement.speed; }
        set { playerMovement.speed = value; }
    }

    public float baseGravity
    {
        get { return playerMovement.gravity; }
        set { playerMovement.gravity = value; }
    }
}