using UnityEngine;

public class SPTile : Tile
{
    private GridManager gridManager;  // Reference to the GridManager component

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();
        gridManager = FindAnyObjectByType<GridManager>();

        // Show the back sprite at the start
        showBack();
    }

    void OnMouseDown()
    {

        gridManager.PlaceSPTile(this);

    }

}
