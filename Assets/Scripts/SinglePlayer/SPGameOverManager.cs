using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using TMPro; // Add this for TextMeshProUGUI
using UnityEngine.UI;

public class SPGameOverManager : MonoBehaviour
{
    public GameObject gameOverCanvas; // The canvas with the "Player X Wins" message
    public TextMeshProUGUI gameOverText; // The prefab for the "Player X Wins" text
    public Button playAgainButton;
    public SPRulesManager SPRulesManager; // Reference to the GridManager

    private GridManager gridManager; // Reference to the GridManager
    private TilePoolManager tilePoolManager; // Reference to the GridManager

    public Volume postProcessingVolume;  // Reference to the Post-Processing Volume for blur
    private float blurDuration = 1.5f;      // Duration to apply the blur effect
    private float maxBlurIntensity = 1.5f;  // Maximum blur intensity
    private float uiFadeDuration = 2f;    // Duration for UI to fade in

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
    }

    public void ShowGameOverUI(int playerNumber)
    {
        gameOverCanvas.SetActive(true);

        // Set the win message text
        if (playerNumber == 0)
        {
            gameOverText.text = "You've found a Fox \nGame Over!";
        }
        else
        {
            gameOverText.text = "You won!";
        }

        Debug.Log("Game Over! Player " + playerNumber + " wins!");

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

            float blurValue = Mathf.Lerp(5f, maxBlurIntensity, elapsedTime / blurDuration);
            ApplyBlur(true, blurValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }


    public void OnTryAgainButtonPressed()
    {
        SPRulesManager.gameEnded = false;

        Debug.Log("Try Again button pressed!");

        // Reset the game board
        gridManager.ResetBoard();
        tilePoolManager.ResetTilesBack();

        // Start the transition to hide the Game Over UI and remove the blur
        StartCoroutine(HideGameOverUIWithTransition());
    }


    private IEnumerator HideGameOverUIWithTransition()
    {
        Debug.Log("Hiding Game Over UI with transition...");
        // Fade out the UI (using CanvasGroup alpha)
        float elapsedTime = 0f;
        CanvasGroup canvasGroup = gameOverCanvas.GetComponent<CanvasGroup>();
        while (elapsedTime < uiFadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / uiFadeDuration);
            elapsedTime += Time.deltaTime;

            float blurValue = Mathf.Lerp(maxBlurIntensity, 5f, elapsedTime / blurDuration);
            ApplyBlur(true, blurValue);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        // Hide the Game Over UI after fade out (optional, if needed)
        gameOverCanvas.SetActive(false);
    }

    // Function to apply or remove blur effect
    private void ApplyBlur(bool enable, float intensity = 0f)
    {
        if (postProcessingVolume != null && postProcessingVolume.profile.TryGet<DepthOfField>(out DepthOfField dof))
        {
            dof.active = enable; // Enable or disable the blur effect
            if (enable)
            {
                dof.focusDistance.overrideState = true;
                dof.focusDistance.value = intensity; // Adjust the blur intensity
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
