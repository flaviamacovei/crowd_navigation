#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;
using Unity.Entities;
using UnityEngine.SceneManagement;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

public class BoundaryAuthoring : MonoBehaviour
{
    public GameObject referenceObject;
    public int width;
    public int height;
    public float exitPc;
    public int exitSize;
    private int borderWidth = 1;
    readonly int dpi = 200;
    private Dictionary<string, Rectangle> rectangles;
    private GameObject boundaryObject;
    private Sprite boundary;
    private SpriteRenderer sr;
    private Vector2 textureSize;
    private List<BoxCollider> colliders;
    private string boundaryObjectName = "boundaryObject";
    private float referenceSize;
    private bool boundaryExists;

    public void CreateBoundary()
    {
        Vector2 objectSize = referenceObject.GetComponent<Renderer>().bounds.size;
        referenceSize = Math.Max(objectSize.x, objectSize.y);
        
        // set rectangle sizes
        Vector2 worldSize = new Vector2(width * referenceSize, height * referenceSize);
        Rectangle.SetWorldSize(worldSize);
        Vector2 textureSize = new Vector2(width * referenceSize * dpi, height * referenceSize * dpi);
        Rectangle.SetTextureSize(textureSize);

        // create boundary
        CreateGameObject();
        int[] pxSize = new[] { (int)(width * referenceSize * dpi), (int)(height * referenceSize * dpi) };
        int pxExitSize = (int)(exitSize * referenceSize * dpi);
        int pxBorderWidth = (int)(borderWidth * referenceSize * dpi);

        CreateBorderPoints(pxSize, pxExitSize, pxBorderWidth);
        Texture2D texture = CreateTexture(pxSize);
        CreateSprite(texture);
        CreateCollider();
        boundaryExists = true;
    }

