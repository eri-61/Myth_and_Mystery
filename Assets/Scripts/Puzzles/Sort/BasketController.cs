using UnityEngine;

public class BasketController : MonoBehaviour
{
    public float speed = 8f;
    private Vector3 touchStartPos;
    private Vector3 basketStartPos;
    private bool isDragging = false;

    void Update()
    {
        // --- Keyboard movement (for PC testing) ---
        float move = Input.GetAxis("Horizontal") * speed * Time.deltaTime;
        transform.Translate(move, 0, 0);

        // --- Touch control (for mobile) ---
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPos = Camera.main.ScreenToWorldPoint(new Vector3(touch.position.x, touch.position.y, 10f));

            switch (touch.phase)
            {
                case TouchPhase.Began:
                    // Begin dragging only if finger starts near basket
                    if (Mathf.Abs(touchPos.x - transform.position.x) < 2f)
                    {
                        isDragging = true;
                        touchStartPos = touchPos;
                        basketStartPos = transform.position;
                    }
                    break;

                case TouchPhase.Moved:
                    if (isDragging)
                    {
                        float deltaX = touchPos.x - touchStartPos.x;
                        Vector3 newPos = basketStartPos + new Vector3(deltaX, 0, 0);

                        // Clamp within screen limits
                        newPos.x = Mathf.Clamp(newPos.x, -7.5f, 7.5f);
                        transform.position = newPos;
                    }
                    break;

                case TouchPhase.Ended:
                case TouchPhase.Canceled:
                    isDragging = false;
                    break;
            }
        }

        // --- Keep basket inside bounds (extra safety) ---
        Vector3 pos = transform.position;
        pos.x = Mathf.Clamp(pos.x, -7.5f, 7.5f);
        transform.position = pos;
    }
}
