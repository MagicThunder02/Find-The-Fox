using UnityEngine;
using UnityEngine.UI;

public class SPRulesManager : MonoBehaviour
{
    public bool gameEnded = false;
    public Button tryAgainButton;
    private GridManager gridManager; // Reference to the GridManager
    private TilePoolManager tilePoolManager; // Reference to the GridManager
    private Tile[,] gameBoard;

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
        string reversedWord = ReverseString(targetWord); // Reverse the target word

        // Iterate over each cell in the grid
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                // Check all 8 directions from the current cell
                Vector2Int[] result = CheckAllDirections(row, col, targetWord, gameBoard) ??
                                      CheckAllDirections(row, col, reversedWord, gameBoard);

                // lose condition
                if (result != null)
                {
                    GameOver(targetWord, result); // Word found, end the game
                    return result; // Return the coordinates of the word
                }
                else
                {
                    // win condition - check if the last cell is placed
                    if (gameBoard[rows - 1, cols - 1] != null)
                    {
                        WinGame();
                        return null;
                    }
                }

            }

        }

        return null; // Word not found
    }

    private Vector2Int[] CheckAllDirections(int startRow, int startCol, string targetWord, Tile[,] gameBoard)
    {
        // Directions: (rowDelta, colDelta)
        Vector2Int[] directions = {
            new Vector2Int(0, 1),   // Right
            new Vector2Int(1, 0),   // Down
            new Vector2Int(1, 1),   // Diagonal down-right
            new Vector2Int(1, -1),  // Diagonal down-left
        };

        foreach (var dir in directions)
        {
            Vector2Int[] result = CheckDirection(startRow, startCol, dir.x, dir.y, targetWord, gameBoard);
            if (result != null)
                return result; // Return coordinates if the word is found in this direction
        }

        return null; // Word not found in any direction
    }

    private Vector2Int[] CheckDirection(int startRow, int startCol, int rowDir, int colDir, string targetWord, Tile[,] gameBoard)
    {
        int rows = gameBoard.GetLength(0);
        int cols = gameBoard.GetLength(1);
        Vector2Int[] coordinates = new Vector2Int[targetWord.Length];

        for (int i = 0; i < targetWord.Length; i++)
        {
            int newRow = startRow + i * rowDir;
            int newCol = startCol + i * colDir;

            // Check bounds
            if (newRow < 0 || newRow >= rows || newCol < 0 || newCol >= cols)
                return null;

            // Check if the gameBoard has the letter we need
            if (gameBoard[newRow, newCol] == null || gameBoard[newRow, newCol].letter != targetWord[i].ToString())
                return null;

            // Record the coordinates
            coordinates[i] = new Vector2Int(newRow, newCol);
        }

        return coordinates; // Return the coordinates of the word
    }

    private string ReverseString(string input)
    {
        char[] charArray = input.ToCharArray();
        System.Array.Reverse(charArray);
        return new string(charArray);
    }

    private void GameOver(string word, Vector2Int[] coordinates)
    {
        gameEnded = true;

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
        // Show the "Try Again" button
        if (tryAgainButton != null)
        {
            tryAgainButton.gameObject.SetActive(true);
        }

        // Additional game-over logic can be added here, e.g., stopping gameplay, showing UI, etc.
    }

    private void WinGame()
    {
        gameEnded = true;

        Debug.Log("Congratulations! You've won the game!");

        // Add your winning logic here (e.g., display a message or animation)
        // Example: Show "You Win" UI
        if (tryAgainButton != null)
        {
            tryAgainButton.gameObject.SetActive(true); // Show the "Try Again" button for restarting
        }
    }



    public void OnTryAgainButtonPressed()
    {
        gameEnded = false;

        Debug.Log("Try Again button pressed!");

        // Reset the game board
        gridManager.ResetBoard();
        tilePoolManager.ScatterTiles();

        tryAgainButton.gameObject.SetActive(false);

    }
}
