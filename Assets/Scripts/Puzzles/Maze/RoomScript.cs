using System.Collections.Generic;
using UnityEngine;

public class RoomScript : MonoBehaviour
{
    #region Variables
    [SerializeField] GameObject TopWall;
    [SerializeField] GameObject BottomWall;
    [SerializeField] GameObject LeftWall;
    [SerializeField] GameObject RightWall;
    #endregion
    public enum Direction
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT,
        NONE,
    }

    Dictionary<Direction, GameObject> walls = new Dictionary<Direction, GameObject>();

    public Vector2Int Index
    {
        get;
        set;
    }

    public bool visited
    {
        get;
        set;
    } = false;

    Dictionary<Direction, bool> dirFlags = new Dictionary<Direction, bool>();

    private void Awake()
    {
        walls[Direction.TOP] = TopWall;
        walls[Direction.RIGHT] = RightWall;
        walls[Direction.LEFT] = LeftWall;
        walls[Direction.BOTTOM] = BottomWall;

        foreach (Direction dir in walls.Keys)
        {
            dirFlags[dir] = true;
            walls[dir].SetActive(true);
        }
    }

    private void SetActive(Direction dir, bool flag)
    {
        walls[dir].SetActive(flag);
    }

    public void SetDirFlag(Direction dir, bool flag)
    {
        dirFlags[dir] = flag;
        SetActive(dir, flag);
    }

    public bool CanMove(Direction dir)
    {
        if (dirFlags.ContainsKey(dir))
        {
            return dirFlags[dir] == false; 
        }
        return false;
    }
}
