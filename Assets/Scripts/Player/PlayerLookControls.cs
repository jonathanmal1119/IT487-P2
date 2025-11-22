using Assets.Scripts;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerLookControls : MonoBehaviour
{
    public Transform horizontalOrientation;
    public Transform verticalPivot;

    public Transform recoilPivot;

    InputAction lookAction;
    InputAction interactAction;

    public float horizontalLookSpeed = 1f;
    public float verticalLookSpeed = 1f;

    Vector2 rotChange;
    float Vrot, Hrot;

    public Vector2 verticalLookClamp;

    public Camera playerCam;
    public LayerMask interactLayer;

    public float OriginalFOV { get; set; }

    public VehicleController? VehicleController { get; set; }
    public Action? OnEnterExitVehicle;
    public bool InVehicle => VehicleController != null;

    private bool enableMouse = true;
    public bool EnableMouse { 
        get => enableMouse;
        set {
            enableMouse = value;
            if (!enableMouse)
            {
                lookAction.Disable();
                interactAction.Disable();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            else
            {
                lookAction.Enable();
                interactAction.Enable();
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
        }
    }
    public float Sensitivity { get; set; }

    private Vector2 remainingVisualRecoil = new();
    private Vector2 currentRecoilAngle = new();

    private Vector3 remainingShake = new();
    private Vector3 currentShake = new();

    private void Awake()
    {
        lookAction = InputSystem.actions.FindAction("Player/Look");
        interactAction = InputSystem.actions.FindAction("Player/Interact");
        OriginalFOV = playerCam.fieldOfView;
        Sensitivity = PlayerPrefs.GetFloat("sensitivity", 1);
    }

    private void OnEnable()
    {
        lookAction.Enable();
        interactAction.Enable();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnDisable()
    {
        lookAction.Disable();
        interactAction.Disable();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    //Just a small glimpse into my sick, twisted world
    private void Update()
    {
        if (!enableMouse) return;

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

        Hrot += horizontalLookSpeed * rotChange.x * Sensitivity * (playerCam.fieldOfView / OriginalFOV);
        Vrot -= verticalLookSpeed * rotChange.y * Sensitivity * (playerCam.fieldOfView / OriginalFOV);

        Vrot = Mathf.Clamp(Vrot, verticalLookClamp.x, verticalLookClamp.y);

        horizontalOrientation.eulerAngles = new Vector3(0.0f, Hrot, 0.0f);
        verticalPivot.localEulerAngles = new Vector3(Vrot, 0.0f, 0.0f);

        if (interactAction.WasPressedThisFrame())
        {
            Interact();
        }

        recoilPivot.localEulerAngles = HandleCameraRecoil() + HandleScreenShake();
    }

    void Interact()
    {
        Ray ray = new(transform.position, playerCam.transform.forward);

        Debug.DrawRay(ray.origin, ray.direction * 4f, Color.green, 1f);

        if (Physics.Raycast(ray, out RaycastHit hit, 4f, interactLayer))
        {
            if (hit.collider.gameObject.CompareTag("Vehicle"))
            {
                VehicleController = hit.collider.gameObject.GetComponentInParent<VehicleController>();
                VehicleController.enabled = true;
                OnEnterExitVehicle?.Invoke();
                gameObject.SetActive(false);
            }
        }
        else
        {
            Debug.Log("Nothing hit.");
        }
    }

    private Vector3 HandleCameraRecoil()
    {
        float recoilForce = 14;
        float restSpeed = 7;
        if (GetComponent<PlayerWeaponManager>().N()?.ActiveWeapon.IsAiming == false)
        {
            recoilForce *= 0.4f;
            restSpeed *= 0.6f;
        }

        if (remainingVisualRecoil.x > 0)
        {
            remainingVisualRecoil.x = Mathf.Clamp(remainingVisualRecoil.x - restSpeed * 1.5f * Time.deltaTime, 0, 10);
            currentRecoilAngle.x = Mathf.Lerp(currentRecoilAngle.x, -remainingVisualRecoil.x, recoilForce * Time.deltaTime);
        }
        else if (remainingVisualRecoil.x <= 0)
        {
            currentRecoilAngle.x = Mathf.Lerp(currentRecoilAngle.x, 0, restSpeed * Time.deltaTime);
        }

        if (remainingVisualRecoil.y != 0)
        {
            remainingVisualRecoil.y = Mathf.Clamp(Mathf.Sign(remainingVisualRecoil.y) * Mathf.Max(0, Mathf.Abs(remainingVisualRecoil.y) - restSpeed * 1.5f * Time.deltaTime), -20, 20);
            currentRecoilAngle.y = Mathf.Lerp(currentRecoilAngle.y, remainingVisualRecoil.y, recoilForce * Time.deltaTime);
        }
        else if (currentRecoilAngle.y != 0)
        {
            currentRecoilAngle.y = Mathf.Lerp(currentRecoilAngle.y, 0, restSpeed * Time.deltaTime);
        }

        return new(currentRecoilAngle.x, currentRecoilAngle.y, 0);
    }

    private Vector3 HandleScreenShake()
    {
        float recoilForce = 22;
        float restSpeed = 18;

        if (remainingShake.x > 0)
        {
            remainingShake.x = Mathf.Clamp(remainingShake.x - restSpeed * 1.5f * Time.deltaTime, 0, 10);
            currentShake.x = Mathf.Lerp(currentShake.x, -remainingShake.x, recoilForce * Time.deltaTime);
        }
        else if (remainingShake.x <= 0)
        {
            currentShake.x = Mathf.Lerp(currentShake.x, 0, restSpeed * Time.deltaTime);
        }

        if (remainingShake.y != 0)
        {
            remainingShake.y = Mathf.Clamp(Mathf.Sign(remainingShake.y) * Mathf.Max(0, Mathf.Abs(remainingShake.y) - restSpeed * 1.5f * Time.deltaTime), -20, 20);
            currentShake.y = Mathf.Lerp(currentShake.y, remainingShake.y, recoilForce * Time.deltaTime);
        }
        else if (currentShake.y != 0)
        {
            currentShake.y = Mathf.Lerp(currentShake.y, 0, restSpeed * Time.deltaTime);
        }

        if (remainingShake.z != 0)
        {
            remainingShake.z = Mathf.Clamp(Mathf.Sign(remainingShake.z) * Mathf.Max(0, Mathf.Abs(remainingShake.z) - restSpeed * 1.5f * Time.deltaTime), -20, 20);
            currentShake.z = Mathf.Lerp(currentShake.z, remainingShake.z, recoilForce * Time.deltaTime);
        }
        else if (currentShake.z != 0)
        {
            currentShake.z = Mathf.Lerp(currentShake.z, 0, restSpeed * Time.deltaTime);
        }

        return currentShake;
    }

    public void AddCameraRecoil(float vertical, float randomHorizontal = 0)
    {
        float recoilSmoothing = Mathf.Clamp01(1 - (rotChange.magnitude / 4f));

        remainingVisualRecoil.x += vertical * recoilSmoothing;
        remainingVisualRecoil.y += UnityEngine.Random.Range(-randomHorizontal, randomHorizontal) * recoilSmoothing;
    }

    public void AddScreenShake(float vertical, float horizontal, float twist)
    {
        remainingShake.x += vertical;
        remainingShake.y += horizontal;
        remainingShake.z += twist;
    }
}
