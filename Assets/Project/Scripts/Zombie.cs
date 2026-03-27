using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int vida = 54;
    public float velocidad = 2f;
    public float rangoAtaque = 1.5f;

    Transform player;
    Rigidbody2D rb;
    float siguienteAtaque;
    Animator zombieAnimator;


    void Start()
    {
        zombieAnimator = GetComponent<Animator>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
        else
            Debug.LogError("No hay player con tag 'Player' en la escena");

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("El zombie no tiene Rigidbody2D");
    }

    void Update()
    {
        if (player == null) return;

        float distancia = Vector2.Distance(transform.position, player.position);
        Debug.Log("Distancia al player: " + distancia); // debug temporal

        if (distancia <= rangoAtaque)
            Atacar();
        else
            Perseguir();
    }

    void Perseguir()
    {
        Vector2 direccion = (player.position - transform.position).normalized;

        // Solo X, respeta la gravedad en Y
        rb.linearVelocity = new Vector2(direccion.x * velocidad, rb.linearVelocity.y);

        // Voltear sprite según dirección
        if (direccion.x != 0)
            transform.localScale = new Vector3(Mathf.Sign(direccion.x), 1, 1);
    }

    void Atacar()
    {
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);

        if (Time.time >= siguienteAtaque)
        {
            siguienteAtaque = Time.time + 1f;

            // ← AGREGAR ESTA LÍNEA
            if (zombieAnimator != null) zombieAnimator.SetTrigger("attack");

            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
                gm.PlayerRecibeDaño(1);
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
}