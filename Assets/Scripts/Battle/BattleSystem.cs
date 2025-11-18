//using UnityEngine;
//using UnityEngine.UI;
//using TMPro;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine.Video;
//using UnityEngine.SceneManagement;

//public enum Battlestate { START, PLAYERTURN, ENEMYTURN, WON, LOST }
//public enum DodgeResult { NotAttempted, Success, Failed }

//public class BattleSystem : MonoBehaviour
//{
//    #region Variables
//    public GameObject playerPrefab;
//    public GameObject enemyPrefab;

//    public Transform playerBattleStation;
//    public Transform enemyBattleStation;

//    public Battlestate state;
//    public DodgeResult dodgeResult = DodgeResult.NotAttempted;

//    public Unit playerUnit;
//    public Unit enemyUnit;

//    public TextMeshProUGUI dialogueText;

//    public BattleHUD enemyHUD;
//    public BattleHUD playerHUD;

//    public GameObject inventoryPanel;

//    public VideoPlayer battleIntro;
//    [HideInInspector] public bool nextEnemyAttackDoubles = false;
//    public ItemData correctItem;

//    public int Victory = 1;
//    public int Lost = 2;

//    // Prevent input spamming while an action is in progress
//    private bool inputLocked = false;
//    #endregion

//    void Start()
//    {
//        state = Battlestate.START;
//        StartCoroutine(PlayAnimation());
//    }
//    IEnumerator PlayAnimation()
//    {
//        battleIntro.Play();
//        yield return new WaitForSecondsRealtime((float)battleIntro.clip.length);
//        battleIntro.gameObject.SetActive(false);
//        yield return new WaitForSecondsRealtime(0.25f);

//        StartCoroutine(SetupBattle());
//    }
//    IEnumerator SetupBattle()
//    {
//        GameObject playerGO = Instantiate(
//            playerPrefab,
//            playerBattleStation.position,
//            playerBattleStation.rotation,
//            playerBattleStation
//        );
//        playerUnit = playerGO.GetComponent<Unit>();

//        GameObject enemyGO = Instantiate(
//            enemyPrefab,
//            enemyBattleStation.position,
//            enemyBattleStation.rotation,
//            enemyBattleStation
//        );
//        enemyUnit = enemyGO.GetComponent<Unit>();

//        dialogueText.text = "The " + enemyUnit.unitName + " has ambushed you!";

//        playerHUD.SetHUD(playerUnit);
//        enemyHUD.SetHUD(enemyUnit);

//        correctItem = enemyUnit.weaknessItem;

//        yield return new WaitForSeconds(2f);

//        state = Battlestate.PLAYERTURN;
//        PlayerTurn();
//    }

//    public IEnumerator PlayerAttack()
//    {
//        dialogueText.text = "Your attack is successful!";
//        bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
//        enemyHUD.setHP(enemyUnit.currentHP);

//        yield return new WaitForSeconds(2f);

//        if (isDead)
//        {
//            state = Battlestate.WON;
//            EndBattle();
//        }
//        else
//        {
//            state = Battlestate.ENEMYTURN;
//            StartCoroutine(EnemyTurn());
//        }
//    }

//    public IEnumerator EnemyTurn()
//    {
//        dialogueText.text = "The " + enemyUnit.unitName + " attacked you!";
//        yield return new WaitForSeconds(1f);

//        if (dodgeResult == DodgeResult.Success)
//        {
//            dialogueText.text = "You dodged the attack!";
//            yield return new WaitForSeconds(2f);
//        }
//        else if (dodgeResult == DodgeResult.Failed)
//        {
//            dialogueText.text = "You failed to dodge!";
//            yield return new WaitForSeconds(2f);

//            int damage = enemyUnit.damage;
//            if (nextEnemyAttackDoubles)
//            {
//                damage *= 2;
//                nextEnemyAttackDoubles = false;
//                dialogueText.text = "The " + enemyUnit.unitName + "'s strikes harder due to you using the wrong item!";
//                yield return new WaitForSeconds(2f);
//            }

