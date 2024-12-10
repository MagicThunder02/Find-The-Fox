using UnityEngine;

public class TPTile : Tile
{
    [SerializeField]
    private GridManager gridManager; // Reference to the GridManager
    private TPRulesManager tpRulesManager; // Reference to the rules manager component

    private bool isDragging = false; // Track if the tile is being dragged
    private Vector3 offset;          // Offset between the tile's position and the mouse position
    private Vector3 originalPosition; // Original position of the tile before dragging
    private Camera mainCamera;       // Reference to the main camera

    void Start()
    {
        // Show the back sprite at the start
        // ShowBack();

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

        // Save the current position as the original position
        originalPosition = transform.position;

        ShowFront(); // Show the front of the tile

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

            int placementResult = tpRulesManager.PlaceTPTile(this);

            if (placementResult == -2)
            {
                // If placement is invalid, return the tile to its original position
                StartCoroutine(ReturnToOriginalPosition());
            }
        }
    }

    private Vector3 GetMouseWorldPosition()
    {
        // Get the mouse position in world space
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 10f; // Adjust the z-axis to match the camera's distance
        return mainCamera.ScreenToWorldPoint(mousePosition);
    }

    private System.Collections.IEnumerator ReturnToOriginalPosition()
    {
        float duration = 0.1f; // Duration of the animation
        float elapsedTime = 0f;
        Vector3 startingPosition = transform.position;

        while (elapsedTime < duration)
        {
            // Smoothly animate the tile back to its original position
            transform.position = Vector3.Lerp(startingPosition, originalPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Ensure the tile snaps exactly back to its original position
        transform.position = originalPosition;
    }
}
