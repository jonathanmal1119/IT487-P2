using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class POIController : MonoBehaviour
{
    public GameObject objectivePart;
    public List<GameObject> enemiesInTrigger = new List<GameObject>();

    public Text enemyCountText;
    
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            if (!enemiesInTrigger.Contains(other.gameObject))
                enemiesInTrigger.Add(other.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            enemiesInTrigger.Remove(other.gameObject);
        }
    }

    void Update()
    {
        // Remove any destroyed enemies and keep objective state in sync
        if (enemiesInTrigger.RemoveAll(e => e == null) > 0)
        {
            if (objectivePart) objectivePart.SetActive(enemiesInTrigger.Count > 0);
        }

        enemyCountText.text = "Enemies: " + enemiesInTrigger.Count.ToString();
        if (enemiesInTrigger.Count == 0) 
        {
            objectivePart.SetActive(true);
        }
    }

}
