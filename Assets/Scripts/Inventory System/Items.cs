using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Items : ScriptableObject
{
    public string itemName;
    public Sprite image;
    bool inInventory = false;
    public ItemType type;
    public ActionType actionType;
}

public enum ItemType
{
    Weapon,
    Weakness,
    Key,
    PuzzleItem,
    Flashlight
}

public enum ActionType
{
    None,
    Attack,
    Light,
    Unlock,
    Puzzle
}