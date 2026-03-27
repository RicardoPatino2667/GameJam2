using UnityEngine;

public class Clavo : MonoBehaviour
{
    private Vector2 direccion;
    private float velocidad;
    public int daño = 10;
    public float tiempoVida = 3f;

    public void Inicializar(Vector2 dir, float vel)
    {
        direccion = dir.normalized;
        velocidad = vel;
        
        // Rotar el sprite según dirección
        if (dir.x < 0)
            transform.localScale = new Vector3(-1, 1, 1);

        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Dañar zombie
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.RecibirDaño(daño);
            Destroy(gameObject);
            return;
        }

        // Destruir al tocar suelo o paredes
        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}