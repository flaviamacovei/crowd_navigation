using UnityEngine;
using System;
public class Rectangle
{
    /// <summary>
    /// 2d rectangle defined by two diagonal vertices.
    /// Can be transformed between texture and world spaces.
    /// Assumption: world space is centered at (0,0); texture space has (0,0) in bottom left corner.
    /// </summary>
    private Vector2 worldSize;
    private Vector2 textureSize;
    private Bounds worldBounds;
    private Bounds textureBounds;
    
    public Rectangle(Vector2 point1, Vector2 point2, string space, Vector2 worldSize, Vector2 textureSize)
    {
        Assert.IsTrue(space == "world" || space == "texture");

        worldSize = worldSize;
        textureSize = textureSize;
        
        Vector2 centre = (point1 + point2) / 2f;
        float minX = Math.Min(point1.x, point2.x);
        float maxX = Math.Max(point1.x, point2.x);
        float minY = Math.Min(point1.y, point2.y);
        float maxY = Math.Max(point1.y, point2.y);

        if (space == "world")
        {
            worldBounds = new Bounds(centre, new Vector2(maxX - minX, maxY - minY));
            textureBounds = World2Texture(worldBounds);
        }
        else
        {
            textureBounds = new Bounds(centre, new Vector2(maxX - minX, maxY - minY));
            worldBounds = Texture2World(textureBounds);
        }
    }

    public static Vector2 World2Texture(Bounds bounds)
    {
        // move
        Vector2 newCentre = bounds.center + (textureBounds.center - worldBounds.center);

        // scale
        // worldSize -> textureSize
        // size      -> x
        // x = size * textureSize / worldSize
        Vector2 newSize = bounds.size * textureBounds.size / worldBounds.size;

        return new Bounds(newCentre, newSize);
    }

    public static Vector2 Texture2World(Bounds bounds)
    {
        // move
        Vector2 newCentre = bounds.center - (worldBounds.center - textureBounds.center);

        // scale
        // textureSize -> worldSize
        // size        -> x
        // x = size * worldSize / textureSize
        Vector2 newSize = bounds.size * worldBounds.size / textureBounds.size;

        return new Bounds(newCentre, newSize);
    }

}