using UnityEngine;

public class ArmorPickup : MonoBehaviour
{
    public float percentNegation = 0.25f;
    public int armorPoints = 40;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth w = other.GetComponent<PlayerHealth>();
            bool itemUsed = false;
            //only use this armor's percent negation if the player's current armor is less than what this item provides.
            if (w != null && w.armorDamageReductionPercent < percentNegation)
            {
                w.armorDamageReductionPercent = percentNegation;
                itemUsed = true;
            }
            //only use this armor's point value if the player currently has less.
            if(w != null && w.armorPoints < armorPoints)
            {
                w.AddArmor(armorPoints);
                itemUsed = true;
            }

            //destroy the item if it has been used
            if (itemUsed)
            {
                Destroy(gameObject);
            }
        }
    }
}
