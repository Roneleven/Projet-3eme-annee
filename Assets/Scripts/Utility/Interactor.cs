using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class Interactor : MonoBehaviour
{
    public TMP_Text text;

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            text.gameObject.SetActive(true);
        }
    }
    private void Update()
    {
        Weapon weapon = FindObjectOfType<Weapon>();
        if (weapon._held) {
            text.gameObject.SetActive(false);
            Destroy(gameObject);
        }
    }
    
    


}
