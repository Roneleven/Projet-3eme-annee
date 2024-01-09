using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    // Rotation originale de la caméra
    private Quaternion originalRotation = Quaternion.identity;

    private void Awake()
    {
        Instance = this;
    }

    private void OnShake(float duration, float strength)
    {
        // Sauvegarde de la rotation actuelle
        Quaternion currentRotation = transform.localRotation;

        // Secousse de la caméra
        transform.DOShakeRotation(duration, strength)
            .OnComplete(() => ResetRotation(currentRotation));
    }

    // Fonction pour réinitialiser la rotation de la caméra
    private void ResetRotation(Quaternion initialRotation)
    {
        // Animation pour la transition douce en utilisant la rotation d'origine
        transform.DOLocalRotateQuaternion(originalRotation, 0.5f);
    }

    public static void Shake(float duration, float strength) => Instance.OnShake(duration, strength);
}
