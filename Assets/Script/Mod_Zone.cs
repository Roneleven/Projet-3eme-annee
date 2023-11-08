using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mod_Zone : MonoBehaviour
{
    public GameObject player;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject == player)
        {
            // Modifie les propriétés du joueur
        }
    }


}
