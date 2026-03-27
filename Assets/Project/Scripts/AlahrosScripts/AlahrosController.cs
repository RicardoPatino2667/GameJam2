using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public enum WeaponType
{
    Bat,
    Shotgun
}

public class AlahrosController : MonoBehaviour
{
    private PlayerInput characterInput;
    private Rigidbody2D characterRigidbody2D;
    private Animator characterAnimator;
    private Vector2 moveInput;
    private bool isDelayingNail = false;

    [Header("Movimiento")]
    public float speed = 1.0f;
    public float jumpForce = 10.0f;

    [Header("Estado")]
    [SerializeField] private float shootInput;
    public bool wasShooting;
    public bool isAttacking;
    public bool isJumping;
    public bool isGrounded = true;

    [Header("Arma Actual")]
    public WeaponType currentWeapon = WeaponType.Bat;

    [Header("Bate - Disparo de Clavos")]
    public GameObject clavoPrefab;
    public Transform puntoDisparo;
    public float velocidadClavo = 12f;
    public float nailSpawnDelay = 0.15f; // ← Nuevo: retraso ajustable

    [Header("Escopeta - Ataque Melee")]
    public Transform puntoAtaqueMelee;
    public float radioAtaqueMelee = 1.5f;
    public int dañoEscopetaMelee = 25;
    public LayerMask capaEnemigos;

    private Vector3 originalScale;

    void Start()
    {
        characterRigidbody2D = GetComponent<Rigidbody2D>();
        characterInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();
        originalScale = transform.localScale;
        isGrounded = true;

        if (characterAnimator == null)
            Debug.LogError("❌ Animator component not found!");
        if (characterInput == null)
            Debug.LogError("❌ PlayerInput component not found!");
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
        if (Input.GetKeyDown(KeyCode.Q))
        {
            currentWeapon = (currentWeapon == WeaponType.Bat) ? WeaponType.Shotgun : WeaponType.Bat;
            Debug.Log("=== 🔫 ARMA CAMBIADA A: " + currentWeapon + " ===");
        }
    }

    void Atacar()
    {
        // Leer inputs
        bool isAiming = false;
        bool attackPressed = false;

        try
        {
            isAiming = characterInput.actions["Aim"].IsPressed();
            attackPressed = characterInput.actions["Attack"].WasPressedThisFrame();
        }
        catch (System.Exception e)
        {
            Debug.LogError($"❌ Error con Input Actions: {e.Message}");
            return;
        }

        // Actualizar animación de apuntar según el arma actual
        if (currentWeapon == WeaponType.Bat)
        {
            characterAnimator.SetBool("isAimingBat", isAiming);
            characterAnimator.SetBool("isAimingShotgun", false);
        }
        else // Shotgun
        {
            characterAnimator.SetBool("isAimingShotgun", isAiming);
            characterAnimator.SetBool("isAimingBat", false);
        }

        // Detectar ataque
        if (attackPressed && !wasShooting)
        {
            isAttacking = true;
            Debug.Log($"🎯 ATAQUE! Arma: {currentWeapon}, Apuntando: {isAiming}");

            if (currentWeapon == WeaponType.Bat)
            {
                if (isAiming)
                {
                    // BATE + APUNTAR → Dispara clavo con delay
                    Debug.Log("🔨 BATE: Disparando clavo!");
                    characterAnimator.SetTrigger("shootBat");
                    if (!isDelayingNail)
                        StartCoroutine(DelayedNailSpawn());
                }
                else
                {
                    // BATE + MELEE → Solo animación, sin daño
                    Debug.Log("👊 BATE: Golpe melee (SIN daño)");
                    characterAnimator.SetTrigger("hitBat");
                }
            }
            else if (currentWeapon == WeaponType.Shotgun)
            {
                if (isAiming)
                {
                    // ESCOPETA + APUNTAR → Solo animación, sin daño
                    Debug.Log("🔫 ESCOPETA: Disparo a distancia (SIN daño)");
                    characterAnimator.SetTrigger("shootShotgun");
                }
                else
                {
                    // ESCOPETA + MELEE → Con daño
                    Debug.Log("💥 ESCOPETA: Golpe melee CON DAÑO!");
                    characterAnimator.SetTrigger("hitShotgun");
                    AtacarMeleeEscopeta();
                }
            }
        }

        wasShooting = attackPressed;
    }

    IEnumerator DelayedNailSpawn()
    {
        isDelayingNail = true;
        yield return new WaitForSeconds(nailSpawnDelay);
        DispararClavo();
        isDelayingNail = false;
    }

    void DispararClavo()
    {
        if (clavoPrefab == null)
        {
            Debug.LogError("❌ clavoPrefab NO asignado en el Inspector!");
            return;
        }

        if (puntoDisparo == null)
        {
            Debug.LogError("❌ puntoDisparo NO asignado en el Inspector!");
            return;
        }

        float direccionX = Mathf.Sign(transform.localScale.x);
        Vector2 posicionDisparo = puntoDisparo.position;

        Debug.Log($"📌 Disparando clavo desde {posicionDisparo}, dirección X: {direccionX}");

        GameObject clavo = Instantiate(clavoPrefab, posicionDisparo, Quaternion.identity);

        Clavo clavoScript = clavo.GetComponent<Clavo>();
        if (clavoScript != null)
        {
            clavoScript.Inicializar(new Vector2(direccionX, 0), velocidadClavo);
            Debug.Log("✅ Clavo creado exitosamente!");
        }
        else
        {
            Debug.LogError("❌ El prefab del Clavo no tiene el script Clavo.cs");
        }
    }

    void AtacarMeleeEscopeta()
    {
        if (puntoAtaqueMelee == null)
        {
            Debug.LogError("❌ puntoAtaqueMelee NO asignado en el Inspector!");
            return;
        }

        Debug.Log($"⚔️ Realizando ataque melee en {puntoAtaqueMelee.position} con radio {radioAtaqueMelee}");

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(
            puntoAtaqueMelee.position,
            radioAtaqueMelee,
            capaEnemigos
        );

        Debug.Log($"🎯 Enemigos detectados en rango: {enemigos.Length}");

        if (enemigos.Length == 0)
        {
            Debug.Log("❌ No hay enemigos en rango");
            return;
        }

        foreach (Collider2D col in enemigos)
        {
            Debug.Log($"💢 Impacto con: {col.name}");

            Zombie zombie = col.GetComponent<Zombie>();
            if (zombie != null)
            {
                zombie.RecibirDaño(dañoEscopetaMelee);
                Debug.Log($"💀 Daño aplicado a {col.name}: {dañoEscopetaMelee} HP");
            }
            else
            {
                Debug.Log($"⚠️ El objeto {col.name} no tiene componente Zombie");
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
            Debug.Log("🦘 Saltando!");
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
            Gizmos.DrawWireSphere(puntoDisparo.position, 0.1f);
        }
    }
}