using UnityEngine;

public class Player : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static Player SharedInstance;
    private GameObject playerInstance;
    
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

    public void PlayerUpdate(float speed)
    {
        Vector2 movement = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")) * speed;

        Rigidbody2D rigidBody = playerInstance.GetComponent<Rigidbody2D>();
        rigidBody.AddForce(movement, ForceMode2D.Force);
    }


    public static Player GetInstance()
    {
        return SharedInstance;
    }

    public void PlacePlayer(GameObject player, Vector2 position)
    {
        playerInstance = Instantiate(player);
        playerInstance.name = "player";
        playerInstance.transform.position = position;
    }
}
