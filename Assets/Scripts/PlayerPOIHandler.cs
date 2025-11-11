using UnityEngine;

public class PlayerPOIHandler : MonoBehaviour
{
    public GameObject winScreen;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "ObjectivePart")
        {
            GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerLookControls>().enabled = false;
            winScreen.SetActive(true);
            Destroy(other.gameObject);
        }
    }

}
