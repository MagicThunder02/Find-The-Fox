using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // The prefab for your grid cell
    public int gridWidth = 4; // Width of the grid
    public int gridHeight = 4; // Height of the grid

    public SPRulesManager SPRulesManager; // Reference to the rules manager component
    public TPRulesManager TPRulesManager; // Reference to the rules manager component

    public Cell[,] grid; // 2D array to store grid object references
    public Tile[,] gameBoard; // 2D array to store placed tiles (both SPTile and TPTile)

    void Awake()
    {
        Debug.Log("GridManager Awake");
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

                // CreateTextObject($"{y}, {x}", worldPosition);
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



    public void ResetBoard()
    {
        // Clear the game board array
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                gameBoard[row, col] = null;
            }
        }


    }
}


