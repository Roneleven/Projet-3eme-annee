using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_Zone : MonoBehaviour
{
    //this script will be on the object that represents the zone, and when the player enters the zone, it will send a debug

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has entered the zone");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player has left the zone");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Debug.Log("Player is in the zone");
        }
    }


}
