using UnityEngine;

/// <summary>
/// Maneja el movimiento horizontal del jugador con A / D.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 7f;

    private Rigidbody2D _rb;
    private Animator _animator;
    private float _horizontalInput;
    private bool _isFacingRight = true;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
    }

    /// <summary>Llamado cada frame desde PlayerController.</summary>
    public void HandleMovement()
    {
        _horizontalInput = Input.GetAxisRaw("Horizontal"); // A = -1 | D = 1
        

        _rb.linearVelocity = new Vector2(_horizontalInput * moveSpeed, _rb.linearVelocity.y);

        FlipSprite();

        _animator?.SetFloat("Speed", Mathf.Abs(_horizontalInput));
    }

    private void FlipSprite()
    {
        if ((_isFacingRight && _horizontalInput < 0f) ||
            (!_isFacingRight && _horizontalInput > 0f))
        {
            _isFacingRight = !_isFacingRight;
            Vector3 scale = transform.localScale;
            scale.x *= -1f;
            transform.localScale = scale;
        }
    }

    public bool IsFacingRight => _isFacingRight;
}