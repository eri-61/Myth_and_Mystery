using UnityEngine;

public class BasketCollision : MonoBehaviour
{
    public GM_Puzzle1 gameManager;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Good"))
        {
            // Optional: reward player with extra time
            // gameManager.timeRemaining += 2f;
            Destroy(other.gameObject);
        }
        else if (other.CompareTag("Bad"))
        {
            gameManager.LoseLife();
            Destroy(other.gameObject);
        }
    }
}

