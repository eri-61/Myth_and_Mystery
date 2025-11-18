using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image image;

    [HideInInspector] public Items item;
    [HideInInspector] public int Count = 0;

    public void InitialiseItem(Items newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
        Count += 1;
    }

}
