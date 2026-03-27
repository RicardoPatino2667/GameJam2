using UnityEngine;
using UnityEngine.InputSystem;


public class AlahrosController : MonoBehaviour
{
    PlayerInput characterInput;
    Rigidbody characterRigidbody;
    Animator characterAnimator;
    Vector2 moveInput;
    public float speed = 1.0f;  
    void Start()
    {
        characterRigidbody = GetComponent<Rigidbody>();
        characterInput = GetComponent<PlayerInput>();
        characterAnimator = GetComponent<Animator>();   
    }

    // Update is called once per frame
    void Update()
    {
        moveInput = characterInput.actions["Move"].ReadValue<Vector2>();
        Vector2 movement = new Vector2 (moveInput.x, moveInput.y);
        characterAnimator.SetFloat("moveX", moveInput.x);
        Debug.Log("moveInput.X");
        transform.Translate (movement * speed * Time.deltaTime);

    }
}
