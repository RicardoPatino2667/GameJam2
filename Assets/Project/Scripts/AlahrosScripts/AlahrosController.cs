using UnityEngine;
using UnityEngine.InputSystem;

public class AlahrosController : MonoBehaviour
{
    PlayerInput characterInput;
    Rigidbody2D characterRigidbody2D;
    Animator characterAnimator;
    Vector2 moveInput;

    [Header("Movimiento")]
    public float speed = 1.0f;
    public float jumpForce = 10.0f;

    [Header("Estado")]
    [SerializeField] float shootInput;
    public bool wasShooting, isAttacking, isJumping;
    public bool isGrounded = true;

    [Header("Arma Actual")]
    public enum WeaponType { Bat, Shotgun }
    public WeaponType currentWeapon = WeaponType.Bat;

    [Header("Bate - Disparo de Clavos")]
    public GameObject clavoPrefab;
    public Transform puntoDisparo;
    public float velocidadClavo = 12f;

    [Header("Escopeta - Ataque Melee")]
    public Transform puntoAtaqueMelee;
    public float radioAtaqueMelee = 1.5f;
    public int dañoEscopetaMelee = 25;
    public LayerMask capaEnemigos;

    Vector3 originalScale;

    void Start()
    {
        characterRigidbody2D = GetComponent<Rigidbody2D>();
        characterInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();
        originalScale = transform.localScale;
        isGrounded = true;
    }

    void Update()
    {
        Mover();
        CambiarArma();
        Atacar();
        Saltar();
    }

    void Mover()
    {
        moveInput = characterInput.actions["Move"].ReadValue<Vector2>();
        characterAnimator.SetFloat("moveX", moveInput.x);

        if (moveInput.x > 0)
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        else if (moveInput.x < 0)
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);

        transform.Translate(new Vector2(moveInput.x, moveInput.y) * speed * Time.deltaTime);
    }

    void CambiarArma()
    {
        // Presiona Q para cambiar entre Bate y Escopeta
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeapon = (currentWeapon == WeaponType.Bat) ? WeaponType.Shotgun : WeaponType.Bat;
            Debug.Log("Arma equipada: " + currentWeapon);
        }
    }

    void Atacar()
    {
        bool isAiming = characterInput.actions["Aim"].IsPressed();
        characterAnimator.SetBool("isAiming", isAiming);

        shootInput = characterInput.actions["Attack"].ReadValue<float>();

        if (shootInput > 0.1f && !wasShooting)
        {
            isAttacking = true;

            if (currentWeapon == WeaponType.Bat)
            {
                if (isAiming)
                {
                    // BATE + APUNTAR → Dispara clavos ✓
                    characterAnimator.SetTrigger("shootBat");
                    DispararClavo();
                }
                else
                {
                    // BATE + MELEE → Sin daño
                    characterAnimator.SetTrigger("hitBat");
                    Debug.Log("Golpe de bate - sin daño");
                }
            }
            else if (currentWeapon == WeaponType.Shotgun)
            {
                if (isAiming)
                {
                    // ESCOPETA + DISPARAR → Sin daño
                    characterAnimator.SetTrigger("shootShotgun");
                    Debug.Log("Disparo de escopeta - sin daño");
                }
                else
                {
                    // ESCOPETA + MELEE → Con daño ✓
                    characterAnimator.SetTrigger("hitShotgun");
                    AtacarMeleeEscopeta();
                }
            }
        }

        wasShooting = shootInput > 0.1f;
    }

    void DispararClavo()
    {
        if (clavoPrefab == null || puntoDisparo == null)
        {
            Debug.LogError("Asigna clavoPrefab y puntoDisparo en el Inspector");
            return;
        }

        float direccionX = Mathf.Sign(transform.localScale.x);
        GameObject clavo = Instantiate(clavoPrefab, puntoDisparo.position, Quaternion.identity);
        
        Clavo clavoScript = clavo.GetComponent<Clavo>();
        if (clavoScript != null)
            clavoScript.Inicializar(new Vector2(direccionX, 0), velocidadClavo);
        else
            Debug.LogError("El prefab del Clavo no tiene el script Clavo.cs");
    }

    void AtacarMeleeEscopeta()
    {
        if (puntoAtaqueMelee == null)
        {
            Debug.LogError("Asigna puntoAtaqueMelee en el Inspector");
            return;
        }

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            puntoAtaqueMelee.position,
            radioAtaqueMelee,
            capaEnemigos
        );

        if (enemigos.Length == 0)
        {
            Debug.Log("Escopeta melee - no hay enemigos en rango");
            return;
        }

        foreach (Collider2D col in enemigos)
        {
            Zombie zombie = col.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.RecibirDaño(dañoEscopetaMelee);
                Debug.Log("Escopeta melee golpeó: " + col.name);
            }
        }
    }

    void Saltar()
    {
        if (characterInput.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            characterRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
            characterAnimator.SetBool("isJumping", true);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            characterAnimator.SetBool("isJumping", false);
        }
    }

    // Visualizar radio de ataque en el Editor
    void OnDrawGizmosSelected()
    {
        if (puntoAtaqueMelee != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(puntoAtaqueMelee.position, radioAtaqueMelee);
        }
        if (puntoDisparo != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(puntoDisparo.position, 0.1f);
        }
    }
}