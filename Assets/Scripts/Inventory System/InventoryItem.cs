using UnityEngine;
using UnityEngine.UI;

public class InventoryItem : MonoBehaviour
{
    public Image image;

    [HideInInspector] public Items item;

    public void InitialiseItem(Items newItem)
    {
        item = newItem;
        image.sprite = newItem.image;
    }

}
