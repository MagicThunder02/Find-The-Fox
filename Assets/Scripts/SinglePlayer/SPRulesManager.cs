using UnityEngine;

public class SPRulesManager : MonoBehaviour
{
    public Vector2Int[] CheckForWord(string targetWord, Tile[,] gameBoard)
    {
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

                if (result != null)
                {
                    EndGame(gameBoard, targetWord, result); // Word found, end the game
                    return result; // Return the coordinates of the word
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

    private void EndGame(Tile[,] gameBoard, string word, Vector2Int[] coordinates)
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

        // Add additional game-over logic here (e.g., UI, stop gameplay, etc.)
    }

}
