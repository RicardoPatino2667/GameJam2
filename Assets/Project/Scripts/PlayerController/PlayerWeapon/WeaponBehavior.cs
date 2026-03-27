using UnityEngine;

/// <summary>
/// Clase base abstracta para todas las armas.
/// Cada arma concreta (Bat, Shotgun) hereda de esta clase
/// e implementa su propio comportamiento.
/// </summary>
public abstract class WeaponBehavior : MonoBehaviour
{
    [Header("Base Weapon Config")]
    [SerializeField] protected string weaponName = "Weapon";

    // Estado compartido
    protected bool IsAiming { get; private set; }
    protected bool IsAttacking { get; private set; }

    protected Animator PlayerAnimator;

    protected virtual void Awake()
    {
        // Busca el Animator en el padre (el Player)
        PlayerAnimator = GetComponentInParent<Animator>();
    }

    /// <summary>
    /// Llamado cada frame por PlayerWeapon mientras esta arma está activa.
    /// </summary>
    public void HandleInput()
    {
        bool rightHeld = Input.GetMouseButton(1);
        bool leftDown = Input.GetMouseButtonDown(0);

        // ── Aiming state ─────────────────────────────────
        if (rightHeld && !IsAiming)
        {
            IsAiming = true;
            OnStartAim();
        }
        else if (!rightHeld && IsAiming)
        {
            IsAiming = false;
            OnStopAim();
        }

        // ── Attack ────────────────────────────────────────
        if (leftDown)
        {
            if (IsAiming)
                OnAimAttack();   // Clic Der + Clic Izq
            else
                OnNormalAttack(); // Solo Clic Izq
        }
    }

    // ── Métodos a implementar por cada arma ──────────────────────

    /// <summary>Clic Izquierdo sin apuntar.</summary>
    protected abstract void OnNormalAttack();

    /// <summary>Clic Derecho sostenido – entrar en pose de apuntar.</summary>
    protected abstract void OnStartAim();

    /// <summary>Soltar Clic Derecho – salir de pose de apuntar.</summary>
    protected abstract void OnStopAim();

    /// <summary>Clic Derecho + Clic Izquierdo – ataque apuntando.</summary>
    protected abstract void OnAimAttack();

    /// <summary>
    /// Llamado cuando el arma se equipa.
    /// </summary>
    public virtual void OnEquip()
    {
        gameObject.SetActive(true);
        IsAiming = false;
        IsAttacking = false;
    }

    /// <summary>
    /// Llamado cuando el arma se desequipa.
    /// </summary>
    public virtual void OnUnequip()
    {
        IsAiming = false;
        IsAttacking = false;
        gameObject.SetActive(false);
    }

    public string WeaponName => weaponName;
}