using UnityEngine;

/// <summary>
/// Coordinador principal del Player.
/// Referencia todos los módulos y los inicializa.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(PlayerJump))]
[RequireComponent(typeof(PlayerHealth))]
[RequireComponent(typeof(PlayerWeapon))]
public class PlayerController : MonoBehaviour
{
    [Header("Modules (auto-assigned if empty)")]
    public PlayerMovement Movement;
    public PlayerJump Jump;
    public PlayerHealth Health;
    public PlayerWeapon Weapon;

    private void Awake()
    {
        Movement = Movement != null ? Movement : GetComponent<PlayerMovement>();
        Jump = Jump != null ? Jump : GetComponent<PlayerJump>();
        Health = Health != null ? Health : GetComponent<PlayerHealth>();
        Weapon = Weapon != null ? Weapon : GetComponent<PlayerWeapon>();
    }

    private void Update()
    {
        Movement.HandleMovement();
        Jump.HandleJump();
        Weapon.HandleWeaponInput();
    }
}