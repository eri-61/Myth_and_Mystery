using UnityEngine;

[CreateAssetMenu(fileName = "ObjectiveData", menuName = "Scriptable Objects/ObjectiveData")]
public class ObjectiveData : ScriptableObject
{
    public string description;

    public bool isCompleted;
    public bool isMainObjective;
    public bool isVisible;
}
