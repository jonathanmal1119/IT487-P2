using UnityEngine;
using System.Collections;

public class FuelPump : MonoBehaviour
{
    [SerializeField]
    int fuelAmount = 100;

    [Header("Fueling Settings")]
    public int fuelRate = 10;
    public float fuelingTime = 1f;

    bool fueling = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            fueling = true;
            StartCoroutine(FuelingCoroutine(other.GetComponentInParent<VehicleController>()));
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Vehicle"))
        {
            fueling = false;
        }
    }

    IEnumerator FuelingCoroutine(VehicleController vehicle)
    {
        while (fueling)
        {
            if (fuelAmount <= 0)
            {
                fueling = false;
                break;
            } 

            if (!vehicle.CanRefuel())
            {
                fueling = false;
                break;
            }

            fuelAmount -= fuelRate;
            vehicle.Refuel(fuelRate);

            yield return new WaitForSeconds(fuelingTime);
        }
    }
}
