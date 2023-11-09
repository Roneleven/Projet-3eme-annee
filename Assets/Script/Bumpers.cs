using UnityEngine;

public class Bumpers : MonoBehaviour
{
    // La direction de la propulsion
    public Vector3 propulsionDirection = Vector3.up;

    // La force de la propulsion
    public float propulsionForce = 100f;

    // Le CharacterController du personnage
    public CharacterController characterController;

    // Initialisation ordughdtiughver du start
    private void Start()
    {
        // R�cup�ration du CharacterController du personnage
        characterController = GetComponent<CharacterController>();
    }

    // �v�nement OnControllerColliderHit
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        // Si l'�l�ment est en collision avec le CharacterController du personnage
        if (hit.gameObject.GetComponent<CharacterController>() == characterController)
        {
            // Applique la force de propulsion au CharacterController
            characterController.Move(propulsionDirection * propulsionForce * Time.deltaTime);
        }
    }
}