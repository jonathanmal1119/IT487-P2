using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookControls : MonoBehaviour
{
    public Transform horizontalOrientation;
    public Transform verticalPivot;

    InputAction lookAction;

    public float horizontalLookSpeed = 1f;
    public float verticalLookSpeed = 1f;

    Vector2 rotChange;
    float Vrot, Hrot;

    public Vector2 verticalLookClamp;

    private void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Player/Look");
    }

    private void OnEnable()
    {
        lookAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //Just a small glimpse into my sick, twisted world
    private void Update()
    {
        rotChange = lookAction.ReadValue<Vector2>();

        /*
        horizontalOrientation.Rotate(0f, rotChange.x * horizontalLookSpeed * Time.deltaTime, 0f);
        //verticalPivot.Rotate(rotChange.y * verticalLookSpeed * Time.deltaTime * -1f, 0f, 0f);
        Vrot = verticalPivot.localEulerAngles.x + (rotChange.y * verticalLookSpeed * Time.deltaTime * -1f);
        //Debug.Log(Vrot);
        //Vrot = Mathf.Clamp(Vrot, -90f, 90f);
        if (Vrot > 90f && Vrot < 180f) { Vrot = 90f; }
        if (Vrot < 270f && Vrot > 180f) { Vrot = 270f; }
        verticalPivot.localEulerAngles = new Vector3(Vrot, verticalPivot.localEulerAngles.y, verticalPivot.localEulerAngles.z);
        */

        Hrot += horizontalLookSpeed * rotChange.x;
        Vrot -= verticalLookSpeed * rotChange.y;

        Vrot = Mathf.Clamp(Vrot, verticalLookClamp.x, verticalLookClamp.y);

        horizontalOrientation.eulerAngles = new Vector3(0.0f, Hrot, 0.0f);
        verticalPivot.localEulerAngles = new Vector3(Vrot, 0.0f, 0.0f);
        
    }
}
