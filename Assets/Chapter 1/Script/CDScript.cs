using UnityEngine;
using TMPro;
public class CDScript : MonoBehaviour
{
    public GameObject Chara1;
    public GameObject Chara2;


    public TextMeshPro name;
    public TextMeshPro dialog;

    public Canvas GameUI;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Chara2.SetActive(false);
        GameUI.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Chara1.SetActive(false);
            Chara2.SetActive(true);
            GameUI.gameObject.SetActive(true);
            dialog.text = "Hey! Did you fall asleep at the office again?!";
            name.text = "Rafael";
        }
    }
}
