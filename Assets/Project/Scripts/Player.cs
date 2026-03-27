using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSimple : MonoBehaviour
{
    public float velocidad = 5f;
    public float fuerzaSalto = 8f;
    public Transform puntoAtaque;
    public float rangoAtaque = 1.2f;
    public float tiempoEntreAtaques = 0.5f;

    private Rigidbody2D rb;
    private Vector2 movimientoInput;
    private float siguienteAtaque;
    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;
    private InputAction attackAction;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Falta Rigidbody2D en " + gameObject.name);

        playerInput = GetComponent<PlayerInput>();
        if (playerInput == null)
        {
            Debug.LogError("Falta PlayerInput en " + gameObject.name);
            return;
        }

        moveAction   = playerInput.actions["Move"];
        jumpAction   = playerInput.actions["Jump"];
        attackAction = playerInput.actions["Attack"];
    }

    void OnEnable()
    {
        if (jumpAction != null)   jumpAction.performed   += OnJump;
        if (attackAction != null) attackAction.performed += OnAttack;
    }

    void OnDisable()
    {
        if (jumpAction != null)   jumpAction.performed   -= OnJump;
        if (attackAction != null) attackAction.performed -= OnAttack;
        if (rb != null)           rb.linearVelocity = Vector2.zero;
    }

    void Update()
    {
        if (moveAction == null) return;

        Vector2 move = moveAction.ReadValue<Vector2>();
        movimientoInput = move;

        rb.linearVelocity = new Vector2(move.x * velocidad, rb.linearVelocity.y);

        if (move.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(move.x), 1, 1);
    }

    void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, fuerzaSalto);
        }
    }

    void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed && Time.time >= siguienteAtaque)
        {
            siguienteAtaque = Time.time + tiempoEntreAtaques;
            Atacar(2);
        }
    }

    void Atacar(int daño)
    {
        if (puntoAtaque == null)
        {
            Debug.LogWarning("puntoAtaque no asignado en el Inspector");
            return;
        }

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoAtaque);
        foreach (Collider2D e in enemigos)
        {
            if (e.CompareTag("Zombie"))
            {
                Zombie z = e.GetComponent<Zombie>();
                if (z != null) z.RecibirDaño(daño);
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
            Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
    }
}