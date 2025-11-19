using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PointerController : MonoBehaviour
{
    [Header("Aim Bar Settings")]
    public Transform triangleA;        
    public Transform triangleB;        
    public RectTransform safeZone;     
    public float moveSpeed = 100f;

    [Header("Lives UI")]
    public List<GameObject> lifeIcons = new List<GameObject>();

    [Header("Power Bar Settings")]
    public Image powerBar;             
    public float powerSpeed = 0.5f;    
    public float perfectMin = 0.45f;   
    public float perfectMax = 0.55f;   

    private RectTransform pointerTransform;
    private Vector3 targetPosition;

    private bool isMoving = true;
    private bool isCompleted = false;
    private int lives = 3;

    private bool inPowerPhase = false;
    private bool isHolding = false;
    private bool hasHeldPower = false; 
    private float currentPower = 0f;

    void Start()
    {
        pointerTransform = GetComponent<RectTransform>();
        lives = 3;
        UpdateLifeUI();
        ResetPointerToStart();

        if (powerBar != null)
            powerBar.fillAmount = 0f;
    }

    void Update()
    {
        if (lives <= 0 || isCompleted)
            return;

        if (!inPowerPhase)
        {
            if (isMoving)
                MovePointer();

            if (Input.GetMouseButtonDown(0) && isMoving)
            {
                isMoving = false;
                CheckAimSuccess();
            }
        }
        else
        {
            HandlePowerBar();
        }
    }

    #region Aim Bar Methods
    void MovePointer()
    {
        pointerTransform.position = Vector3.MoveTowards(pointerTransform.position, targetPosition, moveSpeed * Time.deltaTime);

        if (Vector3.Distance(pointerTransform.position, triangleA.position) < 0.1f)
            targetPosition = triangleB.position;
        else if (Vector3.Distance(pointerTransform.position, triangleB.position) < 0.1f)
            targetPosition = triangleA.position;
    }

    void CheckAimSuccess()
    {
        bool isInsideSafe = RectTransformUtility.RectangleContainsScreenPoint(safeZone, pointerTransform.position);

        if (isInsideSafe)
            StartPowerPhase();
        else
            LoseLife();
    }

    //Power Bar

    void StartPowerPhase()
    {
        inPowerPhase = true;
        isHolding = false;
        hasHeldPower = false; 
        currentPower = 0f;
        if (powerBar != null)
            powerBar.fillAmount = 0f;

        Debug.Log("Aim successful! Enter Power Bar Phase. Hold to reach correct power.");
    }

    void HandlePowerBar()
    {
        // Only increase power if player is holding
        if (!hasHeldPower && Input.GetMouseButtonDown(0))
        {
            isHolding = true;
            hasHeldPower = true; 
            Debug.Log("Started holding Power Bar.");
        }

        if (isHolding)
        {
            // Increase power bar gradually only while holding
            currentPower += powerSpeed * Time.deltaTime;
            currentPower = Mathf.Clamp01(currentPower);

            if (powerBar != null)
                powerBar.fillAmount = currentPower;
        }

       
        if (isHolding && Input.GetMouseButtonUp(0))
        {
            isHolding = false;
            EvaluatePower();
        }
    }


    void EvaluatePower()
    {
        if (currentPower >= perfectMin && currentPower <= perfectMax)
        {
            Debug.Log("Power perfect! Net thrown successfully.");
            CompleteGame();
        }
        else
        {
            Debug.Log("Power incorrect! Missed.");
            LoseLife();
            if (lives > 0)
                ResetPowerPhase();
        }
    }

    void ResetPowerPhase()
    {
        currentPower = 0f;
        hasHeldPower = false; 
        isHolding = false;
        inPowerPhase = true; 
        if (powerBar != null)
            powerBar.fillAmount = 0f;

        Debug.Log("Try Power Phase again.");
    }
    #endregion

    #region Life & End Methods
    void LoseLife()
    {
        lives = Mathf.Max(0, lives - 1);
        UpdateLifeUI();

        if (lives <= 0)
        {
            Debug.Log("Game Over! No lives left.");
            isMoving = false;
            inPowerPhase = false;
        }
        else
        {
            Debug.Log("Life lost! Remaining lives: " + lives);
            if (!inPowerPhase)
                Invoke(nameof(PrepareNextRound), 0.5f);
        }
    }

    void CompleteGame()
    {
        isCompleted = true;
        isMoving = false;
        inPowerPhase = false;

        Debug.Log("Game completed successfully!");
    }

    void PrepareNextRound()
    {
        if (lives > 0 && !isCompleted)
        {
            ResetPointerToStart();
            isMoving = true;
        }
    }
    #endregion

    void ResetPointerToStart()
    {
        pointerTransform.position = triangleA.position;
        targetPosition = triangleB.position;
    }

    void UpdateLifeUI()
    {
        if (lifeIcons == null || lifeIcons.Count < 3)
        {
            Debug.LogWarning("Please assign 3 life UI GameObjects in the inspector (lifeIcons).");
            return;
        }

        for (int i = 0; i < lifeIcons.Count; i++)
            lifeIcons[i].SetActive(i < lives);
    }
}
