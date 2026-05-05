using UnityEngine;
using System;
public class Rectangle
{
    /// <summary>
    /// 2d rectangle defined by two diagonal vertices.
    /// Can be transformed between texture and world spaces.
    /// Assumption: world space is centered at (0,0); texture space has (0,0) in bottom left corner.
    /// </summary>
    private static Vector2 worldSize;
    private static Vector2 textureSize;
    private Bounds worldBounds;
    private Bounds textureBounds;
    
    public Rectangle(Vector2 point1, Vector2 point2, string space)
    {
        // Assert.IsTrue(space == "world" || space == "texture");
        
        Vector2 centre = (point1 + point2) / 2f;
        float minX = Math.Min(point1.x, point2.x);
        float maxX = Math.Max(point1.x, point2.x);
        float minY = Math.Min(point1.y, point2.y);
        float maxY = Math.Max(point1.y, point2.y);

        if (space == "world")
        {
            worldBounds = new Bounds(centre, new Vector2(maxX - minX, maxY - minY));
            textureBounds = new Bounds(new Vector2(0, 0), World2Texture(worldBounds.size));
        }
        else
        {
            textureBounds = new Bounds(centre, new Vector2(maxX - minX, maxY - minY));
            worldBounds = new Bounds(new Vector2(0, 0), Texture2World(textureBounds.size));
        }
    }

    public static void SetWorldSize(Vector2 size)
    {
        worldSize = size;
    }
    public static void SetTextureSize(Vector2 size)
    {
        textureSize = size;
    }
    public static Vector2 GetWorldSize()
    {
        return worldSize;
    }
    public static Vector2 GetTextureSize()
    {
        return textureSize;
    }
    
    public Bounds GetWorldBounds()
    {
        return worldBounds;
    }

    public Bounds GetTextureBounds()
    {
        return textureBounds;
    }

    public static Vector2 World2Texture(Vector2 point)
    {
        // worldSize -> textureSize
        // point      -> x
        // x = point * textureSize / worldSize
        return point * textureSize / worldSize;
    }

    public static Vector2 Texture2World(Vector2 point)
    {
        // textureSize -> worldSize
        // point        -> x
        // x = point * worldSize / textureSize
        return point * worldSize / textureSize;
    }

}