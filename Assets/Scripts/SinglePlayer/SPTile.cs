using UnityEngine;

public class SPTile : Tile
{
    private GridManager gridManager;  // Reference to the GridManager component
    private SPRulesManager spRulesManager;  // Reference to the GridManager component

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        if (gridManager == null)
        {
            gridManager = FindAnyObjectByType<GridManager>();
        }
        if (spRulesManager == null)
        {
            spRulesManager = FindAnyObjectByType<SPRulesManager>();
        }

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene.");
        }
    }

    void OnMouseDown()
    {
        spRulesManager.PlaceSPTile(this);
    }

}
