using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum Battlestate { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
    #region Variables
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public Battlestate state;

    public Unit playerUnit;
    public Unit enemyUnit;

    public TextMeshProUGUI dialogueText;

    public BattleHUD enemyHUD;
    public BattleHUD playerHUD;

    private bool playerDodged = false;

    public GameObject inventoryPanel;

    [HideInInspector] public bool nextEnemyAttackDoubles = false;
    public InventoryData correctItem;
    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        state = Battlestate.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
        playerUnit = playerGO.GetComponent<Unit>();
        GameObject enemyGo = Instantiate(enemyPrefab, enemyBattleStation);
        enemyUnit = enemyGo.GetComponent<Unit>();

        dialogueText.text = enemyUnit.unitName + " has ambushed you!";
        
        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        correctItem = enemyUnit.weaknessItem;

        yield return new WaitForSeconds(2f);

        state = Battlestate.PLAYERTURN;
        PlayerTurn();
    }

    public IEnumerator PlayerAttack()
    {
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        
        enemyHUD.setHP(enemyUnit.currentHP);
        dialogueText.text = "The attack is successful!";

        if (isDead)
        {
            state = Battlestate.WON;
            EndBattle();
        }
        
        else
        {
            state = Battlestate.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }

        yield return new WaitForSeconds(2f);
    }

    public IEnumerator EnemyTurn()
    {
        dialogueText.text = enemyUnit.unitName + " attacks!";

        yield return new WaitForSeconds(1f);

        int damage = enemyUnit.damage;

        if(nextEnemyAttackDoubles)
        {
            damage *= 2;
            nextEnemyAttackDoubles = false;
            dialogueText.text = "The "+ enemyUnit.unitName + "'s strikes harder due to you using the wrong item!";
            yield return new WaitForSeconds(2f);
        }

        if (playerDodged)
        {
            dialogueText.text = "You dodged the attack!";
            playerDodged = false;
            yield return new WaitForSeconds(2f);
        }
        else
        {
            bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
            playerHUD.setHP(playerUnit.currentHP);

            if (isDead)
            {
                state = Battlestate.LOST;
                EndBattle();
                yield break;
            }

        }
        state = Battlestate.PLAYERTURN;
        PlayerTurn();
    }

    public IEnumerator HandleDodge()
    {
        float dodgeChance = 0.15f;
        float roll = Random.Range(0f, 1f);

        if (roll < dodgeChance)
        {
            dialogueText.text = "You dodged the attack!";
            playerDodged = true;
        }
        else
        {
            dialogueText.text = "Dodge failed!";
            playerDodged = false;
        }

        yield return new WaitForSeconds(2f);

        state = Battlestate.ENEMYTURN;
        StartCoroutine(EnemyTurn());

    }
    void EndBattle()
    {
        if (state == Battlestate.WON)
        {
            dialogueText.text = "You won agaisnt the "+enemyUnit.unitName + "!";
        }
        else if (state == Battlestate.LOST)
        {
            dialogueText.text = "You were defeated...";
        }
    }

    void PlayerTurn()
    {
        dialogueText.text = "Choose an action:";
    }

    public void OnAttackButton()
    {
        if (state != Battlestate.PLAYERTURN)
            return;
        StartCoroutine(PlayerAttack());
    }

    public void OnItemButton()
    {
        if (state != Battlestate.PLAYERTURN)
            return;

        inventoryPanel.SetActive(true);
        dialogueText.text = "Choose an item to use:";
    }

    public void oncloseInventory()
    {
        inventoryPanel.SetActive(false);
        dialogueText.text = "Choose an action:";
    }

    public void OnDodgeButton()
    {
        if (state != Battlestate.PLAYERTURN)
            return;

        StartCoroutine(HandleDodge());
    }

    public void UseItem(InventoryData item)
    {
        if (state != Battlestate.PLAYERTURN)
            return;
     
        
        if(item == correctItem)
        {
            // Right item used
            dialogueText.text = "You used the right item! The enemy is weakened!";
            enemyUnit.currentHP = 1;
            enemyHUD.setHP(enemyUnit.currentHP);

            state = Battlestate.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
        else
        {
            // Wrong item used
            dialogueText.text = "You used the wrong item! The enemy is enraged!";
            nextEnemyAttackDoubles = true;
            state = Battlestate.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

}
