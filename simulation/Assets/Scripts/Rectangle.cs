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
        Vector2 size = new Vector2(maxX - minX, maxY - minY);
        Bounds setBounds = new Bounds(centre, size);

        if (space == "world")
        {
            worldBounds = setBounds;
            textureBounds = ConvertWorld2Texture(setBounds);
        }
        else
        {
            textureBounds = setBounds;
            worldBounds = ConvertTexture2World(setBounds);
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

    public Vector2[] GetBottomLineSegment(string space)
    {
        Bounds referenceBounds = space == "world"? worldBounds : textureBounds;

        return new[]
        {
            new Vector2(
                referenceBounds.min.x,
                referenceBounds.min.y
            ),
            new Vector2(
                referenceBounds.max.x,
                referenceBounds.min.y
            )
        };
    }

    public Vector2[] GetTopLineSegment(string space)
    {
        Bounds referenceBounds = space == "world"? worldBounds : textureBounds;

        return new[]
        {
            new Vector2(
                referenceBounds.min.x,
                referenceBounds.max.y
            ),
            new Vector2(
                referenceBounds.max.x,
                referenceBounds.max.y
            )
        };
    }

    public Vector2[] GetLeftLineSegment(string space)
    {
        Bounds referenceBounds = space == "world"? worldBounds : textureBounds;

        return new[]
        {
            new Vector2(
                referenceBounds.min.x,
                referenceBounds.min.y
            ),
            new Vector2(
                referenceBounds.min.x,
                referenceBounds.max.y
            )
        };
    }

    public Vector2[] GetRightLineSegment(string space)
    {
        Bounds referenceBounds = space == "world"? worldBounds : textureBounds;

        return new[]
        {
            new Vector2(
                referenceBounds.max.x,
                referenceBounds.min.y
            ),
            new Vector2(
                referenceBounds.max.x,
                referenceBounds.max.y
            )
        };
    }


    public static Bounds ConvertWorld2Texture(Bounds bounds)
    {
        Vector2 oldSize = bounds.size;
        Vector2 oldCentre = bounds.center;

        Vector2 newSize = ResizeWorld2Texture(oldSize);
        Vector2 newCentre = (oldCentre + (worldSize / 2)) * (textureSize / worldSize);

        return new Bounds(newCentre, newSize);
    }

    public static Bounds ConvertTexture2World(Bounds bounds)
    {
        Vector2 oldSize = bounds.size;
        Vector2 oldCentre = bounds.center;

        Vector2 newSize = ResizeTexture2World(oldSize);
        Vector2 newCentre = (oldCentre - (textureSize / 2)) * (worldSize / textureSize);    

        return new Bounds(newCentre, newSize);
    }
    
    public static Vector2 ResizeWorld2Texture(Vector2 size)
    {
        // worldSize -> textureSize
        // size      -> x
        // x = size * textureSize / worldSize
        return size * textureSize / worldSize;
    }

    public static Vector2 ResizeTexture2World(Vector2 size)
    {
        // textureSize -> worldSize
        // size        -> x
        // x = size * worldSize / textureSize
        return size * worldSize / textureSize;
    }

}