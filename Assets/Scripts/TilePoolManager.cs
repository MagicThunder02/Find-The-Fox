using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;


public class TilePoolManager : MonoBehaviour
{
    // public GameObject SPTilePrefab; // Prefab of the tile object
    // public GameObject TPTilePrefab; // Prefab of the tile object
    public Tile SPTilePrefab; // Prefab of the tile object
    public Tile TPTilePrefab; // Prefab of the tile object
    public Sprite backSprite;       // Sprite for "F" tile
    public Sprite spriteF;       // Sprite for "F" tile
    public Sprite spriteO;       // Sprite for "O" tile
    public Sprite spriteX;       // Sprite for "X" tile

    public Transform poolParent; // Parent object for organization
    public Vector2 gridCenter;   // Center position of the grid
    public Vector2 gridSize = new Vector2(4, 4); // Width and height of the grid (in tiles)
    public float cameraPadding = 1f; // Padding to ensure tiles stay within the camera view

    private List<Tile> tiles = new List<Tile>();
    private string activeScene;

    void Start()
    {
        // Detect the active scene
        activeScene = SceneManager.GetActiveScene().name;


        // Create tiles
        CreateTiles(5, spriteF, backSprite, "F"); // Create 5 "F" tiles
        CreateTiles(6, spriteO, backSprite, "O"); // Create 6 "O" tiles
        CreateTiles(5, spriteX, backSprite, "X"); // Create 5 "X" tiles

        // Scatter tiles above or below the grid
        ScatterTiles();
    }

    // Method to create tiles
    void CreateTiles(int count, Sprite frontSprite, Sprite backSprite, string letter)
    {
        for (int i = 0; i < count; i++)
        {
            if (activeScene == "SinglePlayerScene")
            {
                // Add SPTile script for SinglePlayerScene
                Tile tile = Instantiate(SPTilePrefab, poolParent).GetComponent<SPTile>();
                tile.name = $"{letter} Tile {i + 1}";
                tile.frontSprite = frontSprite;
                tile.backSprite = backSprite;
                tile.letter = letter;
                tiles.Add(tile);

            }
            else
            {
                // Add TPTile script for TwoPlayerScene
                Tile tile = Instantiate(TPTilePrefab, poolParent).GetComponent<TPTile>();
                tile.name = $"{letter} Tile {i + 1}";
                tile.frontSprite = frontSprite;
                tile.backSprite = backSprite;
                tile.letter = letter;
                tiles.Add(tile);

            }


        }
    }

    // Scatter tiles randomly above or below the grid
    public void ScatterTiles()
    {
        Camera mainCamera = Camera.main;

        // Get the camera's visible boundaries in world space
        float cameraHalfHeight = mainCamera.orthographicSize;
        float cameraHalfWidth = cameraHalfHeight * mainCamera.aspect;

        float cameraLeft = mainCamera.transform.position.x - cameraHalfWidth + cameraPadding;
        float cameraRight = mainCamera.transform.position.x + cameraHalfWidth - cameraPadding;

        float gridTop = gridCenter.y + gridSize.y / 2f;
        float gridBottom = gridCenter.y - gridSize.y / 2f;

        foreach (Tile tile in tiles)
        {
            tile.showBack();
            tile.placed = false; // Whether the tile is placed on the grid

            // Randomize position within the camera's horizontal bounds
            float randomX = Random.Range(cameraLeft, cameraRight);

            // Randomly place above or below the grid
            float randomY;
            if (Random.value > 0.5f)
            {
                // Place above the grid
                randomY = Random.Range(gridTop + 1f, gridTop + 3f); // Offset above the grid
            }
            else
            {
                // Place below the grid
                randomY = Random.Range(gridBottom - 3f, gridBottom - 1f); // Offset below the grid
            }

            tile.transform.position = new Vector2(randomX, randomY);
        }
    }
}
