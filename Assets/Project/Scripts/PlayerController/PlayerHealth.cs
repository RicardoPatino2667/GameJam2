using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Sistema de salud del jugador.
/// </summary>
public class PlayerHealth : MonoBehaviour
{
    [Header("Health Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private float invincibleTime = 1f;   // segundos de invulnerabilidad tras daño

    [Header("Events")]
    public UnityEvent<int> OnHealthChanged;   // (currentHealth)
    public UnityEvent OnDeath;

    private int _currentHealth;
    private float _invincibleTimer;
    private bool _isDead;

    private Animator _animator;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _currentHealth = maxHealth;
    }

    private void Update()
    {
        if (_invincibleTimer > 0f)
            _invincibleTimer -= Time.deltaTime;
    }

    public void TakeDamage(int amount)
    {
        if (_isDead || _invincibleTimer > 0f) return;

        _currentHealth = Mathf.Max(0, _currentHealth - amount);
        _invincibleTimer = invincibleTime;

        OnHealthChanged?.Invoke(_currentHealth);
        _animator?.SetTrigger("Hurt");

        if (_currentHealth <= 0)
            Die();
    }

    public void Heal(int amount)
    {
        if (_isDead) return;
        _currentHealth = Mathf.Min(maxHealth, _currentHealth + amount);
        OnHealthChanged?.Invoke(_currentHealth);
    }

    private void Die()
    {
        _isDead = true;
        _animator?.SetTrigger("Die");
        OnDeath?.Invoke();
        // Aquí puedes desactivar el gameObject, cargar Game Over screen, etc.
    }

    // ── Getters ─────────────────────────────────────────────────
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => maxHealth;
    public bool IsDead => _isDead;
}