using UnityEngine;
using UnityEngine.InputSystem;
public class TopdownMovement : MonoBehaviour
{
    public InputAction playerControls;

    private Rigidbody2D rb;
    [SerializeField]private float playerSpeed;
    [SerializeField]private GameObject mainCamera;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        Vector2 Input = playerControls.ReadValue<Vector2>();

        rb.velocity = Input * playerSpeed;


        mainCamera.transform.position = new Vector3(0f, transform.position.y/1.3f, -10f);

    }

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
}
