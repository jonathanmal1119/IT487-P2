using UnityEngine;
using System.Collections.Generic;

public class poiMetaData : MonoBehaviour
{
	public ObjectiveData POI_data;
	public GameObject spawnLoc;

	private GameObject part = null;

	[Header("Enemies In Trigger")]
	public HashSet<GameObject> enemiesInTrigger = new HashSet<GameObject>();


	void Update()
	{
		if (enemiesInTrigger.Count == 0) return;

		var snapshot = new List<GameObject>(enemiesInTrigger);
		for (int i = 0; i < snapshot.Count; i++)
		{
			GameObject enemy = snapshot[i];
			if (enemy == null || !enemy.activeInHierarchy)
			{
				if (enemiesInTrigger.Count == 1)
				{
					part = Instantiate(POI_data.objectivePart);
					part.transform.position = spawnLoc.transform.position;
				}

				enemiesInTrigger.Remove(enemy);
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (!other || !other.CompareTag("Enemy")) return;

		enemiesInTrigger.Add(other.gameObject);
	}
}
