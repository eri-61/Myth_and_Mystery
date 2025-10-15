using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public enum Battlestate { START, PLAYERTURN, ENEMYTURN, WON, LOST }
public enum DodgeResult { NotAttempted, Success, Failed }

public class BattleSystem : MonoBehaviour
{
    #region Variables
    public GameObject playerPrefab;
    public GameObject enemyPrefab;

    public Transform playerBattleStation;
    public Transform enemyBattleStation;

    public Battlestate state;
    public DodgeResult dodgeResult = DodgeResult.NotAttempted;

    public Unit playerUnit;
    public Unit enemyUnit;

    public TextMeshProUGUI dialogueText;

    public BattleHUD enemyHUD;
    public BattleHUD playerHUD;

    public GameObject inventoryPanel;

    [HideInInspector] public bool nextEnemyAttackDoubles = false;
    public ItemData correctItem;
    #endregion

    void Start()
    {
        state = Battlestate.START;
        StartCoroutine(SetupBattle());
    }

    IEnumerator SetupBattle()
    {
        GameObject playerGO = Instantiate(
            playerPrefab,
            playerBattleStation.position,
            playerBattleStation.rotation,
            playerBattleStation
        );
        playerUnit = playerGO.GetComponent<Unit>();

        GameObject enemyGO = Instantiate(
            enemyPrefab,
            enemyBattleStation.position,
            enemyBattleStation.rotation,
            enemyBattleStation
        );
        enemyUnit = enemyGO.GetComponent<Unit>();

        dialogueText.text = "The " + enemyUnit.unitName + " has ambushed you!";

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);

        correctItem = enemyUnit.weaknessItem;

        yield return new WaitForSeconds(2f);

        state = Battlestate.PLAYERTURN;
        PlayerTurn();
    }

    public IEnumerator PlayerAttack()
    {
        dialogueText.text = "Your attack is successful!";
        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
        enemyHUD.setHP(enemyUnit.currentHP);

        yield return new WaitForSeconds(2f);

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
    }

    public IEnumerator EnemyTurn()
    {
        dialogueText.text = "The " + enemyUnit.unitName + " attacked you!";
        yield return new WaitForSeconds(1f);

        if (dodgeResult == DodgeResult.Success)
        {
            dialogueText.text = "You dodged the attack!";
            yield return new WaitForSeconds(2f);
        }
        else if (dodgeResult == DodgeResult.Failed)
        {
            dialogueText.text = "You failed to dodge!";
            yield return new WaitForSeconds(2f);

            int damage = enemyUnit.damage;
            if (nextEnemyAttackDoubles)
            {
                damage *= 2;
                nextEnemyAttackDoubles = false;
                dialogueText.text = "The " + enemyUnit.unitName + "'s strikes harder due to you using the wrong item!";
                yield return new WaitForSeconds(2f);
            }

            bool isDead = playerUnit.TakeDamage(damage);
            playerHUD.setHP(playerUnit.currentHP);
            if (isDead)
            {
                state = Battlestate.LOST;
                EndBattle();
                yield break;
            }
        }
        else
        {
            int damage = enemyUnit.damage;
            if (nextEnemyAttackDoubles)
            {
                damage *= 2;
                nextEnemyAttackDoubles = false;
                dialogueText.text = "The " + enemyUnit.unitName + "'s strikes harder due to you using the wrong item!";
                yield return new WaitForSeconds(2f);
            }

            bool isDead = playerUnit.TakeDamage(damage);
            playerHUD.setHP(playerUnit.currentHP);
            if (isDead)
            {
                state = Battlestate.LOST;
                EndBattle();
                yield break;
            }
        }

        // Reset dodgeResult for next turn
        dodgeResult = DodgeResult.NotAttempted;

        state = Battlestate.PLAYERTURN;
        PlayerTurn();
    }

    public IEnumerator HandleDodge()
    {
        float dodgeChance = 0.15f;
        float roll = Random.Range(0f, 1f);

        dodgeResult = (roll < dodgeChance) ? DodgeResult.Success : DodgeResult.Failed;

        yield return new WaitForSeconds(0.2f); 

        state = Battlestate.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    public IEnumerator AttackSequence()
    {
        dialogueText.text = "You attacked the " + enemyUnit.unitName + "!";
        yield return new WaitForSeconds(1f);
        StartCoroutine(PlayerAttack());
    }

    void EndBattle()
    {
        if (state == Battlestate.WON)
        {
            dialogueText.text = "You won agaisnt the " + enemyUnit.unitName + "!";
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

        StartCoroutine(AttackSequence());
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
}
