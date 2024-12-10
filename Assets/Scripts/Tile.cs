using UnityEngine;

public class Tile : MonoBehaviour
{
    public bool placed = false; // Whether the tile is placed on the grid
    public Sprite frontSprite;  // The sprite for the letter side (front)
    public Sprite backSprite;   // The sprite for the back of the tile
    public string letter;       // The letter on the tile
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteRenderer.sprite = backSprite; // Start with the back sprite
    }

    public void ShowFront()
    {
        if (spriteRenderer.sprite == backSprite)
        {
            Turn();
        }
    }

    public void ShowBack()
    {
        if (spriteRenderer.sprite == frontSprite)
        {
            Turn();
        }
    }

    public void Shine()
    {
        // Start a coroutine to handle the shine animation
        StartCoroutine(ShineEffect());
    }

    private System.Collections.IEnumerator ShineEffect()
    {
        float duration = 0.5f; // Total duration of the shine effect
        float elapsedTime = 0f;

        Vector3 originalScale = transform.localScale;
        Vector3 targetScale = originalScale * 1.5f; // Enlarge the tile slightly

        // Lerp the scale up and down
        while (elapsedTime < duration)
        {
            float t = elapsedTime / (duration / 2); // Normalize time for up and down phase
            transform.localScale = Vector3.Lerp(originalScale, targetScale, Mathf.PingPong(t, 1));

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Restore the original scale
        transform.localScale = originalScale;
    }

    public void Turn()
    {
        StartCoroutine(TurnEffect());
    }

    private System.Collections.IEnumerator TurnEffect()
    {
        float duration = 0.2f; // Total duration of the turn effect
        float halfDuration = duration / 2;
        float elapsedTime = 0f;

        // First half: Rotate to 90 degrees (midpoint)
        while (elapsedTime < halfDuration)
        {
            float angle = Mathf.Lerp(0, 90, elapsedTime / halfDuration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Switch the sprite at the midpoint of the flip
        spriteRenderer.sprite = (spriteRenderer.sprite == backSprite) ? frontSprite : backSprite;

        // Second half: Rotate back to 0 degrees
        elapsedTime = 0f;
        while (elapsedTime < halfDuration)
        {
            float angle = Mathf.Lerp(90, 0, elapsedTime / halfDuration);
            transform.localRotation = Quaternion.Euler(0, angle, 0);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure rotation is fully reset to zero
        transform.localRotation = Quaternion.identity;
    }
}
