using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectiveUIController : MonoBehaviour
{
    public GameObject POI_Objective;

    poiMetaData meta;

    public TextMeshProUGUI ObjectiveText;
    public TextMeshProUGUI EnemiesCount;

    void Start()
    {
        meta = POI_Objective.GetComponent<poiMetaData>();
    }

    void Update()
    {
        EnemiesCount.text = meta.enemiesInTrigger.Count.ToString();

        if (meta.enemiesInTrigger.Count == 0)
        {
            ObjectiveText.text = "Collect the explosive and interact with the concrete wall to explode it.";
            EnemiesCount.enabled = false;
        }
    }
}