//            bool isDead = playerUnit.TakeDamage(damage);
//            playerHUD.setHP(playerUnit.currentHP);
//            if (isDead)
//            {
//                state = Battlestate.LOST;
//                EndBattle();
//                yield break;
//            }
//        }
//        else
//        {
//            int damage = enemyUnit.damage;
//            if (nextEnemyAttackDoubles)
//            {
//                damage *= 2;
//                nextEnemyAttackDoubles = false;
//                dialogueText.text = "The " + enemyUnit.unitName + "'s strikes harder due to you using the wrong item!";
//                yield return new WaitForSeconds(2f);
//            }

//            bool isDead = playerUnit.TakeDamage(damage);
//            playerHUD.setHP(playerUnit.currentHP);
//            if (isDead)
//            {
//                state = Battlestate.LOST;
//                EndBattle();
//                yield break;
//            }
//        }

//        dodgeResult = DodgeResult.NotAttempted;

//        state = Battlestate.PLAYERTURN;
//        PlayerTurn();
//    }

//    public IEnumerator HandleDodge()
//    {
//        float dodgeChance = 0.15f;
//        float roll = Random.Range(0f, 1f);

//        dodgeResult = (roll < dodgeChance) ? DodgeResult.Success : DodgeResult.Failed;

//        yield return new WaitForSeconds(0.2f);

//        state = Battlestate.ENEMYTURN;
//        StartCoroutine(EnemyTurn());
//    }

//    public IEnumerator AttackSequence()
//    {
//        dialogueText.text = "You attacked the " + enemyUnit.unitName + "!";
//        yield return new WaitForSeconds(1f);
//        StartCoroutine(PlayerAttack());
//    }

//    void EndBattle()
//    {
//        if (state == Battlestate.WON)
//        {
//            dialogueText.text = "You won agaisnt the " + enemyUnit.unitName + "!";
//            SceneManager.LoadScene(Victory);
//        }
//        else if (state == Battlestate.LOST)
//        {
//            dialogueText.text = "You were defeated...";
//            SceneManager.LoadScene(Lost);

//        }
//    }

//    void PlayerTurn()
//    {
//        // Allow player input when it's their turn
//        inputLocked = false;
//        dialogueText.text = "Choose an action:";
//    }

//    public void OnAttackButton()
//    {
//        if (state != Battlestate.PLAYERTURN)
//            return;

//        if (inputLocked) // guard against spam
//            return;

//        inputLocked = true;
//        StartCoroutine(AttackSequence());
//    }

//    public void OnItemButton()
//    {
//        if (state != Battlestate.PLAYERTURN)
//            return;
//        dialogueText.text = "Choose an item to use:";
//    }

//    private void UseItemInBattle(ItemData item)
//    {
//        if (state != Battlestate.PLAYERTURN) return;
//        if (item == null) return;
//        if (item == correctItem)
//        {
//            dialogueText.text = $"You used {item.itemName}! It was super effective!";
//            enemyUnit.currentHP = 1;
//            enemyHUD.setHP(1);
//        }
//        else
//        {
//            dialogueText.text = $"You used {item.itemName}, but it had no effect!";
//            nextEnemyAttackDoubles = true;
//        }
//        state = Battlestate.ENEMYTURN;
//        StartCoroutine(EnemyTurn());
//    }

//    public IEnumerator AfterUseItem()
//    {
//        yield return new WaitForSecondsRealtime(1f);
//        state = Battlestate.ENEMYTURN;
//        StartCoroutine(EnemyTurn());
//    }

//    public void OnDodgeButton()
//    {
//        if (state != Battlestate.PLAYERTURN)
//            return;

//        if (inputLocked) // guard against spam
//            return;

//        inputLocked = true;
//        StartCoroutine(HandleDodge());
//    }

//}
