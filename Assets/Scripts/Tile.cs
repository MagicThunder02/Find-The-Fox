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
}
