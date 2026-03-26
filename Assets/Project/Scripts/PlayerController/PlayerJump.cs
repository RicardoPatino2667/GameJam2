using UnityEngine;

/// <summary>
/// Maneja el salto del jugador con Espacio.
/// Soporta Coyote Time y Jump Buffer para mejor game feel.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerJump : MonoBehaviour
{
    [Header("Jump Settings")]
    [SerializeField] private float jumpForce = 14f;
    [SerializeField] private float fallMultiplier = 2.5f;   // caída más rápida
    [SerializeField] private float lowJumpMultiplier = 2f;  // salto corto al soltar espacio

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius = 0.15f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Coyote & Buffer")]
    [SerializeField] private float coyoteTime = 0.12f;
    [SerializeField] private float jumpBufferTime = 0.1f;

    private Rigidbody2D _rb;
    private Animator _animator;

    private bool _isGrounded;
    private float _coyoteTimeCounter;
    private float _jumpBufferCounter;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>Llamado cada frame desde PlayerController.</summary>
    public void HandleJump()
    {
        // ── Ground check ──────────────────────────────────────────
        _isGrounded = Physics2D.OverlapCircle(groundCheck.position,
                                              groundCheckRadius,
                                              groundLayer);

        _animator?.SetBool("IsGrounded", _isGrounded);

        // ── Coyote time ───────────────────────────────────────────
        if (_isGrounded)
            _coyoteTimeCounter = coyoteTime;
        else
            _coyoteTimeCounter -= Time.deltaTime;

        // ── Jump buffer ───────────────────────────────────────────
        if (Input.GetButtonDown("Jump"))            // Spacebar
            _jumpBufferCounter = jumpBufferTime;
        else
            _jumpBufferCounter -= Time.deltaTime;

        // ── Execute jump ──────────────────────────────────────────
        if (_jumpBufferCounter > 0f && _coyoteTimeCounter > 0f)
        {
            _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, jumpForce);
            _jumpBufferCounter = 0f;
            _coyoteTimeCounter = 0f;
            _animator?.SetTrigger("Jump");
        }

        // ── Better fall physics ───────────────────────────────────
        if (_rb.linearVelocity.y < 0f)
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                                  * (fallMultiplier - 1f) * Time.deltaTime;
        }
        else if (_rb.linearVelocity.y > 0f && !Input.GetButton("Jump"))
        {
            _rb.linearVelocity += Vector2.up * Physics2D.gravity.y
                                  * (lowJumpMultiplier - 1f) * Time.deltaTime;
        }
    }

    public bool IsGrounded => _isGrounded;

    private void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}