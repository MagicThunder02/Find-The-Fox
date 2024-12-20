using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;

public class TPRulesManager : MonoBehaviour
{
    public bool gameEnded = false;
    public TPGameOverManager GameOverManager; // Riferimento allo script del blur
    public LayerMask gridLayerMask;

    private GridManager gridManager; // Reference to the GridManager
    private TilePoolManager tilePoolManager; // Reference to the GridManager
    private Tile[,] gameBoard;

    private int playerTurn = 1; // Player 1 starts the game

    void Start()
    {
        // Get the GridManager from the scene
        gridManager = FindAnyObjectByType<GridManager>();
        tilePoolManager = FindAnyObjectByType<TilePoolManager>();

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene.");
        }
        else
        {
            gameBoard = gridManager.gameBoard;
        }

        if (tilePoolManager == null)
        {
            Debug.LogError("TilePoolManager not found in the scene.");
        }
    }

    public Vector2Int[] CheckForWord(string targetWord)
    {
        if (gameBoard == null)
        {
            Debug.LogError("Game board is null.");
            return null;
        }

        int rows = gameBoard.GetLength(0);
        int cols = gameBoard.GetLength(1);

        // Find all 'F' tiles
        List<Vector2Int> fTiles = new List<Vector2Int>();
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                Tile tile = gameBoard[row, col];
                if (tile != null && tile.letter == "F")
                {
                    fTiles.Add(new Vector2Int(row, col));
                }
            }
        }

        // Directions: (rowDelta, colDelta)
        Vector2Int[] directions = {
        new Vector2Int(0, 1),   // Right
        new Vector2Int(0, -1),  // Left
        new Vector2Int(1, 0),   // Down
        new Vector2Int(-1, 0),  // Up
        new Vector2Int(1, 1),   // Diagonal down-right
        new Vector2Int(1, -1),  // Diagonal down-left
        new Vector2Int(-1, 1),  // Diagonal up-right
        new Vector2Int(-1, -1)  // Diagonal up-left
        };

        // Check each F tile
        foreach (Vector2Int fTile in fTiles)
        {
            foreach (Vector2Int direction in directions)
            {
                if (IsDiagonal(direction) && !(getDiagonalLength(fTile, direction) > 2))
                {
                    continue; // Skip short diagonals
                }

                Vector2Int[] coordinates = CheckDirectionForWord(fTile, direction, targetWord);
                if (coordinates != null)
                {
                    GameOver(1, targetWord, coordinates); // Word found, end the game
                    return coordinates; // Return the coordinates of the word
                }
            }
        }

        return null; // Word not found
    }

    private Vector2Int[] CheckDirectionForWord(Vector2Int start, Vector2Int direction, string targetWord)
    {
        int rows = gameBoard.GetLength(0);
        int cols = gameBoard.GetLength(1);
        Vector2Int[] coordinates = new Vector2Int[targetWord.Length];

        for (int i = 0; i < targetWord.Length; i++)
        {
            int row = start.x + i * direction.x;
            int col = start.y + i * direction.y;

            if (IsDiagonal(direction))
            {
                int diagonalLength = getDiagonalLength(start, direction);
                if (diagonalLength > 2)
                {
                    // Debug.Log($"Diagonal wraparound '{row}' '{col}' 'diaglen' '{diagonalLength}'");

                    if (row < 0 || row >= rows || col < 0 || col >= cols)
                    {
                        row = row - (diagonalLength) * direction.x;
                        col = col - (diagonalLength) * direction.y;
                    }

                    // Debug.Log($"Diagonal wraparound 2 '{row}' '{col}'");


                }

            }


            // Wrap diagonally to stay within bounds
            row = (row % rows + rows) % rows;
            col = (col % cols + cols) % cols;



            // Check if the tile matches the required letter
            Tile tile = gameBoard[row, col];
            if (tile == null || tile.letter != targetWord[i].ToString())
            {
                return null; // Sequence doesn't match
            }

            // Record the coordinate
            coordinates[i] = new Vector2Int(row, col);
        }

        return coordinates; // Sequence matches
    }

    private bool IsDiagonal(Vector2Int direction)
    {
        // A diagonal direction has both x and y changes
        return Mathf.Abs(direction.x) == 1 && Mathf.Abs(direction.y) == 1;
    }

    private int getDiagonalLength(Vector2Int start, Vector2Int direction)
    {
        int rows = gameBoard.GetLength(0);
        int cols = gameBoard.GetLength(1);

        // Calculate steps in the positive direction
        int positiveStepsRow = direction.x > 0 ? rows - 1 - start.x : start.x;
        int positiveStepsCol = direction.y > 0 ? cols - 1 - start.y : start.y;
        int positiveSteps = Mathf.Min(positiveStepsRow, positiveStepsCol);

        // Calculate steps in the negative direction
        int negativeStepsRow = direction.x > 0 ? start.x : rows - 1 - start.x;
        int negativeStepsCol = direction.y > 0 ? start.y : cols - 1 - start.y;
        int negativeSteps = Mathf.Min(negativeStepsRow, negativeStepsCol);

        // Total length is steps in both directions plus the starting tile
        return positiveSteps + negativeSteps + 1;
    }

    public int PlaceTPTile(TPTile tile)
    {
        if (gameEnded || tile.placed)
        {
            return 0; // Game has ended, no more tiles can be placed
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

                    CheckForWord("FOX"); // Check for words after placing the tile

                    playerTurn = playerTurn == 1 ? 2 : 1; // Switch player turn 

                    // Check if all tiles are placed
                    bool allTilesPlaced = true;
                    for (int row = 0; row < gameBoard.GetLength(0); row++)
                    {
                        for (int col = 0; col < gameBoard.GetLength(1); col++)
                        {
                            if (gameBoard[row, col] == null)
                            {
                                allTilesPlaced = false;
                                break;
                            }
                        }
                        if (!allTilesPlaced)
                        {
                            break;
                        }
                    }

                    if (allTilesPlaced)
                    {
                        GameOver(0, "FOX", null); // All tiles placed, end the game with a draw
                    }

                    return 1;

                }
                else
                {
                    Debug.Log("Cell is already occupied. Tile not placed.");
                    return -2;
                }
            }
        }
        else
        {
            Debug.Log("No grid cell detected. Tile remains at its position.");
            return -1;
        }
        return 0;
    }


    private void GameOver(int state, string word, Vector2Int[] coordinates)
    {
        gameEnded = true;

        if (state == 1)
        {
            Debug.Log($"Game Over! The word '{word}' was found at:");
            foreach (var coord in coordinates)
            {
                Debug.Log($"({coord.x}, {coord.y})");

                // Trigger the shine effect on the corresponding tile
                Tile tile = gameBoard[coord.x, coord.y];
                if (tile != null)
                {
                    tile.Shine();
                }
            }

            StartCoroutine(ShowGameOverWithDelay(playerTurn));
        }
        else
        {
            Debug.Log("It's a draw!");
            StartCoroutine(ShowGameOverWithDelay(0));
        }

        // Additional game-over logic can be added here, e.g., stopping gameplay, showing UI, etc.
    }

    private IEnumerator ShowGameOverWithDelay(int playerTurn)
    {
        yield return new WaitForSeconds(0.5f);
        GameOverManager.ShowGameOverUI(playerTurn);
        yield return null;
    }

}
