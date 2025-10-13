using UnityEngine;
using UnityEngine.UI;
using TMPro;

using System.Collections.Generic;

public class CreaturesScript : MonoBehaviour
{
    #region Variables
    [Header("Slots")]
    public List<Button> slots;

    [Header("Creatures Details")]
    public TextMeshProUGUI creatureName;
    public TextMeshProUGUI creatureDescription;
    public Image creatureImage;

    [Header("Variables and Data")]
    public List<CreaturesData> creatures = new();
    #endregion

    void Awake()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.RegisterCreatures(this);
        }
    }

    void OnEnable()
    {
        for(int i = 0; i < slots.Count; i++)
        {
            int index = i;
            slots[i].onClick.AddListener(() => showCreatures(index));
        }
        UpdateCreaturesUI();
    }

    void OnDisable()
    {
        for (int i = 0; i < slots.Count; i++)
        {
            int index = i;
            slots[i].onClick.RemoveAllListeners();
        }
    }

    public void AddCreature(CreaturesData newCreature)
    {
        if (!creatures.Contains(newCreature))
        {
            creatures.Add(newCreature);
            UpdateCreaturesUI();
        }
    }

    public void showCreatures(int index)
    {
        if (index < 0 || index >= creatures.Count) return;
        var creature = creatures[index];
        creatureName.text = creature.name;
    }

    public void UpdateCreaturesUI()
    {

    }
}
