using UnityEngine;

public class RepairPickup : MonoBehaviour
{
    public int repairAmount = 100;
    public VehicleController vehicle;

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
            
            if (vehicle != null && vehicle.CanRepair())
            {
                vehicle.Repair(repairAmount);
                Destroy(gameObject);
            }
        }
    }
}
