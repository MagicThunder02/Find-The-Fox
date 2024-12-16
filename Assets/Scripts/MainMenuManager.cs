using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    public void PlayMainMenu()
    {
        SceneManager.LoadScene("MainMenuScene"); // Replace with the actual name of your two-player scene
    }
    // This method will load the Singleplayer scene
    public void PlaySingleplayer()
    {
        SceneManager.LoadScene("SingleplayerScene"); // Replace with the actual name of your singleplayer scene
    }

    // This method will load the Two-Player scene
    public void PlayTwoPlayer()
    {
        SceneManager.LoadScene("TwoPlayersScene"); // Replace with the actual name of your two-player scene
    }

    // This method will quit the game
    public void QuitGame()
    {
        Debug.Log("Quit Game"); // Works in the editor
        Application.Quit();    // Only works in builds
    }
}
