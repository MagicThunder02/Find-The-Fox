using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

public class TilePoolManager : MonoBehaviour
{
    [Header("Tile Prefabs")]
    public Tile SPTilePrefab; // Prefab for SinglePlayerScene tiles
    public Tile TPTilePrefab; // Prefab for TwoPlayerScene tiles

    [Header("Tile Sprites")]
    public Sprite backSprite;
    public Sprite spriteF;
    public Sprite spriteO;
    public Sprite spriteX;

    [Header("Grid Settings")]
    public Transform poolParent;
    public Vector2 gridCenter;
    public Vector2 gridSize = new Vector2(4, 4);
    public float cameraPadding = 1f;

    private List<Tile> tiles = new List<Tile>();
    private string activeScene;

    private void Start()
    {
        activeScene = SceneManager.GetActiveScene().name;

        // Create tiles for each letter
        CreateTiles(5, spriteF, "F");
        CreateTiles(6, spriteO, "O");
        CreateTiles(5, spriteX, "X");

        // Scatter tiles after placing them in the center
        ScatterTiles();
    }

    /// <summary>
    /// Creates a specified number of tiles with given properties.
    /// </summary>
    private void CreateTiles(int count, Sprite frontSprite, string letter)
    {
        for (int i = 0; i < count; i++)
        {
            // Determine which prefab to use based on the active scene
            Tile tilePrefab = activeScene == "SinglePlayerScene" ? SPTilePrefab : TPTilePrefab;

            // Instantiate the tile and configure its properties
            Tile tile = Instantiate(tilePrefab, poolParent);
            tile.name = $"{letter} Tile {i + 1}";
            tile.frontSprite = frontSprite;
            tile.backSprite = backSprite;
            tile.letter = letter;
            tile.spriteRenderer.sprite = backSprite;
            tile.transform.position = gridCenter; // Start at the grid center
            tiles.Add(tile);
        }
    }

    /// <summary>
    /// Starts scattering tiles animation.
    /// </summary>
    public void ScatterTiles()
    {
        StartCoroutine(ScatterTilesAnimation());
    }

    /// <summary>
    /// Resets all tiles to their back side and scatters them again.
    /// </summary>
    public void ResetTilesBack()
    {
        StartCoroutine(ResetTilesBackAnimation());
    }

    /// <summary>
    /// Handles the full reset animation: turn tiles, move to center, scatter.
    /// </summary>
    private IEnumerator ResetTilesBackAnimation()
    {
        yield return StartCoroutine(ResetBoardSequentially());
        yield return StartCoroutine(MoveTilesToCenter());

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(ScatterTilesAnimation());
    }

    /// <summary>
    /// Animates a tile's movement to a target position.
    /// </summary>
    private IEnumerator AnimateTileMovement(Tile tile, Vector2 targetPosition)
    {
        float duration = 0.3f;
        float elapsedTime = 0f;
        Vector2 startingPosition = tile.transform.position;

        while (elapsedTime < duration)
        {
            tile.transform.position = Vector2.Lerp(startingPosition, targetPosition, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        tile.transform.position = targetPosition;
    }

    /// <summary>
    /// Turns all tiles back sequentially.
    /// </summary>
    private IEnumerator ResetBoardSequentially()
    {
        foreach (Tile tile in tiles)
        {
            tile.ShowBack();
            yield return new WaitForSeconds(0.05f);
        }
    }

    /// <summary>
    /// Moves all tiles to the grid center sequentially.
    /// </summary>
    private IEnumerator MoveTilesToCenter()
    {
        foreach (Tile tile in tiles)
        {
            StartCoroutine(AnimateTileMovement(tile, gridCenter));
            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Scatters tiles randomly within the camera bounds.
    /// </summary>
    private IEnumerator ScatterTilesAnimation()
    {
        Camera mainCamera = Camera.main;

        // Calculate camera boundaries
        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;
        float cameraLeft = mainCamera.transform.position.x - cameraHalfWidth + cameraPadding;
        float cameraRight = mainCamera.transform.position.x + cameraHalfWidth - cameraPadding;
        float gridTop = gridCenter.y + gridSize.y / 2f;
        float gridBottom = gridCenter.y - gridSize.y / 2f;

        foreach (Tile tile in tiles)
        {
            tile.placed = false;

            // Determine random position above or below the grid
            float randomX = Random.Range(cameraLeft, cameraRight);
            float randomY = Random.value > 0.5f
                ? Random.Range(gridTop + 1f, gridTop + 3f) // Above the grid
                : Random.Range(gridBottom - 3f, gridBottom - 1f); // Below the grid

            Vector2 targetPosition = new Vector2(randomX, randomY);
            StartCoroutine(AnimateTileMovement(tile, targetPosition));
            yield return new WaitForSeconds(0.1f);
        }
    }



}
