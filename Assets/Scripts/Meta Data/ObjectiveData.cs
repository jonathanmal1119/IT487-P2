using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveData", menuName = "Scriptable Objects/ObjectiveData")]
public class ObjectiveData : ScriptableObject
{
    [Header("Basic Info")]
    public string objectiveName;
    [TextArea] public string description;

    [Header("Reward")]
    public GameObject objectivePart;
}
