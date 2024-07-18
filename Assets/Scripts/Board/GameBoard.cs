using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Unity.VisualScripting;
using UnityEngine;

public class GameBoard : MonoBehaviour
{
    [SerializeField]
    Transform ground = default;

    [SerializeField]
    GameTile tilePrefab = default;

    [SerializeField]
    GameTile[] boardTiles;

    [SerializeField]
    Texture2D gridTexture = default;

    Queue<GameTile> searchFrontier = new Queue<GameTile>();

    public Vector2Int size;

    GameTileContentFactory tileContentFactory;


    List<GameTile> spawnPoints = new List<GameTile>();

    public GameTile GetSpawnPoint(int index)
    {
        return spawnPoints[index];
    }

    public int SpawnPointCount => spawnPoints.Count;

    public void Initialize(Vector2Int size, GameTileContentFactory factory)
    {
        this.size = size;
        tileContentFactory = factory;

        ground.localScale = new Vector3(size.x, size.y, 1f);

        boardTiles = new GameTile[size.x * size.y];

        Vector2 offset = new Vector2((size.x - 1) * 0.5f, (size.y - 1) * 0.5f);
        for (int i = 0, y = 0; y < size.y; ++y)
        {
            for (int x = 0; x < size.x; ++x, ++i)
            {
                GameTile tile = Instantiate(tilePrefab);
                boardTiles[i] = tile;

                tile.transform.SetParent(transform, false);
                tile.transform.localPosition = new Vector3(x - offset.x, y - offset.y, -0.01f);

                tile.IsAlternative = (x & 1) == 0;
                
                if ((y & 1) == 0)
                {
                    tile.IsAlternative = !tile.IsAlternative;
                }

                if (x > 0)
                {
                    GameTile.MakeEastWestNeighbors(tile, boardTiles[i - 1]);
                }
                if (y > 0)
                {
                    GameTile.MakeNorthSouthNeighbors(tile, boardTiles[i - size.x]);
                }
                tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
            }
        }
        ToggleDestination(boardTiles[boardTiles.Length / 2]);
        ToggleSpawnPoint(boardTiles[0]);
        ShowPaths = false;
    }


    bool showGrid, showPaths;


    public bool ShowPaths
    {
        get => showPaths;
        set
        {
            showPaths = value;
            if (showPaths)
            {
                foreach (GameTile tile in boardTiles)
                {
                    tile.ShowPath();
                }
            }
            else
            {
                foreach (GameTile tile in boardTiles)
                {
                    tile.HidePath();
                }
            }
        }
    }

    public bool ShowGrid
    {
        get => showGrid;
        set
        {
            showGrid = value;
            Material m = ground.gameObject.GetComponent<MeshRenderer>().material;
            if (showGrid)
            {
                m.mainTexture = gridTexture;
                m.mainTexture.wrapMode = TextureWrapMode.Repeat;
                m.SetTextureScale("_MainTex", size);
            }
            else
            {
                m.mainTexture = null;
            }
        }
    }

    public void ToggleDestination(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Destination)
        {
            tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
            if (!FindPaths())
            {
                tile.Content = tileContentFactory.Get(GameTileContentType.Destination);
                FindPaths();
            }
        }
        else if(tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = tileContentFactory.Get(GameTileContentType.Destination);
            FindPaths();
        }
    }

    public void ToggleWall(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
            FindPaths();
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = tileContentFactory.Get(GameTileContentType.Wall);
            tile.BecomeWall();
            if (!FindPaths())
            {
                tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
    }

    List<GameTileContent> updatingContent = new List<GameTileContent>();
    public void GameUpdate()
    {
        for (int i = 0; i < updatingContent.Count; i++)
        {
            updatingContent[i].GameUpdate();
        }
    }
    public void ToggleTower(GameTile tile, TowerType towerType)
    {
        if (tile.Content.Type == GameTileContentType.Tower)
        {
            updatingContent.Remove(tile.Content);
            if (((Tower)tile.Content).TowerType == towerType)
            {
                tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
            else
            {
                tile.Content = tileContentFactory.Get(towerType);
                updatingContent.Add(tile.Content);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            tile.Content = tileContentFactory.Get(towerType);
            if (FindPaths())
            {
                updatingContent.Add(tile.Content);
            }
            else
            {
                tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
                FindPaths();
            }
        }
        else if (tile.Content.Type == GameTileContentType.Wall)
        {
            tile.Content = tileContentFactory.Get(towerType);
            updatingContent.Add(tile.Content);
        }
    }

    public void ToggleSpawnPoint(GameTile tile)
    {
        if (tile.Content.Type == GameTileContentType.SpawnPoint)
        {
            if (spawnPoints.Count > 1)
            {
                spawnPoints.Remove(tile);
                tile.Content = tileContentFactory.Get(GameTileContentType.Empty);
            }
        }
        else if (tile.Content.Type == GameTileContentType.Empty)
        {
            spawnPoints.Add(tile);
            tile.Content = tileContentFactory.Get(GameTileContentType.SpawnPoint);
        }
        FindPaths();
    }

    public bool FindPaths()
    {
        foreach (GameTile tileL in boardTiles)
        {
            if (tileL.Content.Type == GameTileContentType.Destination)
            {
                tileL.BecomeDestination();
                searchFrontier.Enqueue(tileL);
            }
            else
            {
                tileL.ClearPath();
            }
        }

        if (searchFrontier.Count == 0)
            return false;

        while (searchFrontier.Count > 0)
        {
            GameTile tile = searchFrontier.Dequeue();
            if (tile != null)
            {
                if (tile.IsAlternative)
                {
                    EnqueueInFrontier(tile.GrowPathNorth());
                    EnqueueInFrontier(tile.GrowPathSouth());
                    EnqueueInFrontier(tile.GrowPathEast());
                    EnqueueInFrontier(tile.GrowPathWest());
                }
                else
                {
                    EnqueueInFrontier(tile.GrowPathWest());
                    EnqueueInFrontier(tile.GrowPathEast());
                    EnqueueInFrontier(tile.GrowPathSouth());
                    EnqueueInFrontier(tile.GrowPathNorth());
                }
            }
        }

        foreach (GameTile tile in boardTiles)
        {
            if (tile.Content.Type == GameTileContentType.Wall)
                continue;
            if (!tile.HasPath)
            {
                return false;
            }
        }

        if (showPaths)
        {
            foreach (GameTile tile in boardTiles)
            {
                tile.ShowPath();
            }
        }
        return true;
    }

    void EnqueueInFrontier(GameTile tile)
    {
        if (tile != null)
            searchFrontier.Enqueue(tile);
    }

    public GameTile GetTile(Ray ray)
    {
        if (Physics.Raycast(ray, out RaycastHit hit, float.MaxValue, 1))
        {
            int x = (int)(hit.point.x + size.x * 0.5f);
            int y = (int)(hit.point.y + size.y * 0.5f);
            if (x >= 0 && x < size.x && y >= 0 && y < size.y)
            {
                return boardTiles[x + y * size.x];
            }
        }
        return null;
    }

}
