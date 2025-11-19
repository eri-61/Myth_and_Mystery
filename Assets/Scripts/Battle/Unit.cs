using UnityEngine;

public class Unit : MonoBehaviour
{
    [Header ("HUD")]
    public string unitName;

    [Header ("HP")]
    public int currentHP;
    public int maxHP;

    [Header ("Damage")]
    public int damage;
    public Items weaknessItem;

    public bool TakeDamage(int dmg)
    {
        currentHP -= dmg;

        if (currentHP <= 0)
        {
            currentHP = 0;
            return true;
        }
        else
        {
            return false;
        }
    }
}
