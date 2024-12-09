using UnityEngine;

public class TPTile : Tile
{
    [SerializeField]
    private GridManager gridManager; // Reference to the GridManager
    private TPRulesManager tpRulesManager; // Reference to the rules manager component

    private bool isDragging = false; // Track if the tile is being dragged
    private Vector3 offset;          // Offset between the tile's position and the mouse position
    private Camera mainCamera;       // Reference to the main camera

    void Start()
    {
        // Get the SpriteRenderer component
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Show the back sprite at the start
        showBack();

        // Cache the main camera
        mainCamera = Camera.main;

        // Get the GridManager from the scene if not assigned
        if (gridManager == null)
        {
            gridManager = FindAnyObjectByType<GridManager>();
        }
        if (tpRulesManager == null)
        {
            tpRulesManager = FindAnyObjectByType<TPRulesManager>();
        }

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene.");
        }
    }

    void OnMouseDown()
    {
        // Start dragging and calculate the offset
        if (placed)
        {
            return; // Game has ended, no more tiles can be placed
        }
        isDragging = true;
        showFront(); // Show the front of the tile

        offset = transform.position - GetMouseWorldPosition();
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            // Update the tile's position to follow the mouse
            transform.position = GetMouseWorldPosition() + offset;
        }
    }

    void OnMouseUp()
    {
        if (isDragging)
        {
            // Stop dragging
            isDragging = false;

            // Check for grid cell overlap
            tpRulesManager.PlaceTPTile(this);
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Adjust the z-axis to match the camera's distance
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }
}
