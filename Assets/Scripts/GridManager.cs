using UnityEngine;
using TMPro;

public class GridManager : MonoBehaviour
{
    public GameObject cellPrefab; // The prefab for your grid cell
    public int gridWidth = 4; // Width of the grid
    public int gridHeight = 4; // Height of the grid

    private GameObject[,] grid; // 2D array to store grid object references
    private Tile[,] gameBoard; // 2D array to store placed tiles (both SPTile and TPTile)

    void Awake()
    {
        grid = new GameObject[gridHeight, gridWidth]; // Initialize the game board
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

                GameObject cell = Instantiate(cellPrefab, worldPosition, Quaternion.identity);
                cell.transform.parent = transform;

                grid[x, y] = cell; // Initially, cells are empty (no tiles placed)

                CreateTextObject($"{x}, {y}", worldPosition);
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
        for (int row = 0; row < gridHeight; row++)
        {
            for (int col = 0; col < gridWidth; col++)
            {
                if (gameBoard[row, col] == null) // Find the first empty cell
                {
                    // Get the world position of the first empty cell
                    Debug.Log("Placing tile at grid: " + grid[row, col]);
                    Vector3 worldPosition = grid[row, col].transform.position;

                    // Place the tile at this position
                    tile.transform.position = worldPosition;
                    tile.ShowFront(); // Show the front of the tile

                    // Update the game board array with the placed tile
                    gameBoard[row, col] = tile; // Store the tile at the correct position

                    return true; // Tile placed successfully
                }
            }
        }

        return false; // No empty spots were found
    }
}
