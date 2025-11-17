using System.ComponentModel;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu(fileName = "Items", menuName = "Scriptable Objects/Items")]
public class Items : ScriptableObject
{
    public Sprite image;
    public ItemType type;
    public ActionType actionType;
}

public enum ItemType
{
    Weapon,
    Key,
    Weakness,
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