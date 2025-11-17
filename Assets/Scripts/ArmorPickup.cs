using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public float percentNegation = 0.25f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth w = other.GetComponent<PlayerHealth>();

            //only use this armor item if the player's current armor is less than what this item provides.
            if (w != null && w.armorDamageReductionPercent < percentNegation)
            {
                w.armorDamageReductionPercent = percentNegation;
                Destroy(gameObject);
            }
        }
    }
}
