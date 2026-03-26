using UnityEngine;

/// <summary>
/// ESCOPETA
/// ─────────────────────────────────────────────────────────────
/// Clic Izq            → Golpe cuerpo a cuerpo (MUCHO daño)
/// Clic Der            → Pose de apuntar
/// Clic Der + Clic Izq → Dispara (sin daño, no mata zombies – arma "rota")
/// </summary>
public class ShotgunWeapon : WeaponBehavior
{
    [Header("Melee Settings")]
    [SerializeField] private int meleeDamage = 80;
    [SerializeField] private float meleeRange = 1.2f;
    [SerializeField] private float meleeCooldown = 0.7f;
    [SerializeField] private LayerMask enemyLayer;

    [Header("Shotgun Shot Settings")]
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float bulletSpeed = 22f;
    [SerializeField] private float shootCooldown = 1.2f;
    [SerializeField] private int pelletCount = 5;
    [SerializeField] private float spreadAngle = 15f;  // grados de dispersión

    private float _meleeCooldownTimer;
    private float _shootCooldownTimer;

    private void Update()
    {
        if (_meleeCooldownTimer > 0f) _meleeCooldownTimer -= Time.deltaTime;
        if (_shootCooldownTimer > 0f) _shootCooldownTimer -= Time.deltaTime;
    }

    // ── Clic Izquierdo sin apuntar – golpe CaC ──────────────────
    protected override void OnNormalAttack()
    {
        if (_meleeCooldownTimer > 0f) return;
        _meleeCooldownTimer = meleeCooldown;

        PlayerAnimator?.SetTrigger("ShotgunMelee");

        // Detectar enemigos en rango
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,
                                                        meleeRange,
                                                        enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<ZombieHealth>(out var zombie))
            {
                zombie.TakeDamage(meleeDamage);
                Debug.Log($"[Shotgun] Golpe CaC a {hit.name} – Daño: {meleeDamage}");
            }
        }
    }

    // ── Clic Derecho ────────────────────────────────────────────
    protected override void OnStartAim()
    {
        PlayerAnimator?.SetBool("ShotgunAiming", true);
        Debug.Log("[Shotgun] Adoptando pose de apuntar");
    }

    protected override void OnStopAim()
    {
        PlayerAnimator?.SetBool("ShotgunAiming", false);
        Debug.Log("[Shotgun] Saliendo de pose de apuntar");
    }

    // ── Clic Derecho + Clic Izquierdo – disparo (sin daño) ──────
    protected override void OnAimAttack()
    {
        if (_shootCooldownTimer > 0f) return;
        _shootCooldownTimer = shootCooldown;

        Fire();
    }

    private void Fire()
    {
        if (bulletPrefab == null || firePoint == null)
        {
            Debug.LogWarning("[Shotgun] Asigna bulletPrefab y firePoint en el Inspector.");
            return;
        }

        float baseAngle = transform.root.localScale.x > 0 ? 0f : 180f;

        for (int i = 0; i < pelletCount; i++)
        {
            float spread = Random.Range(-spreadAngle / 2f, spreadAngle / 2f);
            float angle = baseAngle + spread;

            Quaternion rot = Quaternion.Euler(0f, 0f, angle);
            Vector2 direction = rot * Vector2.right;

            GameObject pellet = Instantiate(bulletPrefab,
                                            firePoint.position,
                                            Quaternion.identity);

            if (pellet.TryGetComponent<Rigidbody2D>(out var rb))
                rb.linearVelocity = direction * bulletSpeed;

            // Daño = 0: la escopeta NO mata zombies al disparar
            if (pellet.TryGetComponent<Projectile>(out var proj))
                proj.SetDamage(0);
        }

        PlayerAnimator?.SetTrigger("ShotgunFire");
        Debug.Log("[Shotgun] ¡BOOM! (sin daño – arma rota)");
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}