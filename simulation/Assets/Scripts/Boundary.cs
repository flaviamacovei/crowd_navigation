using UnityEngine;

public class Boundary : MonoBehaviour
{
    public static Boundary SharedInstance;
    
    public int width = 10;
    public int height = 10;
    public int borderWidth = 1;
    public float exitPc = 0.5f;
    public GameObject player;
    readonly int dpi = 600;
    private GameObject boundaryObject;
    private Sprite boundary;
    private SpriteRenderer sr;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        SharedInstance = this;
    }
    
    void Start()
    {
        Texture2D texture = CreateTexture();
        CreateSprite(texture);
    }

    private Texture2D CreateTexture()
    {
        float referenceSize = player.GetComponent<Renderer>().bounds.size.x;
        int pxWidth = (int)(width * referenceSize * dpi);
        int pxHeight = (int)(height * referenceSize * dpi);
        int pxBorderWidth = (int)(borderWidth * referenceSize * dpi);
        int exitStart = (int)(pxHeight * exitPc - pxBorderWidth / 2);
        int exitEnd = (int)(pxHeight * exitPc + pxBorderWidth / 2);
        Texture2D texture = new Texture2D(pxWidth, pxHeight, TextureFormat.RGB24, false);

        Color transparent = new Color(0, 0, 0, 0);
        Color white = Color.white;

        Vector2 a = new Vector2(0, 0);
        Vector2 b = new Vector2(pxWidth - pxBorderWidth, 0);
        Vector2 c = new Vector2(pxWidth, pxHeight);
        Vector2 d = new Vector2(pxBorderWidth, pxHeight);
        Vector2 e = new Vector2(0, exitEnd);
        Vector2 f = new Vector2(pxWidth - pxBorderWidth, pxHeight - pxBorderWidth);
        Vector2 g = new Vector2(pxBorderWidth, pxBorderWidth);
        Vector2 h = new Vector2(pxBorderWidth, exitStart);

        for (int y = 0; y < pxHeight; y++)
        {
            for (int x = 0; x < pxWidth; x++)
            {
                Vector2 pos = new Vector2(x, y);
                if (
                    a.x <= x && x <= h.x && a.y <= y && y <= h.y || // bottom left rectangle
                    g.x <= x && x <= b.x && b.y <= y && y <= g.y || // bottom rectangle
                    b.x <= x && x <= c.x && b.y <= y && y <= c.y || // right rectangle
                    d.x <= x && x <= f.x && f.y <= y && y <= d.y || // top rectangle
                    e.x <= x && x <= d.x && e.y <= y && y <= d.y // top left rectangle
                )
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
        boundaryObject = new GameObject("boundary");
        sr = boundaryObject.AddComponent<SpriteRenderer>() as SpriteRenderer;
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
        // w      -> s
        // w - 2b -> x
        // x = ((w - 2b) * s) / w
        // x = (1 - 2b/w) * s
        Vector2 spawnSize = size * (new Vector2(1, 1) - new Vector2(2 * borderWidth, 2 * borderWidth) / new Vector2(width, height));
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
