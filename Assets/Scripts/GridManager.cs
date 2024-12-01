using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // The prefab for your grid cell
    public int gridWidth = 4; // Width of the grid
    public int gridHeight = 4; // Height of the grid

    public LayerMask gridLayerMask;

    public SPRulesManager SPRulesManager; // Reference to the rules manager component
    public TPRulesManager TPRulesManager; // Reference to the rules manager component

    private Cell[,] grid; // 2D array to store grid object references
    public Tile[,] gameBoard; // 2D array to store placed tiles (both SPTile and TPTile)

    void Awake()
    {
        grid = new Cell[gridHeight, gridWidth]; // Initialize the game board
        gameBoard = new Tile[gridHeight, gridWidth]; // Initialize the game board
        CreateGrid(); // Create the grid layout
    }

    // Create the grid cells
    private void CreateGrid()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        float gridWidthInWorldSpace = gridWidth * cellPrefab.transform.localScale.x;
        float gridHeightInWorldSpace = gridHeight * cellPrefab.transform.localScale.y;
        float offsetX = cameraPosition.x - (gridWidthInWorldSpace / 2f);
        float offsetY = cameraPosition.y + (gridHeightInWorldSpace / 2f);

        for (int x = 0; x < gridWidth; x++)
        {
            for (int y = 0; y < gridHeight; y++)
            {
                Vector3 worldPosition = new Vector3(
                    offsetX + (x * cellPrefab.transform.localScale.x) + cellPrefab.transform.localScale.x / 2,
                    offsetY - (y * cellPrefab.transform.localScale.y) - cellPrefab.transform.localScale.y / 2,
                    0);

                Cell cell = Instantiate(cellPrefab, worldPosition, Quaternion.identity).GetComponent<Cell>();
                cell.transform.parent = transform;

                cell.name = $"Cell ({y}, {x})";

                // inverting x and y to match the grid layout
                cell.gridX = y;
                cell.gridY = x;
                grid[y, x] = cell; // Initially, cells are empty (no tiles placed)

                CreateTextObject($"{y}, {x}", worldPosition);
            }
        }
    }

    // Create labels for debugging (e.g., cell coordinates)
    private void CreateTextObject(string text, Vector3 position)
    {
        GameObject textObject = new GameObject("CellLabel");
        TextMeshPro textMeshPro = textObject.AddComponent<TextMeshPro>();
        textMeshPro.text = text;
        textMeshPro.fontSize = 4;
        textMeshPro.alignment = TextAlignmentOptions.Center;
        textMeshPro.color = Color.black;
        textObject.transform.position = position + new Vector3(0, 0, -0.1f);
    }

    // Place any tile (SPTile or TPTile) on the board
    public bool PlaceSPTile(Tile tile)
    {
        if (SPRulesManager.gameEnded || tile.placed)
        {
            return false; // Game has ended, no more tiles can be placed
        }

        tile.placed = true; // Mark the tile as placed
        tile.showFront(); // Show the front of the tile

        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                if (gameBoard[row, col] == null) // Find the first empty cell
                {
                    // Get the world position of the first empty cell
                    // Debug.Log("Placing tile at grid: " + grid[row, col]);
                    Vector3 worldPosition = grid[row, col].transform.position;

                    // Place the tile at this position
                    tile.transform.position = worldPosition + new Vector3(0, 0, -0.1f); // Slightly above the cell
                    tile.showFront(); // Show the front of the tile

                    // Update the game board array with the placed tile
                    gameBoard[row, col] = tile; // Store the tile at the correct position

                    SPRulesManager.CheckForWord("FOX"); // Check for words after placing the tile

                    return true; // Tile placed successfully
                }
            }
        }

        return false; // No empty spots were found
    }

    public void ResetBoard()
    {
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                gameBoard[row, col] = null;
            }
        }
        Debug.Log(gameBoard[0, 0]);
    }


    public bool PlaceTPTile(TPTile tile)
    {
        if (TPRulesManager.gameEnded || tile.placed)
        {
            return false; // Game has ended, no more tiles can be placed
        }


        // Get the world position of the tile
        Vector3 worldPosition = tile.transform.position;

        Collider2D detectedCollider = Physics2D.OverlapPoint(worldPosition, gridLayerMask);
        // Debug.Log($"Checking for collider at position {worldPosition}.");

        if (detectedCollider != null)
        {
            // Debug.Log($"Detected collider: {detectedCollider.name}");
            Cell detectedCell = detectedCollider.GetComponent<Cell>();

            if (detectedCell != null)
            {
                // Check if the cell is available
                if (gameBoard[detectedCell.gridX, detectedCell.gridY] == null)
                {
                    // Place the tile in the grid
                    gameBoard[detectedCell.gridX, detectedCell.gridY] = tile;

                    // Snap the tile to the cell's position
                    tile.transform.position = detectedCell.transform.position;
                    tile.placed = true;

                    TPRulesManager.CheckForWord("FOX"); // Check for words after placing the tile
                    return true;

                }
                else
                {
                    Debug.Log("Cell is already occupied. Tile not placed.");
                }
            }
        }
        else
        {
            Debug.Log("No grid cell detected. Tile remains at its position.");
            return false;
        }
        return false;
    }
}

