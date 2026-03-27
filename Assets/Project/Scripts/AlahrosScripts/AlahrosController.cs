using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;


public class AlahrosController : MonoBehaviour
{
    PlayerInput characterInput;
    Rigidbody2D characterRigidbody2D;
    Animator characterAnimator;
    Vector2 moveInput;
    public float speed = 1.0f;
    public float jumpForce = 10.0f;
    [SerializeField] float shootInput, jumpInput, aimInput;
    public bool wasShooting, isAttacking, isJumping;
    public bool isGrounded = true;

    public enum WeaponType
    {
        Bat,
        Shotgun
    }

    public WeaponType currentWeapon = WeaponType.Bat;


    //public bool shootInput;
    Vector3 originalScale;
    void Start()
    {
        characterRigidbody2D = GetComponent<Rigidbody2D>();
        characterInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();
        originalScale = transform.localScale;
        isGrounded = true;
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = characterInput.actions["Move"].ReadValue<Vector2>();
        Vector2 movement = new Vector2(moveInput.x, moveInput.y);
        Debug.Log("moveInput.X" + moveInput.x);
        characterAnimator.SetFloat("moveX", moveInput.x);
        if (moveInput.x > 0)
        {
            transform.localScale = new Vector3(Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        else if (moveInput.x < 0)
        {
            transform.localScale = new Vector3(-Mathf.Abs(originalScale.x), originalScale.y, originalScale.z);
        }
        //if (!isAttacking)
        //{
            transform.Translate(movement * speed * Time.deltaTime);
        //}
        bool isAiming = characterInput.actions["Aim"].IsPressed();
        characterAnimator.SetBool("isAimingBat", isAiming && currentWeapon == WeaponType.Bat);
        characterAnimator.SetBool("isAimingShotgun", isAiming && currentWeapon == WeaponType.Shotgun);
        Debug.Log("Aiming: " + characterInput.actions["Aim"].IsPressed());
        shootInput = characterInput.actions["Attack"].ReadValue<float>();

        if (Keyboard.current.qKey.wasPressedThisFrame)
        {
            currentWeapon = currentWeapon == WeaponType.Bat
                ? WeaponType.Shotgun
                : WeaponType.Bat;

            Debug.Log("Switched to: " + currentWeapon);
        }

        if (shootInput > 0.1 && !wasShooting)
        {
            isAttacking = true;

            switch (currentWeapon)
            {
                case WeaponType.Bat:

                    if (isAiming)
                        characterAnimator.SetTrigger("shootBat"); // maybe throw?
                    else
                        characterAnimator.SetTrigger("hitBat");

                    break;

                case WeaponType.Shotgun:

                    if (isAiming)
                        characterAnimator.SetTrigger("shootShotgun");
                    else
                        characterAnimator.SetTrigger("hitShotgun"); // or block melee

                    break;
            }
        }
        //else
        //{
        //characterRigidbody2D.linearVelocity = new Vector2(0, characterRigidbody2D.linearVelocity.x);
        //}
        wasShooting = shootInput > 0.1;

        jumpInput = characterInput.actions["Jump"].ReadValue<float>();

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
}