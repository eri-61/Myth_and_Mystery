using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Newtonsoft.Json.Bson;
public class GM_Puzzle1 : MonoBehaviour
{

    [Header("Gameplay Settings")]
    public float timeRemaining = 60f;     // Player must survive this long
    public int lives = 3;
    private bool gameEnded = false;

    [Header("UI References")]
    public TextMeshProUGUI timerText;     // Drag TextMeshPro object for timer
    public Image[] heartImages;           // Drag your 3 heart icons
    public GameObject winPanel;           // (Optional) “You Survived” panel

    [Header("Instructions")]
    public GameObject instructions;
    public Button closeInstructions;

    [Header("Scene Index")]
    public int loseIndex = 1;
    public int winIndex = 2;

    private void OnEnable()
    {
        closeInstructions.onClick.AddListener(() => instructions.SetActive(true));
    }

    private void OnDisable()
    {
        closeInstructions.onClick.RemoveAllListeners();
    }

    void Start()
    {
        Time.timeScale = 1f; // Ensure game starts running
        UpdateUI();
    }

    void Update()
    {
        if (gameEnded) return;

        // Countdown timer
        timeRemaining -= Time.deltaTime;

        if (timeRemaining <= 0f)
        {
            timeRemaining = 0f;
            EndGame(true); // Player survived the time
        }

        UpdateUI();
    }

    public void LoseLife()
    {
        if (gameEnded) return;

        lives--;
        UpdateUI();

        if (lives <= 0)
        {
            EndGame(false); // Player died
        }
    }

    void EndGame(bool survived)
    {
        gameEnded = true;
        Time.timeScale = 0f; // Stop gameplay

        if (survived)
        {
            Debug.Log("🎉 You survived the full 60 seconds!");
            if (winPanel != null) winPanel.SetActive(true);
            SceneManager.LoadScene(loseIndex);

        }
        else
        {
            Debug.Log("💀 Game Over!");
            SceneManager.LoadScene(loseIndex);
        }
    }

    void UpdateUI()
    {
        // Update timer text
        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timeRemaining).ToString();

        // Update hearts visibility
        for (int i = 0; i < heartImages.Length; i++)
        {
            heartImages[i].enabled = (i < lives);
        }
    }

}
