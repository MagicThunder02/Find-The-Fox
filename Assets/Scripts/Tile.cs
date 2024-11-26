using UnityEngine;

public class Tile : MonoBehaviour
{
    public Sprite frontSprite;  // The sprite for the letter side (front)
    public Sprite backSprite;   // The sprite for the back of the tile
    public string letter;       // The letter on the tile
    public SpriteRenderer spriteRenderer; // Reference to the SpriteRenderer

    public void ShowFront()
    {
        spriteRenderer.sprite = frontSprite;
    }

    public void ShowBack()
    {
        spriteRenderer.sprite = backSprite;
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
}
