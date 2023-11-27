using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerShooting : MonoBehaviour
{
    public GameObject bulletPrefab;
    public Transform firePoint;
    public float fireRate = 1f;

    private Player player;
    private PlayerInput playerInput;
    private InputAction shootAction;
    private float nextFireTime = 0f;

    void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        shootAction = playerInput.actions["shoot"];
    }

    void Update()
    {
        if (Time.time > nextFireTime)
        {
            // Vérifiez si l'action de tir est en cours
            if (shootAction.ReadValue<float>() > 0)
            {
                Fire();
                nextFireTime = Time.time + 1/fireRate;
            }
        }
    }

    void Fire()
    {
        Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
    }
}