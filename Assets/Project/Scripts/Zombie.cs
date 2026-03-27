using UnityEngine;
using UnityEngine.InputSystem;

public class Zombie : MonoBehaviour
{
    [Header("Estadísticas")]
    public int vida = 54;
    public float velocidad = 2f;
    public float rangoAtaque = 1.5f;
    public float tiempoEntreAtaques = 1f;
    
    [Header("Detección")]
    public Transform puntoSuelo;
    public Transform puntoFrente;
    public float distanciaSuelo = 0.2f;
    public float distanciaFrente = 0.2f;
    public LayerMask capaSuelo;
    
    [Header("Referencias")]
    public Transform target; // Puedes asignar el jugador manualmente o se buscará automáticamente
    
    // Componentes
    Rigidbody2D rb;
    Animator animator;
    PlayerInput playerInput; // Referencia al PlayerInput del jugador
    
    // Variables de estado
    bool enSuelo;
    bool haySueloAdelante;
    bool hayPared;
    float siguienteAtaque;
    bool isAttacking;
    
    void Start()
    {
        // Obtener componentes
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        
        // Buscar al jugador si no está asignado manualmente
        if (target == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
            {
                target = playerObj.transform;
                playerInput = playerObj.GetComponent<PlayerInput>();
            }
            else
            {
                Debug.LogError("No hay player con tag 'Player'");
            }
        }
        else
        {
            playerInput = target.GetComponent<PlayerInput>();
        }
    }
    
    void Update()
    {
        if (target == null) return;
        
        DetectarEntorno();
        
        float distancia = Vector2.Distance(transform.position, target.position);
        
        if (distancia <= rangoAtaque)
            Atacar();
        else
            Perseguir();
        
        // Actualizar animaciones
        if (animator != null)
        {
            animator.SetFloat("Velocidad", Mathf.Abs(rb.linearVelocity.x));
            animator.SetBool("Atacando", isAttacking);
        }
    }
    
    void DetectarEntorno()
    {
        enSuelo = Physics2D.Raycast(puntoSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);
        haySueloAdelante = Physics2D.Raycast(puntoFrente.position, Vector2.down, distanciaSuelo, capaSuelo);
        hayPared = Physics2D.Raycast(puntoFrente.position, Vector2.right * transform.localScale.x, distanciaFrente, capaSuelo);
    }
    
    void Perseguir()
    {
        if (isAttacking) return;
        
        float direccion = Mathf.Sign(target.position.x - transform.position.x);
        
        // Girar sprite
        transform.localScale = new Vector3(direccion, 1, 1);
        
        // Evitar caerse o chocar
        if (!haySueloAdelante || hayPared)
        {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
            return;
        }
        
        // Movimiento SOLO en X
        rb.linearVelocity = new Vector2(direccion * velocidad, rb.linearVelocity.y);
    }
    
    void Atacar()
    {
        // Detener movimiento al atacar
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        
        if (Time.time >= siguienteAtaque && !isAttacking)
        {
            siguienteAtaque = Time.time + tiempoEntreAtaques;
            isAttacking = true;
            
            // Activar animación de ataque
            if (animator != null)
            {
                animator.SetTrigger("Atacar");
            }
            
            // Aplicar daño al jugador
            if (playerInput != null)
            {
                // Buscar el GameManager o aplicar daño directamente al jugador
                GameManager gm = FindFirstObjectByType<GameManager>();
                if (gm != null) 
                {
                    gm.PlayerRecibeDaño(1);
                }
                else
                {
                    // Alternativa: aplicar daño directamente al componente de vida del jugador
                    var playerHealth = target.GetComponent<PlayerHealth>();
                    if (playerHealth != null)
                        playerHealth.TakeDamage(1);
                }
            }
            
            // Resetear estado de ataque después de un tiempo
            Invoke(nameof(ResetAttack), 0.5f);
        }
    }
    
    void ResetAttack()
    {
        isAttacking = false;
        if (animator != null)
            animator.SetBool("Atacando", false);
    }
    
    public void RecibirDaño(int daño)
    {
        vida -= daño;
        
        // Activar animación de daño
        if (animator != null)
            animator.SetTrigger("RecibirDaño");
        
        if (vida <= 0) Morir();
    }
    
    void Morir()
    {
        // Activar animación de muerte
        if (animator != null)
            animator.SetTrigger("Morir");
        
        // Notificar al GameManager
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) 
            gm.ZombieMuere();
        
        // Destruir después de un pequeño delay para que se vea la animación
        Destroy(gameObject, 0.5f);
    }
    
    void OnDrawGizmos()
    {
        if (puntoSuelo != null)
            Gizmos.DrawLine(puntoSuelo.position, puntoSuelo.position + Vector3.down * distanciaSuelo);
        
        if (puntoFrente != null)
        {
            Gizmos.DrawLine(puntoFrente.position, puntoFrente.position + Vector3.down * distanciaSuelo);
            Gizmos.DrawLine(puntoFrente.position, puntoFrente.position + Vector3.right * transform.localScale.x * distanciaFrente);
        }
    }
}