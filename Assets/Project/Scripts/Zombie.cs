using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int vida = 54;
    public float velocidad = 2f;
    public float rangoAtaque = 1.5f;

    [Header("Detección")]
    public Transform puntoSuelo;
    public Transform puntoFrente;
    public float distanciaSuelo = 0.2f;
    public float distanciaFrente = 0.2f;
    public LayerMask capaSuelo;

    Transform player;
    Rigidbody2D rb;
    float siguienteAtaque;

    bool enSuelo;
    bool haySueloAdelante;
    bool hayPared;

    void Start()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null) player = playerObj.transform;
        else Debug.LogError("No hay player con tag 'Player'");

        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (player == null) return;

        DetectarEntorno();

        float distancia = Vector2.Distance(transform.position, player.position);

        if (distancia <= rangoAtaque)
            Atacar();
        else
            Perseguir();
    }

    void DetectarEntorno()
    {
        enSuelo = Physics2D.Raycast(puntoSuelo.position, Vector2.down, distanciaSuelo, capaSuelo);
        haySueloAdelante = Physics2D.Raycast(puntoFrente.position, Vector2.down, distanciaSuelo, capaSuelo);
        hayPared = Physics2D.Raycast(puntoFrente.position, Vector2.right * transform.localScale.x, distanciaFrente, capaSuelo);
    }

    void Perseguir()
    {
        float direccion = Mathf.Sign(player.position.x - transform.position.x);

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
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (Time.time >= siguienteAtaque)
        {
            siguienteAtaque = Time.time + 1f;
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null) gm.PlayerRecibeDaño(1);
        }
    }

    public void RecibirDaño(int daño)
    {
        vida -= daño;
        if (vida <= 0) Morir();
    }

    void Morir()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.ZombieMuere();
        Destroy(gameObject);
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