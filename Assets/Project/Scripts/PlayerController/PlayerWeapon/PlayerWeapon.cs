using UnityEngine;

/// <summary>
/// Gestiona el sistema de armas del jugador.
/// Cambia entre el Bate y la Escopeta con la tecla Q.
/// Delega el input de cada arma a su WeaponBehavior activo.
/// </summary>
public class PlayerWeapon : MonoBehaviour
{
    [Header("Weapon References")]
    [SerializeField] private BatWeapon bat;
    [SerializeField] private ShotgunWeapon shotgun;

    [Header("Switch Settings")]
    [SerializeField] private KeyCode switchKey = KeyCode.Q;

    private WeaponBehavior _currentWeapon;
    private WeaponBehavior _previousWeapon;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();

        // Validación
        if (bat == null || shotgun == null)
        {
            Debug.LogError("[PlayerWeapon] Asigna Bat y Shotgun en el Inspector.");
            return;
        }

        // Empezar con el bate equipado
        bat.OnEquip();
        shotgun.OnUnequip();
        _currentWeapon = bat;
        _previousWeapon = shotgun;

        UpdateAnimatorWeapon();
    }

    /// <summary>Llamado cada frame desde PlayerController.</summary>
    public void HandleWeaponInput()
    {
        if (_currentWeapon == null) return;

        // ── Cambio de arma ────────────────────────────────────────
        if (Input.GetKeyDown(switchKey))
            SwitchWeapon();

        // ── Delegar input al arma activa ──────────────────────────
        _currentWeapon.HandleInput();
    }

    private void SwitchWeapon()
    {
        _currentWeapon.OnUnequip();

        // Swap referencias
        (_currentWeapon, _previousWeapon) = (_previousWeapon, _currentWeapon);

        _currentWeapon.OnEquip();
        UpdateAnimatorWeapon();

        Debug.Log($"[PlayerWeapon] Arma activa: {_currentWeapon.WeaponName}");
    }

    private void UpdateAnimatorWeapon()
    {
        // Puedes usar un int/enum en el Animator: 0 = Bat, 1 = Shotgun
        bool hasShotgun = _currentWeapon is ShotgunWeapon;
        _animator?.SetBool("HasShotgun", hasShotgun);
    }

    // ── Getters ──────────────────────────────────────────────────
    public WeaponBehavior CurrentWeapon => _currentWeapon;
    public bool IsHoldingBat => _currentWeapon is BatWeapon;
    public bool IsHoldingShotgun => _currentWeapon is ShotgunWeapon;
}