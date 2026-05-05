using UnityEngine;
using System.Collections.Generic;

public class Boundary : MonoBehaviour
{
    public static Boundary SharedInstance;
    
    public int width = 10;
    public int height = 10;
    public int borderWidth = 1;
    public float exitPc = 0.5f;
    public GameObject player;
    readonly int dpi = 600;
    private List<Rectangle> border;
    private GameObject boundaryObject;
    private Sprite boundary;
    private SpriteRenderer sr;
    private Vector2 textureSize;

    private float referenceSize;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        referenceSize = player.GetComponent<Renderer>().bounds.size.x;
        SharedInstance = this;
        CreateGameObject();
        CreateBorderPoints();
        Texture2D texture = CreateTexture();
        CreateSprite(texture);
        CreateCollider();
    }
    
    void Start()
    {
        
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

        Rectangle.SetTextureSize(new Vector2(pxWidth, pxHeight));

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

        border = new List<Rectangle> {bottomLeft, bottom, right, top, topLeft} ;
    }

    private void CreateCollider()
    {
        boundaryObject.AddComponent<BoxCollider2D>();
    }
    
    private Texture2D CreateTexture()
    {
        float referenceSize = player.GetComponent<Renderer>().bounds.size.x;
        int pxWidth = (int)(width * referenceSize * dpi);
        int pxHeight = (int)(height * referenceSize * dpi); // THIS IS DUPLICATED FIX
        Texture2D texture = new Texture2D(pxWidth, pxHeight, TextureFormat.RGBA32, false);

        Color transparent = new Color(0, 0, 0, 0);
        Color white = Color.white;

        for (int y = 0; y < texture.height; y++)
        {
            for (int x = 0; x < texture.width; x++)
            {
                Vector2 px = new Vector2(x, y);
                bool inRectangle = false;
                for (int i = 0; i < border.Count; i++)
                {
                    if (border[i].GetTextureBounds().Contains(px))
                    {
                        texture.SetPixel(x, y, white);
                        inRectangle = true;
                    }
                }
                if (!inRectangle)
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
        Rectangle.SetWorldSize(sr.bounds.size);
    }

    public Bounds GetSpawningBounds()
    {
        Vector2 centre = sr.bounds.center;
        Vector2 size = sr.bounds.size;
        // w      -> s
        // w - 2b -> x
        // x = ((w - 2b) * s) / w
        // x = (1 - 2b/w) * s
        Vector2 spawnSize = Rectangle.Texture2World(new Vector2(width - 2 * borderWidth, height - 2 * borderWidth));
        Debug.Log("spawnSize: " + spawnSize);
        return new Bounds(centre, spawnSize);
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
