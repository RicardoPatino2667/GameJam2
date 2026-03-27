using UnityEngine;

public class Clavo : MonoBehaviour
{
    private Vector2 direccion;
    private float velocidad;
    public int daño = 10;
    public float tiempoVida = 3f;
    private Vector3 escalaOriginal;

    void Awake()
    {
        // Guardar la escala original del prefab al despertar
        escalaOriginal = transform.localScale;
    }

    public void Inicializar(Vector2 dir, float vel)
    {
        direccion = dir.normalized;
        velocidad = vel;

        // Voltear el sprite horizontalmente según la dirección
        if (dir.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);
        else
            transform.localScale = new Vector3(Mathf.Abs(escalaOriginal.x), escalaOriginal.y, escalaOriginal.z);

        Destroy(gameObject, tiempoVida);
    }

    void Update()
    {
        transform.Translate(direccion * velocidad * Time.deltaTime, Space.World);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Zombie zombie = other.GetComponent<Zombie>();
        if (zombie != null)
        {
            zombie.RecibirDaño(daño);
            Destroy(gameObject);
            return;
        }

        if (other.CompareTag("Ground") || other.CompareTag("Wall"))
        {
            Destroy(gameObject);
        }
    }
}