using UnityEngine;
using UnityEngine.InputSystem;
public class TopdownMovement : MonoBehaviour
{
    public InputAction playerControls;
    public InputAction playerInteract;
    public InputAction playerClick;

    private Rigidbody2D rb;
    [SerializeField]private float playerSpeed;
    [SerializeField]private GameObject mainCamera;
    [SerializeField]private GameObject touchCheck;
    [SerializeField]private float touchRadius;
    [SerializeField]private LayerMask touchLayer;
    private Collider2D touching;
    private bool canInteract;
    private bool doorOpen;
    private bool doorClosed;
    private bool bedOpen;
    private bool bedClosed;

     
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        isTouchingObject();
        OpenUI();
        playerMovement();
    }
    private void isTouchingObject()
    {
        Vector2 checkPos = new Vector2(touchCheck.transform.position.x, touchCheck.transform.position.y);
        touching = Physics2D.OverlapCircle(checkPos, touchRadius, touchLayer);
    }  
    private void OpenUI()
    {
        if(playerInteract.ReadValue<float>() > 0 && canInteract == true)
        {
            canInteract = false;
            if(bedOpen || doorOpen)
            {
                FindObjectOfType<Overlays>().CloseBed();
                FindObjectOfType<Overlays>().CloseDoor();
            } else if(touching.name == "Door"){
                FindObjectOfType<Overlays>().OpenDoor();
            } else if(touching.name == "Bed"){
                FindObjectOfType<Overlays>().OpenBed();
            }
        }
        if(playerInteract.ReadValue<float>() == 0)
        {
            canInteract = true;
        }
    }
    private void playerMovement()
    {
        Vector2 Input = playerControls.ReadValue<Vector2>();

        rb.velocity = Input * playerSpeed;


        mainCamera.transform.position = new Vector3(0f, transform.position.y/1.3f, -10f);
    }
    private void OnEnable()
    {
        playerControls.Enable();
        playerInteract.Enable();
        playerClick.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
        playerInteract.Disable();
        playerClick.Disable();
    }
}
