using UnityEngine;

/// <summary>
/// BATE
/// ─────────────────────────────────────────────────────────────
/// Clic Izq            → Animación de golpe (sin daño)
/// Clic Der            → Pose de apuntar
/// Clic Der + Clic Izq → Dispara clavos (mata zombies)
/// </summary>
public class BatWeapon : WeaponBehavior
{
    [Header("Bat Settings")]
    [SerializeField] private float meleeCooldown = 0.5f;

    [Header("Nail Projectile")]
    [SerializeField] private GameObject nailPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float nailSpeed = 18f;
    [SerializeField] private int nailDamage = 40;
    [SerializeField] private float fireRate = 0.3f;   // clavos por segundo

    private float _meleeCooldownTimer;
    private float _fireRateTimer;

    private void Update()
    {
        if (_meleeCooldownTimer > 0f) _meleeCooldownTimer -= Time.deltaTime;
        if (_fireRateTimer > 0f) _fireRateTimer -= Time.deltaTime;
    }

    // ── Clic Izquierdo sin apuntar ──────────────────────────────
    protected override void OnNormalAttack()
    {
        if (_meleeCooldownTimer > 0f) return;

        _meleeCooldownTimer = meleeCooldown;
        PlayerAnimator?.SetTrigger("BatSwing");

        // El bate NO hace daño en golpe normal.
        // (El daño real ocurre en OnAimAttack con clavos)
        Debug.Log("[Bat] ¡Swing! (sin daño)");
    }

    // ── Clic Derecho ────────────────────────────────────────────
    protected override void OnStartAim()
    {
        PlayerAnimator?.SetBool("BatAiming", true);
        Debug.Log("[Bat] Adoptando pose de apuntar");
    }

    protected override void OnStopAim()
    {
        PlayerAnimator?.SetBool("BatAiming", false);
        Debug.Log("[Bat] Saliendo de pose de apuntar");
    }

    // ── Clic Derecho + Clic Izquierdo ───────────────────────────
    protected override void OnAimAttack()
    {
        ShootNail();
    }

    /// <summary>
    /// Se puede llamar en Update para disparo continuo mientras se mantiene
    /// el botón izquierdo mientras se apunta. También respeta el fireRate.
    /// </summary>
    private void ShootNail()
    {
        if (_fireRateTimer > 0f) return;
        if (nailPrefab == null || firePoint == null)
        {
            Debug.LogWarning("[Bat] Asigna nailPrefab y firePoint en el Inspector.");
            return;
        }

        _fireRateTimer = 1f / fireRate;

        // Dirección: hacia donde mira el player
        float dir = transform.root.localScale.x > 0 ? 1f : -1f;
        Vector2 direction = new Vector2(dir, 0f);

        GameObject nail = Instantiate(nailPrefab,
                                      firePoint.position,
                                      Quaternion.identity);

        if (nail.TryGetComponent<Rigidbody2D>(out var rb))
            rb.linearVelocity = direction * nailSpeed;

        // Asigna daño al proyectil si tiene el componente Projectile
        if (nail.TryGetComponent<Projectile>(out var proj))
            proj.SetDamage(nailDamage);

        PlayerAnimator?.SetTrigger("BatShootNail");
        Debug.Log($"[Bat] ¡Clavo disparado! Daño: {nailDamage}");
    }
}