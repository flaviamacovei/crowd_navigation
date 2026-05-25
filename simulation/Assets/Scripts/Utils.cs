using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using Unity.Mathematics;

public static class Utils
{
    public static List<Vector2> GetSpawnPositions(float radius, Bounds spawnBounds, int numSamplesBeforeRejection = 30)
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

    private static bool IsValid(Vector2 candidate, Vector2 spawnSize, float cellSize,  List<Vector2> points, int[,] grid)
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

    public static Vector2 GetClosestPointOnTarget2D(Vector2[] target, Vector2 point)
    {
        // convert to float3
        float3 start3D = new float3(
            target[0].x,
            target[0].y,
            0
        );
        float3 stop3D = new float3(
            target[1].x,
            target[1].y,
            0
        );
        float3[] target3D = new[] {start3D, stop3D};
        float3 point3D = new float3
        (
            point.x,
            point.y,
            0
        );
        
        // run logic
        float3 closestPoint3D = GetClosestPointOnTarget3D(target3D, point3D);

        // convert back to Vector2
        return new Vector2(closestPoint3D.x, closestPoint3D.y);
    }

    public static float3 GetClosestPointOnTarget3D(float3[] target, float3 point)
    {
        float3 start = target[0];
        float3 stop = target[1];
        float3 direction = stop - start;
        // variable point v(t) = start + t * direction
        // distance point-v is minimal <=> vector point-v is orthogonal to direction <=> point-v * direction = 0
        // (point             - (start             + t * direction)) * direction  = 0
        // (point             -  start             - t * direction)  * direction  = 0
        //  point * direction -  start * direction - t * direction   * direction  = 0
        //  point * direction -  start * direction                                = t * direction   * direction
        // (point * direction -  start * direction) /     (direction * direction) = t
        float t = (math.dot(point, direction) - math.dot(start, direction)) / math.dot(direction, direction);

        float3 closestPoint;
        if (t <= 0)
        {
            closestPoint = start;
        }
        else if (t >= 1)
        {
            closestPoint = stop;
        }
        else
        {
            closestPoint = start + t * direction;
        }
        return closestPoint;
    }

    public static List<Color> GetNpcColours(List<Vector2> positions, Bounds bounds)
    {
        Color horizontalColour = Color.cyan;
        Color verticalColour = Color.magenta;
        Color baseColour = Color.white;
        float basePercent = 0.8f;

        float minX = bounds.min.x;
        float maxX = bounds.max.x;
        float minY = bounds.min.y;
        float maxY = bounds.max.y;

        List<Color> colours = new List<Color>();

        for (int i = 0; i < positions.Count; i++)
        {
            Vector2 position = positions[i];
            float percentHorizontal = (position.x - minX) / (maxX - minX);
            float percentVertical = (position.y - minY) / (maxY - minY);
            Color positionColour = (horizontalColour * percentHorizontal + verticalColour * percentVertical) / 2;
            Color colour = basePercent * baseColour + (1 - basePercent) * positionColour;
            colours.Add(colour);
        }

        return colours;
    }
}