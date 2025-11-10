using UnityEngine;

public class ObstacleMetaData : MonoBehaviour
{
    public bool destructible = true;
    public int damageToCar = 10;
	public float speedForMaxDamage = 20f; 
	public float minMultiplier = 0.25f;
	public float maxMultiplier = 2f;
    public float destroyTime = 0.1f;

    public void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.tag == "Vehicle")
        {
            float t = 1f;
            if (speedForMaxDamage > 0f)
                t = Mathf.Clamp(collision.relativeVelocity.magnitude / speedForMaxDamage, minMultiplier, maxMultiplier);

			int scaledDamage = Mathf.Max(1, Mathf.RoundToInt(damageToCar * t));

			collision.gameObject.GetComponent<VehicleController>().TakeDamage(scaledDamage);

            if (destructible || scaledDamage == damageToCar * maxMultiplier)
                Destroy(gameObject, destroyTime);
        }

    }
}
