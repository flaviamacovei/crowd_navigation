using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Player SharedInstance;
    private GameObject player;
    
    void Awake()
    {
        SharedInstance = this;
    }
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static Player GetInstance()
    {
        return SharedInstance;
    }

    public void PlacePlayer(GameObject player, Vector2 position)
    {
        player = Instantiate(player);
        player.transform.position = position;
    }
}
