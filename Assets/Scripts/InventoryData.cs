using UnityEngine;

[CreateAssetMenu(fileName = "InventoryScrptable", menuName = "Scriptable Objects/InventoryScrptable")]
public class InventoryScrptable : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public bool inInventory;
    
}
