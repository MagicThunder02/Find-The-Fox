using UnityEngine;
using UnityEngine.UI;

public class TPRulesManager : MonoBehaviour
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
        return null;
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
