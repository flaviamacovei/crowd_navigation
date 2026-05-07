using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Boundary : MonoBehaviour
{
    public static Boundary SharedInstance;
    
    public int width = 10;
    public int height = 10;
    private int borderWidth = 1;
    public float exitPc = 0.5f;
    public int exitSize = 2;
    public GameObject player;
    readonly int dpi = 600;
    private Rectangle[] rectangles;
    private GameObject boundaryObject;
    private Sprite boundary;
    private SpriteRenderer sr;
    private Vector2 textureSize;

    private BoxCollider2D[] colliders;
    private CompositeCollider2D compositeCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SharedInstance = this;
    }
    
    void Start()
    {
        
    }


    public void CreateBoundary(float referenceSize)
    {
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
    }

    private void CreateGameObject()
    {
        boundaryObject = new GameObject("boundary");
        sr = boundaryObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
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

        Rectangle bottomLeft = new Rectangle(a, h, "texture");
        Rectangle bottom = new Rectangle(b, g, "texture");
        Rectangle right = new Rectangle(b, c, "texture");
        Rectangle top = new Rectangle(d, f, "texture");
        Rectangle topLeft = new Rectangle(d, e, "texture");

        rectangles = new[] { bottomLeft, bottom, right, top, topLeft };
    }

    private void CreateCollider()
    {
        // add partial colliders
        colliders = new BoxCollider2D[rectangles.Length];
        for (int i = 0; i < rectangles.Length; i++)
        {
            Bounds rectangleBounds = rectangles[i].GetWorldBounds();
            BoxCollider2D collider = boundaryObject.AddComponent<BoxCollider2D>();
            collider.offset = rectangleBounds.center;
            collider.size = rectangleBounds.size;
            colliders[i] = collider;
        }
        // merge into composite collider
        compositeCollider = boundaryObject.AddComponent<CompositeCollider2D>();

        // remove gravity
        Rigidbody2D rigidBody = boundaryObject.GetComponent<Rigidbody2D>();
        rigidBody.bodyType = RigidbodyType2D.Kinematic;

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

                IEnumerable<bool> inRectangles =
                    from rectangle in rectangles
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

    public Vector2[] GetTargetLineSegment()
    {
        Rectangle bottomLeft = rectangles[0];
        Rectangle topLeft = rectangles[4];

        Vector2 lowerEnd = new Vector2(
            bottomLeft.GetWorldBounds().max.x,
            bottomLeft.GetWorldBounds().max.y
        );
        Vector2 upperEnd = new Vector2(
            topLeft.GetWorldBounds().max.x,
            topLeft.GetWorldBounds().min.y
        );

        return new[] { lowerEnd, upperEnd };
    }

    public static Boundary GetInstance()
    {
        return SharedInstance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
