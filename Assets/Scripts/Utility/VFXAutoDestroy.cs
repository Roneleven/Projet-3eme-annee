using UnityEngine;
using UnityEngine.VFX;
using System.Collections;

public class VFXAutoDestroy : MonoBehaviour
{
    private VisualEffect vfx;
    public float initialDelay = 0.2f; // Delay before starting to check for destruction
    public float checkInterval = 0.2f; // Interval between checks
    public float extraDelay = 0.2f; // Additional delay to ensure particles are completely finished

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
        // Wait for the initial delay to let the visual effect start playing
        yield return new WaitForSeconds(initialDelay);

        // Continuously check at the specified interval
        while (true)
        {
            // If no particles are alive, wait for the extra delay then destroy the object
            if (vfx.aliveParticleCount == 0)
            {
                // Wait for the extra delay to ensure all particles are finished
                yield return new WaitForSeconds(extraDelay);

                // Double-check if the particle count is still zero after the extra delay
                if (vfx.aliveParticleCount == 0)
                {
                    // Add a final check to ensure the VFX has truly finished
                    if (!vfx.HasAnyParticlesAlive())
                    {
                        Destroy(gameObject);
                        yield break;
                    }
                }
            }

            // Wait for the check interval before the next check
            yield return new WaitForSeconds(checkInterval);
        }
    }
}

public static class VisualEffectExtensions
{
    public static bool HasAnyParticlesAlive(this VisualEffect vfx)
    {
        // This function can be expanded if additional checks are needed
        return vfx.aliveParticleCount > 0;
    }
}