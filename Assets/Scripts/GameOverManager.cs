using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro; // Add this for TextMeshProUGUI
using UnityEngine.UI;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas; // The canvas with the "Player X Wins" message
    public TextMeshProUGUI gameOverText; // The prefab for the "Player X Wins" text
    public Button playAgainButton;
    public Button mainMenuButton;
    public Button viewBoardButton;
    public TPRulesManager TPRulesManager; // Reference to the GridManager

    private GridManager gridManager; // Reference to the GridManager
    private TilePoolManager tilePoolManager; // Reference to the GridManager

    public Volume postProcessingVolume;  // Reference to the Post-Processing Volume for blur
    public float blurDuration = 0.5f;      // Duration to apply the blur effect
    public float maxBlurIntensity = 5f;  // Maximum blur intensity
    public float uiFadeDuration = 1f;    // Duration for UI to fade in

    private void Start()
    {
        gridManager = FindAnyObjectByType<GridManager>();
        tilePoolManager = FindAnyObjectByType<TilePoolManager>();

        if (gridManager == null)
        {
            Debug.LogError("GridManager not found in the scene.");
        }

        if (tilePoolManager == null)
        {
            Debug.LogError("TilePoolManager not found in the scene.");
        }

        // Initially hide the game over UI
        gameOverCanvas.SetActive(false);

        // Start with no blur effect and UI invisible
        SetBlurIntensity(0f);
        SetUIAlpha(0f);
    }

    public void ShowGameOverUI(int playerNumber)
    {
        gameOverCanvas.SetActive(true);

        // Set the win message text
        if (playerNumber == 0)
        {
            gameOverText.text = "It's a draw!";
        }
        else
        {
            gameOverText.text = "Player " + playerNumber + " Wins!";
        }

        // Start the transition (fade in UI and apply blur effect)
        StartCoroutine(ShowGameOverUIWithTransition());
    }

    private IEnumerator ShowGameOverUIWithTransition()
    {
        // Fade in the UI (using CanvasGroup alpha)
        float elapsedTime = 0f;
        CanvasGroup canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
        while (elapsedTime < uiFadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / uiFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 1f;

        // Gradually apply blur effect
        elapsedTime = 0f;
        while (elapsedTime < blurDuration)
        {
            float blurValue = Mathf.Lerp(0f, maxBlurIntensity, elapsedTime / blurDuration);
            SetBlurIntensity(blurValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetBlurIntensity(maxBlurIntensity);
    }

    public void OnTryAgainButtonPressed()
    {
        TPRulesManager.gameEnded = false;

        Debug.Log("Try Again button pressed!");

        // Reset the game board
        gridManager.ResetBoard();
        tilePoolManager.ResetTilesBack();

        // Start the transition to hide the Game Over UI and remove the blur
        StartCoroutine(HideGameOverUIWithTransition());
    }

    private IEnumerator HideGameOverUIWithTransition()
    {
        // Fade out the UI (using CanvasGroup alpha)
        float elapsedTime = 0f;
        CanvasGroup canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
        while (elapsedTime < uiFadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / uiFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = 0f;

        // Gradually remove blur effect
        elapsedTime = 0f;
        while (elapsedTime < blurDuration)
        {
            float blurValue = Mathf.Lerp(maxBlurIntensity, 0f, elapsedTime / blurDuration);
            SetBlurIntensity(blurValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        SetBlurIntensity(0f);

        // Hide the Game Over UI after fade out (optional, if needed)
        gameOverCanvas.SetActive(false);
    }

    // Function to set blur intensity (using Post-Processing Volume)
    private void SetBlurIntensity(float intensity)
    {
        if (postProcessingVolume != null)
        {
            if (postProcessingVolume.profile.TryGet<DepthOfField>(out DepthOfField dof))
            {
                dof.active = true;
                dof.focusDistance.value = intensity; // Adjust blur effect based on intensity
            }
        }
    }

    // Function to set the UI alpha (fade in/out using CanvasGroup)
    private void SetUIAlpha(float alpha)
    {
        CanvasGroup canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = alpha;
        }
    }
}
