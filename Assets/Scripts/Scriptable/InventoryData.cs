using UnityEngine;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Scriptable Objects/InventoryData")]
public class InventoryData : ScriptableObject
{
    public string itemName;
    public string itemDescription;
    public Sprite itemImage;
    public bool inInventory;
}
