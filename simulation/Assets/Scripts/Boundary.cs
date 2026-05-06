using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class Boundary : MonoBehaviour
{
    public static Boundary SharedInstance;
    
    public int width = 10;
    public int height = 10;
    public int borderWidth = 1;
    public float exitPc = 0.5f;
    public GameObject player;
    readonly int dpi = 600;
    private List<Rectangle> rectangles;
    private GameObject boundaryObject;
    private Sprite boundary;
    private SpriteRenderer sr;
    private Vector2 textureSize;

    private float referenceSize;
    private List<BoxCollider2D> colliders;
    private CompositeCollider2D compositeCollider;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        referenceSize = player.GetComponent<Renderer>().bounds.size.x;

        // set rectangle sizes
        Vector2 worldSize = new Vector2(width * referenceSize, height * referenceSize);
        Rectangle.SetWorldSize(worldSize);
        Vector2 textureSize = new Vector2(width * referenceSize * dpi, height * referenceSize * dpi);
        Rectangle.SetTextureSize(textureSize);


        SharedInstance = this;
        CreateGameObject();
        CreateBorderPoints();
        Texture2D texture = CreateTexture();
        CreateSprite(texture);
    }
    
    void Start()
    {
        CreateCollider();
    }

    private void CreateGameObject()
    {
        boundaryObject = new GameObject("boundary");
        sr = boundaryObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
    }

    private void CreateBorderPoints()
    {
        int pxWidth = (int)(width * referenceSize * dpi);
        int pxHeight = (int)(height * referenceSize * dpi);

        int pxBorderWidth = (int)(borderWidth * referenceSize * dpi);
        int exitStart = (int)(pxHeight * exitPc - pxBorderWidth / 2);
        int exitEnd = (int)(pxHeight * exitPc + pxBorderWidth / 2);
        
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

        rectangles = new List<Rectangle> {bottomLeft, bottom, right, top, topLeft} ;
    }

    private void CreateCollider()
    {
        // add partial colliders
        colliders = new List<BoxCollider2D>();
        for (int i = 0; i < rectangles.Count; i++)
        {
            Bounds rectangleBounds = rectangles[i].GetWorldBounds();
            BoxCollider2D collider = boundaryObject.AddComponent<BoxCollider2D>();
            collider.offset = rectangleBounds.center;
            collider.size = rectangleBounds.size;
            colliders.Add(collider);
        }
        // merge into composite collider
        compositeCollider = boundaryObject.AddComponent<CompositeCollider2D>();

        // remove gravity
        Rigidbody2D rigidBody = boundaryObject.GetComponent<Rigidbody2D>();
        rigidBody.bodyType = RigidbodyType2D.Kinematic;

    }
    
    private Texture2D CreateTexture()
    {
        float referenceSize = player.GetComponent<Renderer>().bounds.size.x;
        int pxWidth = (int)(width * referenceSize * dpi);
        int pxHeight = (int)(height * referenceSize * dpi); // THIS IS DUPLICATED FIX
        Texture2D texture = new Texture2D(pxWidth, pxHeight, TextureFormat.RGB24, false);

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
        byte[] bytes = texture.EncodeToPNG();
        System.IO.File.WriteAllBytes("Assets/Scripts/boundary.png", bytes);	
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

    public static Boundary GetInstance()
    {
        return SharedInstance;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
