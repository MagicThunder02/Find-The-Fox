using UnityEngine;

public class TPTile : Tile
{

    [SerializeField]
    private GridManager gridManager;       // Reference to the GridManager

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Show the back sprite at the start
        ShowBack();
    }

    void OnMouseDown()
    {
        // Show the front (reveal the letter)
        ShowFront();

        // gridManager.PlaceTile(this);

    }

}
