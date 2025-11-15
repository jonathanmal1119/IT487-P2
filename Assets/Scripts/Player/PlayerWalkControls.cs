using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerWalkControls : MonoBehaviour
{
    [Header("Basic Movement Stuff")]

    public PlayerLookControls playerLook;
    public CharacterController controller;
    InputAction walkAction, jumpAction, sprintAction;

    public float walkSpeed = 5f;
    public float runSpeed = 7.5f;
    public float jumpVelocity = 5f;

    Vector2 walkVector;

    //x and z are left, right, forward, and back. y is for vertical movement (jumping and gravity)
    public Vector3 moveVector;

    [Header("Grounding and Gravity")]

    public Transform groundCheckPosition;
    public float groundCheckRadius = 0.45f;
    public LayerMask groundLayers;
    public float gravity = 10f;
    public float groundHoldVelocity = 10f;

    bool grounded;

    [Header("Sprint and Stamina")]

    public float stamina = 1f;
    public float runStaminaPerSecond = 0.15f;
    public float staminaRegenPerSecond = 0.1f;
    public bool waitForRefill = false;

    //public Slider staminaBar;
    //public Text staminaText;

    //public int DEBUGFRAMERATE = 60;

    private void Awake()
    {
        //I don't know how else to access the project-wide input actions. This is my punishment for not learning the new Input System sooner.
        walkAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");
        sprintAction = InputSystem.actions.FindAction("Player/Sprint");
    }

    private void OnEnable()
    {
        walkAction.Enable();
        jumpAction.Enable();
        sprintAction.Enable();

        //if (staminaBar != null)
        //{
        //    staminaBar.minValue = 0;
        //    staminaBar.maxValue = 1;
        //}
    }
    /*
     * ########## Because I am using the project-wide actions, disabling them here will also disable them everywhere. I don't want to do that. ##########
     * 
    private void OnDisable()
    {
        playerWalkControls.Disable();
        playerJumpControls.Disable();
    }
    */

    void Update()
    {
        //Application.targetFrameRate = DEBUGFRAMERATE;

        //WASD controls. The input system's axes are between -1 and 1, so I can multiply it by a desired value to set the player's speed.

        if ((!waitForRefill || stamina >= 1) && sprintAction.IsPressed())
        {
            waitForRefill = false;
            walkVector = walkAction.ReadValue<Vector2>() * runSpeed;
            stamina -= runStaminaPerSecond * Time.deltaTime;
            if(stamina < 0)
            {
                stamina = 0;
                waitForRefill = true;
            }
        }
        else if(sprintAction.IsPressed() == false)
        {
            waitForRefill = false;
            stamina += staminaRegenPerSecond * Time.deltaTime;
            if(stamina > 1f) { stamina = 1f; }

            walkVector = walkAction.ReadValue<Vector2>() * walkSpeed;
        }
        else
        {
            stamina += staminaRegenPerSecond * Time.deltaTime;
            if (stamina > 1f) { stamina = 1f; }

            walkVector = walkAction.ReadValue<Vector2>() * walkSpeed;
        }
        
        moveVector.x = walkVector.x;
        moveVector.z = walkVector.y;

        //jumping and gravity stuff

        grounded = Physics.CheckSphere(groundCheckPosition.position, groundCheckRadius, groundLayers);

        if (grounded)
        {
            //stop falling when hitting the ground. Uses groundHoldVelocity instead of just 0 so we can fine tune how floaty the player feels when walking off platforms.
            if(controller.velocity.y <= 0)
            {
                moveVector.y = groundHoldVelocity * -1f;
            }

            if (jumpAction.triggered)
            {
                //Debug.Log("Player jumped");
                moveVector.y = jumpVelocity;
            }
        }
        else
        {
            moveVector.y -= gravity * Time.deltaTime;
        }

        //Finally, moving the charactercontroller.
        controller.Move((playerLook.horizontalOrientation.right * moveVector.x * Time.deltaTime) + (playerLook.horizontalOrientation.forward * moveVector.z * Time.deltaTime) + (playerLook.horizontalOrientation.up * moveVector.y * Time.deltaTime));

        //Stamina UI stuff
        //if(staminaBar != null)
        //{
        //    staminaBar.value = stamina;
        //}
        //if(staminaText != null)
        //{
        //    staminaText.text = "Stamina: " + stamina.ToString();
        //}
    }
}
