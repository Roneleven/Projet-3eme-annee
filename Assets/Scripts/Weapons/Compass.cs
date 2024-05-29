using UnityEngine;

public class Compass : MonoBehaviour
{
    public Transform target; // La cible actuelle vers laquelle la flèche doit pointer
    public Transform arrow; // La flèche qui doit pointer vers la cible
    public Transform player; // Le joueur ou la caméra du joueur

    void Update()
    {
        if (target == null) return; // Assurez-vous que la cible existe

        // Calculer la direction de la cible par rapport au joueur, en ignorant l'axe vertical (y)
        Vector3 directionToTarget = new Vector3(target.position.x - player.position.x, 0, target.position.z - player.position.z);

        // Calculer l'angle sur l'axe X entre la direction vers la cible et l'avant du joueur
        float angle = Vector3.SignedAngle(player.forward, directionToTarget, Vector3.up);

        // Appliquer l'angle à la flèche sur l'axe X en inversant si nécessaire
        float adjustedAngle = -angle;

        // Ajuster la rotation de la flèche en fonction de l'angle corrigé
        arrow.localRotation = Quaternion.Euler(adjustedAngle, 0, 0);
    }
}