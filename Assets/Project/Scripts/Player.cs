using UnityEngine;
using UnityEngine.InputSystem;  // Importante!

public class Player : MonoBehaviour
{
    public float velocidad = 5f;
    public Transform puntoAtaque;
    public float rangoAtaque = 1.5f;
    
    Rigidbody2D rb;
    Vector2 movimiento;
    
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    
    void Update()
    {
        // Movimiento con el nuevo Input System
        float x = Keyboard.current.aKey.isPressed ? -1 : (Keyboard.current.dKey.isPressed ? 1 : 0);
        float y = Keyboard.current.sKey.isPressed ? -1 : (Keyboard.current.wKey.isPressed ? 1 : 0);
        movimiento = new Vector2(x, y).normalized;
        
        // Rotar hacia el mouse con el nuevo Input System
        Vector3 mouse = Mouse.current.position.ReadValue();
        mouse = Camera.main.ScreenToWorldPoint(mouse);
        Vector2 direccion = new Vector2(mouse.x - transform.position.x, mouse.y - transform.position.y);
        float angulo = Mathf.Atan2(direccion.y, direccion.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angulo, Vector3.forward);
        
        // Atacar
        if (Mouse.current.leftButton.wasPressedThisFrame) Atacar(2);
        if (Mouse.current.rightButton.wasPressedThisFrame) Atacar(1);
    }
    
    void FixedUpdate()
    {
        rb.linearVelocity = movimiento * velocidad;
    }
    
    void Atacar(int daño)
    {
        Collider2D[] zombies = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoAtaque);
        
        foreach (Collider2D z in zombies)
        {
            if (z.CompareTag("Zombie"))
            {
                z.GetComponent<Zombie>().RecibirDaño(daño);
                Debug.Log("Atacó con daño: " + daño);
            }
        }
    }
    
    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
            Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
    }
}