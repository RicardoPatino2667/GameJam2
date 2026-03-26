using UnityEngine;

public class Zombie : MonoBehaviour
{
    public int vida = 54;
    public float velocidad = 2f;
    public float rangoAtaque = 1.5f;
    
    Transform player;
    Rigidbody2D rb;
    float siguienteAtaque;
    
    void Start()
    {
        // Buscar al player por TAG
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
        {
            player = playerObj.transform;
            Debug.Log("Zombie encontró al player");
        }
        else
        {
            Debug.LogError("ERROR: No hay player con tag 'Player' en la escena");
        }
        
        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            Debug.LogError("ERROR: El zombie no tiene Rigidbody2D");
        }
    }
    
    void Update()
    {
        // Verificar que player existe antes de usarlo
        if (player == null) 
        {
            Debug.LogWarning("Zombie no tiene referencia al player");
            return;
        }
        
        float distancia = Vector2.Distance(transform.position, player.position);
        
        if (distancia <= rangoAtaque)
            Atacar();
        else
            Perseguir();
    }
    
    void Perseguir()
    {
        Vector2 direccion = (player.position - transform.position).normalized;
        rb.linearVelocity = direccion * velocidad;
        
        // Rotar
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
    }
    
    void Atacar()
    {
        rb.linearVelocity = Vector2.zero;
        
        if (Time.time >= siguienteAtaque)
        {
            siguienteAtaque = Time.time + 1f;
            
            // Buscar GameManager
            GameManager gm = FindFirstObjectByType<GameManager>();
            if (gm != null)
            {
                gm.PlayerRecibeDaño(1);
                Debug.Log("Zombie atacó");
            }
            else
            {
                Debug.LogError("ERROR: No se encuentra el GameManager");
            }
        }
    }
    
    public void RecibirDaño(int daño)
    {
        vida -= daño;
        Debug.Log("Zombie vida: " + vida);
        if (vida <= 0) Morir();
    }
    
    void Morir()
    {
        GameManager gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.ZombieMuere();
        
        Destroy(gameObject);
    }
}