    private void CreateGameObject()
    {
        boundaryObject = new GameObject(boundaryObjectName);

        SceneManager.MoveGameObjectToScene(
            boundaryObject,
            gameObject.scene
        );
        boundaryObject.transform.SetParent(transform);

        sr = boundaryObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    public float GetReferenceSize()
    {
        return referenceSize;
    }


    private void CreateBorderPoints(int[] pxSize, int pxExitSize, int pxBorderWidth)
    {
        int pxWidth = pxSize[0];
        int pxHeight = pxSize[1];

        int exitStart = (int)(pxHeight * exitPc - pxExitSize / 2);
        int exitEnd = (int)(pxHeight * exitPc + pxExitSize / 2);
        if (exitStart < pxBorderWidth)
        {
            int offset = exitStart - pxBorderWidth;
            exitStart = pxBorderWidth;
            exitEnd += offset;
        }
        else if (exitEnd > pxHeight - pxBorderWidth)
        {
            int offset = exitEnd - (pxHeight - pxBorderWidth);
            exitEnd = pxHeight - pxBorderWidth;
            exitStart -= offset;
        }
        
        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(pxWidth - pxBorderWidth, 0);
        Vector2 c = new Vector2(pxWidth, pxHeight);
        Vector2 d = new Vector2(pxBorderWidth, pxHeight);
        Vector2 e = new Vector2(0, exitEnd);
        Vector2 f = new Vector2(pxWidth - pxBorderWidth, pxHeight - pxBorderWidth);
        Vector2 g = new Vector2(pxBorderWidth, pxBorderWidth);
        Vector2 h = new Vector2(pxBorderWidth, exitStart);

        rectangles = new Dictionary<string, Rectangle>
        {
            { "bottomLeft", new Rectangle(a, h, "texture") },
            { "bottom", new Rectangle(b, g, "texture") },
            { "right", new Rectangle(b, c, "texture") },
            { "top", new Rectangle(d, f, "texture") },
            { "topLeft", new Rectangle(d, e, "texture") }
        };
    }

    private void CreateCollider()
    {
        // add partial colliders
        colliders = new List<BoxCollider>();
        foreach( string key in rectangles.Keys )
        {
            Bounds rectangleBounds = rectangles[key].GetWorldBounds();
            BoxCollider collider = boundaryObject.AddComponent<BoxCollider>();
            collider.center = new Vector3(
                rectangleBounds.center.x,
                rectangleBounds.center.y,
                0
            );
            collider.size = new Vector3(
                rectangleBounds.size.x,
                rectangleBounds.size.y,
                0.2f
            );
            colliders.Add(collider);
        }

        // remove gravity
        Rigidbody rigidBody = boundaryObject.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true;

    }

    private Texture2D CreateTexture(int[] textureSize)
    {
        Texture2D texture = new Texture2D(textureSize[0], textureSize[1], TextureFormat.RGBA32, false);

        Color transparent = new Color(0, 0, 0, 0);
        Color white = Color.white;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Vector2 px = new Vector2(x, y);

                List<Rectangle> rectangleList = rectangles.Values.ToList();
                
                IEnumerable<bool> inRectangles =
                    from rectangle in rectangleList
                    where rectangle.GetTextureBounds().Contains(px)
                    select true;
                bool inRectangle = inRectangles.Any();
                if (inRectangle)
                {
                    texture.SetPixel(x, y, white);
                }
                else
                {
                    texture.SetPixel(x, y, transparent);
                }
            }
        }
        texture.Apply();	
        return texture;
    }

    private void CreateSprite(Texture2D texture)
    {
        boundary = Sprite.Create(
            texture,
            new Rect(0, 0, texture.width, texture.height),
            new Vector2(0.5f, 0.5f),
            dpi
        );
        sr.sprite = boundary;
    }

    public Bounds GetSpawningBounds()
    {
        Vector2 centre = sr.bounds.center;
        Vector2 size = sr.bounds.size;

        Vector2 spawnPercent = new Vector2(1.0f - 2.0f * (float)borderWidth / (float)width, 1 - 2.0f * (float)borderWidth / (float)height);
        Vector2 spawnSizeTexture = Rectangle.ResizeWorld2Texture(size) * spawnPercent;
        Vector2 spawnCentreTexture = new Vector2(0,0);
        Vector2 spawnSizeWorld = Rectangle.ResizeTexture2World(spawnSizeTexture);
        
        return new Bounds(centre, spawnSizeWorld);
    }

    public Rectangle GetFarEdge()
    {
        Rectangle bottom = rectangles["bottom"];
        Rectangle top = rectangles["top"];
        Rectangle right = rectangles["right"];

        Vector2 lowerEnd = new Vector2(
            bottom.GetWorldBounds().max.x,
            bottom.GetWorldBounds().max.y
        );

        Vector2 upperEnd = new Vector2(
            right.GetWorldBounds().max.x,
            top.GetWorldBounds().min.y
        );

        return new Rectangle(lowerEnd, upperEnd, "world");
    }

    public Rectangle GetExitRectangle()
    {
        Rectangle bottomLeft = rectangles["bottomLeft"];
        Rectangle topLeft = rectangles["topLeft"];

        Rectangle output = new Rectangle(
            bottomLeft.GetWorldBounds().max,
            topLeft.GetWorldBounds().min,
            "world"
        );
        return output;
    }

    #if UNITY_EDITOR
    private void OnValidate()
    {
        EditorApplication.delayCall += DelayedRebuild;
    }

    public void CleanUp()
    {
        GameObject existingBoundaryObject = GameObject.Find(boundaryObjectName);
        while (existingBoundaryObject != null)
        {
            DestroyImmediate(existingBoundaryObject);
            existingBoundaryObject = GameObject.Find(boundaryObjectName);
        }
        boundaryExists = false;
    }

    public bool BoundaryExists()
    {
        return boundaryExists;
    }

    private void DelayedRebuild()
    {
        if (this == null)
        {
            return;
        }
        CleanUp();
        CreateBoundary();
    }
    #endif

    public class Baker : Baker<BoundaryAuthoring>
    {
        public override void Bake(BoundaryAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);

            if (authoring.BoundaryExists())
            {

                Rectangle exitRectangle = authoring.GetExitRectangle();
                Vector2[] targetLineSegment2D = exitRectangle.GetRightLineSegment("world");
                float3 targetLineSegmentStart = new float3(targetLineSegment2D[0].x, targetLineSegment2D[0].y, 0);
                float3 targetLineSegmentStop = new float3(targetLineSegment2D[1].x, targetLineSegment2D[1].y, 0);

                Bounds spawnBounds = authoring.GetSpawningBounds();

                float radius = authoring.GetReferenceSize();

                List<Vector2> spawnPositions = Utils.GetSpawnPositions(radius, spawnBounds);

                Debug.Log("making an unsafe list of length " + spawnPositions.Count);
                UnsafeList<float2> spawnPositionsUnsafe = new UnsafeList<float2>(spawnPositions.Count, Allocator.Persistent);
                
                for (int i = 0; i < spawnPositions.Count; i++)
                {
                    spawnPositionsUnsafe.Add((float2)spawnPositions[i]);
                }
                Debug.Log("made a list of " + spawnPositionsUnsafe.Length + " items");
            
                AddComponent(entity, new Boundary
                {
                    boundaryObject = GetEntity(authoring.boundaryObject, TransformUsageFlags.Dynamic),
                    targetLineSegmentStart = targetLineSegmentStart,
                    targetLineSegmentStop = targetLineSegmentStop,
                    spawnPositionsUnsafe = spawnPositionsUnsafe
                });
            }
        }
    }
}

[ChunkSerializable]
public struct Boundary : IComponentData
{
    public Entity boundaryObject;
    public float3 targetLineSegmentStart;
    public float3 targetLineSegmentStop;
    public UnsafeList<float2> spawnPositionsUnsafe;
}
