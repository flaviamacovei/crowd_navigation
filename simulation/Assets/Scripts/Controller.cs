using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class PlayerControl : MonoBehaviour
{
    public GameObject player;
    float objectRadius;
    private Color playerColour = Color.red;
    private Color npcColour = Color.white;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Vector2 objectSize = player.GetComponent<Renderer>().bounds.size;
        objectRadius = Math.Max(objectSize.x, objectSize.y);

        // create boundary
        Boundary.GetInstance().CreateBoundary(objectRadius);

        Bounds spawnBounds = Boundary.GetInstance().GetSpawningBounds();
        Vector2 spawnCentre = spawnBounds.center;
        Vector2 spawnSize = (Vector2)spawnBounds.size - new Vector2(objectRadius, objectRadius);

        List<Vector2> positions = GetSpawnPositions(objectRadius, new Bounds(spawnCentre, spawnSize));
        int playerIndex = UnityEngine.Random.Range(0, positions.Count);
        Vector2 playerPosition = positions[playerIndex];
        positions.RemoveAt(playerIndex);

        // npcs have white colour
        player.GetComponent<SpriteRenderer>().color = npcColour;
        ObjectPool.GetInstance().PlaceObjects(player, positions);

        // player has red colour
        player.GetComponent<SpriteRenderer>().color = playerColour;
        Player.GetInstance().PlacePlayer(player, playerPosition);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private List<Vector2> GetSpawnPositions(float radius, Bounds spawnBounds, int numSamplesBeforeRejection = 30)
    {
        Vector2 spawnSize = spawnBounds.size;
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(spawnSize.x / cellSize), Mathf.CeilToInt(spawnSize.y / cellSize)];
        Vector2 offset = new Vector2(spawnBounds.min.x - spawnBounds.center.x, spawnBounds.min.y - spawnBounds.center.y);
        List<Vector2> points = new List<Vector2>();
        List<Vector2> spawnPoints = new List<Vector2>();

        spawnPoints.Add(spawnSize / 2);
        while (spawnPoints.Count > 0)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector2 spawnCentre = spawnPoints[spawnIndex];

            bool candidateAccepted = false;

            for (int i = 0; i < numSamplesBeforeRejection; i++)
            {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                Vector2 direction = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCentre + direction * UnityEngine.Random.Range(radius, 2 * radius);

                if (IsValid(candidate, spawnSize, cellSize, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }

        // offset points by spawnBounds offset
        List<Vector2> offsetPoints = new List<Vector2>();
        for (int i = 0; i < points.Count; i++)
        {
            offsetPoints.Add(points[i] + offset);
        }
        return offsetPoints;
    }

    private bool IsValid(Vector2 candidate, Vector2 spawnSize, float cellSize,  List<Vector2> points, int[,] grid)
    {
        // if (candidate.x >= spawnSize.x * -0.5f && candidate.x < spawnSize.x * 0.5f && candidate.y >= spawnSize.y * -0.5f && candidate.y < spawnSize.y * 0.5f)
        if (candidate.x >= 0 && candidate.x < spawnSize.x && candidate.y >= 0 && candidate.y < spawnSize.y)
        {
            float radius = cellSize * Mathf.Sqrt(2);
            
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);

            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float suqaredDistance = (candidate - points[pointIndex]).sqrMagnitude;
                        if (suqaredDistance < radius*radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
