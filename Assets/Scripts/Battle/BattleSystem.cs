using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Systems.Collections;
using Systems.Collections.Generic;

public enum Battlestate { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    #region Variables
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public Battlestate state;

    public TextMeshProUGUI dialogueText;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = Battlestate.START;
        SetupBattle();
    }

    void SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerGO.GetComponent<Unit>();
        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyGo.GetComponent<Unit>();

        dialogueText.text = enemyGo.unit.unitName + " has ambushed you!";
    }

}
