using UnityEngine;
using UnityEngine.InputSystem;

public class VehicleController : MonoBehaviour
{
    public InputActionReference move;        
    public InputActionReference handbrake;

    public float carPower = 1500f;
    public float brakeingPower = 3000f;
    public float maxSteeringAngle = 30f;

    public WheelCollider frontLeftWheelCollider;
    public WheelCollider frontRightWheelCollider;
    public WheelCollider rearLeftWheelCollider;
    public WheelCollider rearRightWheelCollider;

    public Transform frontLeftWheelPos;
    public Transform frontRightWheelPos;
    public Transform rearLeftWheelPos;
    public Transform rearRightWheelPos;

    Vector2 input = Vector2.zero;

    void Update()
    {
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

        // Align the wheels to the colider
        AlignWheels(frontLeftWheelCollider, frontLeftWheelPos);
        AlignWheels(frontRightWheelCollider, frontRightWheelPos);
        AlignWheels(rearLeftWheelCollider, rearLeftWheelPos);
        AlignWheels(rearRightWheelCollider, rearRightWheelPos);
    }

    static void AlignWheels(WheelCollider col, Transform wheel)
    {
        col.GetWorldPose(out var pos, out var rot);

        wheel.position = pos;
        wheel.rotation = rot;
    }
}
