using UnityEngine;
using DG.Tweening;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance;

    private Quaternion originalRotation = Quaternion.identity;

    private void Awake()
    {
        Instance = this;
    }

    private void OnShake(float duration, float strength)
    {
        Quaternion currentRotation = transform.localRotation;

        float noiseX = Mathf.PerlinNoise(Time.time, 0) * 2 - 1;
        float noiseY = Mathf.PerlinNoise(0, Time.time) * 2 - 1;

        Vector3 noiseVector = new Vector3(noiseX, noiseY, 0) * strength;

        transform.DOShakeRotation(duration, noiseVector)
            .OnComplete(() => ResetRotation(currentRotation));
    }

    private void ResetRotation(Quaternion initialRotation)
    {
        transform.DOLocalRotateQuaternion(originalRotation, 0.5f);
    }

    public static void Shake(float duration, float strength) => Instance.OnShake(duration, strength);
}
