using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VFXAutoDestroy : MonoBehaviour
{
    private VisualEffect vfx;
    public float initialDelay = 0.1f; // Délai avant de commencer à vérifier la destruction
    public float checkInterval = 0.1f; // Intervalle entre les vérifications
    public float extraDelay = 0.1f; // Délai supplémentaire pour s'assurer que les particules sont complètement terminées

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
        // Attendre un moment pour que l'effet visuel commence à jouer
        yield return new WaitForSeconds(initialDelay);

        // Vérifier à plusieurs reprises avec un intervalle
        while (true)
        {
            // Si les particules sont terminées, attendre un délai supplémentaire puis détruire l'objet
            if (vfx.aliveParticleCount == 0)
            {
                yield return new WaitForSeconds(extraDelay);
                Destroy(gameObject);
                yield break;
            }

            // Attendre un intervalle avant la prochaine vérification
            yield return new WaitForSeconds(checkInterval);
        }
    }
}