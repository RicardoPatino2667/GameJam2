using UnityEngine;

/// <summary>
/// Componente para los proyectiles (clavos y perdigones).
/// Aplica daño al zombie que impacte y se destruye a sí mismo.
/// </summary>
public class Projectile : MonoBehaviour
{
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 4f;
    [SerializeField] private string enemyTag = "Zombie";

    private void Start()
    {
        Destroy(gameObject, lifeTime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag(enemyTag)) return;

        if (other.TryGetComponent<ZombieHealth>(out var zombie))
            zombie.TakeDamage(damage);

        Destroy(gameObject);
    }

    /// <summary>Permite que las armas sobreescriban el daño en tiempo de ejecución.</summary>
    public void SetDamage(int value) => damage = value;
}

// ─────────────────────────────────────────────────────────────────────────────
/// <summary>
/// Stub de salud del zombie. Crea tu clase real o amplía esta.
/// </summary>
public class ZombieHealth : MonoBehaviour
{
    [SerializeField] private int maxHealth = 100;
    private int _currentHealth;

    private void Awake() => _currentHealth = maxHealth;

    public void TakeDamage(int amount)
    {
        if (amount <= 0) return;   // daño 0 no hace nada

        _currentHealth -= amount;
        Debug.Log($"[Zombie] {name} recibió {amount} de daño. HP: {_currentHealth}/{maxHealth}");

        if (_currentHealth <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"[Zombie] {name} murió.");
        // Aquí puedes disparar animación de muerte, soltar loot, etc.
        Destroy(gameObject);
    }
}