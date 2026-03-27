using Unity.VisualScripting;
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
    [SerializeField] float shootInput, jumpInput;
    public bool wasShooting;
    public bool isGrounded = true;

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
        transform.Translate(movement * speed * Time.deltaTime);

        shootInput = characterInput.actions["Attack"].ReadValue<float>();

        if (shootInput > 0.1 && !wasShooting)
        {
            characterAnimator.SetTrigger("shootShotgun");
        }
        wasShooting = shootInput > 0.1;

        jumpInput = characterInput.actions["Jump"].ReadValue<float>();

        if (characterInput.actions["Jump"].WasPressedThisFrame() && isGrounded)
        {
            characterRigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
}