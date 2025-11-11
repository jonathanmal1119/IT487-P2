using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    public InputActionReference move;        
    public InputActionReference handbrake;
    public InputActionReference exit;

    public float carPower = 1500f;
    public float brakeingPower = 3000f;
    public float maxSteeringAngle = 30f;


    [Header("Dynamics")]
    [Tooltip("Acceleration applied opposite to lateral (sideways) speed. Units: 1/s")]
    public float sidewaysDragCoefficient = 2.0f;

    [Header("Car Stats")]
    [SerializeField]
    float fuelLevel = 100;
    int maxFuelLevel = 100;

    [SerializeField]
    int engineHealth = 100;

    [SerializeField]
    int enginePerformance = 100;

    [SerializeField]
    float fuelConsumptionRate = 1f;

    [Header("Car Parts")]
    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelPos;
    public Transform frontRightWheelPos;
    public Transform rearLeftWheelPos;
    public Transform rearRightWheelPos;

    Vector2 input = Vector2.zero;

    public GameObject Player;
    public GameObject Camera;

    void Awake()
    {
        enabled = false;
        Camera.SetActive(false);
    }

    void OnEnable()
    {
        move.action.Enable();
        handbrake.action.Enable();
        exit.action.Enable();
        if (handbrake) handbrake.action.Enable();
        carCamera.SetActive(true);
    }

    void Update()
    {
        // BUG WITH INPUT
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Player.SetActive(true);
            Player.transform.position = transform.TransformPoint(new Vector3(3f, 1f, 0f));
            this.enabled = false;
            Camera.SetActive(false);

            frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = 10000f;
            rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = 10000f;

            return;
        }

        if (move) 
            input = move.action.ReadValue<Vector2>();

        // Drive
        rearLeftWheelCollider.motorTorque = rearRightWheelCollider.motorTorque = input.y * carPower;

        if (handbrake && handbrake.action.IsPressed())
        {
            frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = brakeingPower;
            rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = brakeingPower;
        }
        else
        {
            frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = 0;
            rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = 0;
        }
        
        // Steer
        frontLeftWheelCollider.steerAngle = frontRightWheelCollider.steerAngle = maxSteeringAngle * input.x;

        // BUGGED
        // Align the wheels to the colider
        //AlignWheels(frontLeftWheelCollider, frontLeftWheelPos);
        //AlignWheels(frontRightWheelCollider, frontRightWheelPos);
        //AlignWheels(rearLeftWheelCollider, rearLeftWheelPos);
        //AlignWheels(rearRightWheelCollider, rearRightWheelPos);
    }

    static void AlignWheels(WheelCollider col, Transform wheel)
    {
        col.GetWorldPose(out var pos, out var rot);

        wheel.position = pos;
        wheel.rotation = rot;
    }
}
