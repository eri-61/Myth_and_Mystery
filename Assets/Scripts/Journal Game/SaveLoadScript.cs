using UnityEngine;

public class SaveLoadScript : MonoBehaviour
{
    void Awake()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.RegisterSaveLoad(this);
        }
    }
}
