using UnityEngine;

public class RefuelPickup : MonoBehaviour
{
    public int fuelAmount = 25;
    public VehicleController vehicle;

    public AudioClip pickupSound;
    public float soundVolume = 1f;

    public void Start()
    {
        if (vehicle == null)
        {
            GameObject v = GameObject.Find("Vehicle");
            if (v != null) { vehicle = v.GetComponent<VehicleController>(); }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (vehicle == null)
            {
                GameObject v = GameObject.Find("Vehicle");
                if (v != null) { vehicle = v.GetComponent<VehicleController>(); }
                if (vehicle == null)
                {
                    Debug.LogWarning("Repair Pickup couldn't automatically find the vehicle object. Try manually giving the vehicle script to the pickup in the inspector.");
                    Destroy(gameObject);
                }
            }

            if (vehicle != null && vehicle.CanRefuel())
            {
                vehicle.Refuel(fuelAmount);

                if (pickupSound != null)
                {
                    AudioSource.PlayClipAtPoint(pickupSound, transform.position, soundVolume);
                }

                Destroy(gameObject);
            }
        }
    }
}
