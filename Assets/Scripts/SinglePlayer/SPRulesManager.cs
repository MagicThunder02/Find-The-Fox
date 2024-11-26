using UnityEngine;

public class RuleManager : MonoBehaviour
{
    // Checks if the "FOX" word has been formed in the grid
    public bool CheckForWin(string[,] board)
    {
        // Check rows and columns for "FOX"
        for (int i = 0; i < 4; i++)
        {
            if (CheckLine(board[i, 0], board[i, 1], board[i, 2])) return true; // Row
            if (CheckLine(board[0, i], board[1, i], board[2, i])) return true; // Column
        }

        // Check diagonals
        if (CheckLine(board[0, 0], board[1, 1], board[2, 2])) return true;
        if (CheckLine(board[0, 3], board[1, 2], board[2, 1])) return true;

        return false; // No win found
    }

    // Helper method to check if a line forms "FOX"
    private bool CheckLine(string a, string b, string c)
    {
        return a == "F" && b == "O" && c == "X";
    }
}
