using UnityEngine;

public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 5f;
    private GM_Puzzle1 gameManager; // ✅ reference to GameManager
    private bool isGood = false;

    void Start()
    {
        // ✅ Find the GameManager in the scene
        gameManager = FindObjectOfType<GM_Puzzle1>();

        // ✅ Determine if this object is a Good object
        if (CompareTag("Good"))
            isGood = true;
    }

    void Update()
    {
        // Fall down
        transform.Translate(Vector2.down * fallSpeed * Time.deltaTime);

        // ✅ If it goes below the screen
        if (transform.position.y < -6f)
        {
            // ✅ If this was a Good object and it wasn't caught, lose a life
            if (isGood && gameManager != null)
            {
                gameManager.LoseLife();
            }

            // Destroy the object
            Destroy(gameObject);
        }
    }
}
