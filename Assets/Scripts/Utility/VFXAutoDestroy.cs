using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VFXAutoDestroy : MonoBehaviour
{
    private VisualEffect vfx;
    public float delay = 0.1f; // Délai avant de vérifier la destruction

    private void Awake()
    {
        vfx = GetComponent<VisualEffect>();
    }

    private void OnEnable()
    {
        StartCoroutine(CheckVFXAlive());
    }

    private IEnumerator CheckVFXAlive()
    {
        yield return new WaitForSeconds(delay);
        while (vfx.aliveParticleCount > 0)
        {
            yield return null;
        }

        Destroy(gameObject);
    }
}