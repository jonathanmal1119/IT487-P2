using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class VehicleController : MonoBehaviour
{
    Rigidbody rb;

    public InputActionReference move;
    public InputActionReference handbrake;
    public InputActionReference exit;

    public float carPower = 1500f;
    public float brakeingPower = 3000f;
    public float maxSteeringAngle = 30f;

    [Header("Steering Response")]
    [Tooltip("How fast steering ramps up toward the target angle (deg/sec).")]
    public float steerRiseRateDegPerSec = 60f;
    [Tooltip("How fast steering ramps down/returns to center (deg/sec).")]
    public float steerFallRateDegPerSec = 120f;
    float currentSteerAngle = 0f;

    [Header("High-Speed Steering Limit")]
    [Tooltip("Speed in kph at which steering angle approaches the reduced percent.")]
    public float highSpeedKph = 80f;
    [Range(0f,1f)]
    [Tooltip("Percent of max steering allowed at high speed.")]
    public float highSpeedSteerPercent = 0.35f;

    public bool isPlayerInCar = false;


    [Header("Dynamics")]
    public float sidewaysDragCoefficient = 2.0f;

    [Header("Car Stats")]
    [SerializeField]
    float fuelLevel = 100;
    int maxFuelLevel = 100;

    public float FuelPercent => fuelLevel / maxFuelLevel;

    public float Speed => rb.linearVelocity.magnitude;

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

    public GameObject frontLeftWheelPos;
    public GameObject frontRightWheelPos;
    public GameObject rearLeftWheelPos;
    public GameObject rearRightWheelPos;

    Vector2 input = Vector2.zero;

    [Header("Player Refs")]
    public GameObject Player;
    public GameObject carCamera;

    bool isEngineDestroyed = false;

    [Header("UI Refs")]
    public Text carHP;
    public Text fuelLevelText;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        enabled = false;
        carCamera.SetActive(false);
    }

    void OnEnable()
    {
        if (isEngineDestroyed) return;

        move.action.Enable();
        handbrake.action.Enable();
        exit.action.Enable();
        if (handbrake) handbrake.action.Enable();
        carCamera.SetActive(true);
        isPlayerInCar = true;
    }

    void Update()
    {

        // BUG WITH INPUT
        if (Keyboard.current.eKey.wasPressedThisFrame || isEngineDestroyed)
        {
            Player.SetActive(true);
            Player.transform.position = transform.TransformPoint(new Vector3(3f, 1f, 0f));
            enabled = false;
            carCamera.SetActive(false);
            Player.GetComponent<PlayerLookControls>().VehicleController = null;
            Player.GetComponent<PlayerLookControls>().OnEnterExitVehicle?.Invoke();

            frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = 10000f;
            rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = 10000f;

            isPlayerInCar = false;
            return;
        }

        if (move)
        {
            input = move.action.ReadValue<Vector2>();
            if (!isPlayerInCar)
            {
                isPlayerInCar = true;
                Player.transform.position = transform.position;
            }
                
        }
            

		// Drive and braking/reverse logic
		float forwardSpeed = transform.InverseTransformDirection(rb.linearVelocity).z;
		float motorTorque = 0f;
		float brakeTorque = 0f;
		bool isHandbraking = (handbrake && handbrake.action.IsPressed());
		const float stopThreshold = 0.5f; 

		if (isHandbraking)
		{
			motorTorque = 0f;
			brakeTorque = brakeingPower;
		}
		else
		{
            if (input.y < -0.01f)
            {
                if (forwardSpeed > stopThreshold)
                {
                    motorTorque = 0f;
                    brakeTorque = brakeingPower;
                }
                else
                {
                    brakeTorque = 0f;
                    motorTorque = input.y * carPower;
                }
            }
            else if (input.y > 0.01f)
            {
                if (forwardSpeed < -stopThreshold)
                {
                    motorTorque = 0f;
                    brakeTorque = brakeingPower;
                }
                else
                {
                    brakeTorque = 0f;
                    motorTorque = input.y * carPower;
                }
            }
            else
            {
                motorTorque = 0f;
                brakeTorque = 0f;
            }
		}

		frontLeftWheelCollider.motorTorque = frontRightWheelCollider.motorTorque = rearLeftWheelCollider.motorTorque = rearRightWheelCollider.motorTorque = motorTorque;
		frontLeftWheelCollider.brakeTorque = frontRightWheelCollider.brakeTorque = rearLeftWheelCollider.brakeTorque = rearRightWheelCollider.brakeTorque = brakeTorque;

        // Steer
        float speedKph = rb.linearVelocity.magnitude * 3.6f;
        float limiterT = Mathf.InverseLerp(0f, highSpeedKph, speedKph);

        // Reduce available steering angle at high speed
        float steerLimiter = Mathf.Lerp(1f, highSpeedSteerPercent, limiterT);
        float targetSteerAngle = maxSteeringAngle * steerLimiter * input.x;

        // Also slow the ramp rate at high speed
        float rateScale = Mathf.Lerp(1f, 0.5f, limiterT);
        bool increasingMagnitude = Mathf.Abs(targetSteerAngle) > Mathf.Abs(currentSteerAngle);
        float baseRate = increasingMagnitude ? steerRiseRateDegPerSec : steerFallRateDegPerSec;
        float rate = baseRate * rateScale;

        currentSteerAngle = Mathf.MoveTowards(currentSteerAngle, targetSteerAngle, rate * Time.deltaTime);
        frontLeftWheelCollider.steerAngle = frontRightWheelCollider.steerAngle = currentSteerAngle;

        if (rb.linearVelocity.magnitude > 0.2f)
        {
            fuelLevel -= Time.deltaTime * fuelConsumptionRate;
            //fuelLevelText.text = "Fuel: " + fuelLevel.ToString("0");
        }

        UpdateWheelVisual(frontRightWheelCollider, frontRightWheelPos.transform);
        UpdateWheelVisual(frontLeftWheelCollider, frontLeftWheelPos.transform);
        UpdateWheelVisual(rearLeftWheelCollider, rearLeftWheelPos.transform);
        UpdateWheelVisual(rearRightWheelCollider, rearRightWheelPos.transform);
    }

    void UpdateWheelVisual(WheelCollider collider, Transform mesh)
    {
        collider.GetWorldPose(out Vector3 pos, out Quaternion rot);
        //mesh.position = pos;
        mesh.rotation = rot;

        mesh.localRotation = Quaternion.Euler(
            mesh.localRotation.eulerAngles.x,
            collider.steerAngle,
            mesh.localRotation.eulerAngles.z
        );
    }

    void FixedUpdate()
    {
        ApplySidewaysDrag();
    }

    void ApplySidewaysDrag()
    {
        if (rb == null) return;

        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        float lateralSpeed = localVelocity.x;

        if (Mathf.Abs(lateralSpeed) < 0.001f) return;

        float dragAcceleration = -lateralSpeed * sidewaysDragCoefficient;
        Vector3 dragForceWorld = transform.right * dragAcceleration;
        rb.AddForce(dragForceWorld, ForceMode.Acceleration);
    }
        
    
    public void Refuel(int amount)
    {
        fuelLevel += amount;
        fuelLevel = Mathf.Clamp(fuelLevel, 0, maxFuelLevel);
        //fuelLevelText.text = "Fuel: " + fuelLevel.ToString();
    }

    public bool CanRefuel()
    {
        return fuelLevel < maxFuelLevel;
    }

    public bool CanRepair()
    {
        return engineHealth < 100;
    }

    public void Repair(int amount)
    {
        engineHealth += amount;
        engineHealth = Mathf.Clamp(engineHealth, 0, 100);
        //carHP.text = "Car: " + engineHealth.ToString();
    }

    public void TakeDamage(int damage)
    {
        engineHealth = Mathf.Clamp(engineHealth - damage, 0, 100);
        if (engineHealth <= 0)
        {
            isEngineDestroyed = true;
        }

        //carHP.text = "Car: " + engineHealth.ToString();
    }

    public void SlowDown(float amt = 0.8f)
    {
        rb.linearVelocity *= amt;
        rb.angularVelocity *= amt;
    }

    public bool CanGetHurtByCar()
    {
        return isPlayerInCar;
    }
}