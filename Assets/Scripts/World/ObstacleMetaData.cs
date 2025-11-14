using UnityEngine;

public class ObstacleMetaData : MonoBehaviour
{
    public bool destructible = true;
    public int damageToCar = 10;
    public float speedForMaxDamage = 20f;
    public float minMultiplier = 0.25f;
    public float maxMultiplier = 1.5f;
    public float slownDownAmt = 0.5f;

    public void OnCollisionEnter(Collision collision)
    {
        OnTriggerEnter(collision.collider);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Vehicle")
        {
            float t = 1f;
            if (speedForMaxDamage > 0f)
                t = Mathf.Clamp(other.GetComponentInParent<Rigidbody>().linearVelocity.magnitude / speedForMaxDamage, minMultiplier, maxMultiplier);

            int scaledDamage = Mathf.Max(1, Mathf.RoundToInt(damageToCar * t));

            VehicleController vc = other.gameObject.GetComponentInParent<VehicleController>();
            vc.TakeDamage(scaledDamage);
            vc.SlowDown(slownDownAmt);

            if (destructible || scaledDamage == damageToCar * maxMultiplier)
                Destroy(gameObject, 0.1f);
        }
    }
}