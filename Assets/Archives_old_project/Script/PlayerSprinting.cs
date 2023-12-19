// Lien pour activer/désactiver le sprint avec un trigger https://youtu.be/tECethq4JQ0?list=PLdPQ93duD7PCkWpTo4ElrWlyx6S9bF18V&t=208

using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Player))]
public class PlayerSprinting : MonoBehaviour
{
    [SerializeField] float speedMultiplier = 2f;

    Player player;
    PlayerInput playerInput;
    InputAction sprintAction;

    void Awake()
    {
        player = GetComponent<Player>();
        playerInput = GetComponent<PlayerInput>();
        sprintAction = playerInput.actions["sprint"];
    }

    void OnEnable() => player.OnBeforeMove += OnBeforeMove;
    void OnDisable() => player.OnBeforeMove -= OnBeforeMove;

    void OnBeforeMove()
    {
        var sprintInput = sprintAction.ReadValue<float>();
        if (sprintInput == 0) return;
        var forwardMovementFactor = Mathf.Clamp01(
            Vector3.Dot(player.transform.forward, player.velocity.normalized)
        );
        var multiplier = Mathf.Lerp(1f, speedMultiplier, forwardMovementFactor);
        player.movementSpeedMultiplier *= multiplier;
    }
}
