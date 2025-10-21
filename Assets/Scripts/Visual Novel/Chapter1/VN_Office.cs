using UnityEngine;
using UnityEngine.UI;

public class VN_Office : MonoBehaviour
{
    [Header("Dialog Setup")]
    [SerializeField] private DialogTree dialogTree;
    [SerializeField] private int startSection = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (DialogController.instance != null && dialogTree != null)
        {
            DialogController.instance.StartDialog(dialogTree, startSection);
        }
        else
        {
            Debug.LogWarning("Dialog Controller / Tree missing");
        }
    }

    
}
