using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Controller : MonoBehaviour
{
    public GameObject player;
    float objectRadius;
    private Color playerColour = Color.red;
    private Color npcColour = Color.white;
    public float speed = 1.0f;
    private Vector2[] targetLineSegment;

    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

        Vector2 objectSize = player.GetComponent<Renderer>().bounds.size;
        objectRadius = Math.Max(objectSize.x, objectSize.y);

        // create boundary
        Boundary.GetInstance().CreateBoundary(objectRadius);

        Bounds spawnBounds = Boundary.GetInstance().GetSpawningBounds();
        Vector2 spawnCentre = spawnBounds.center;
        Vector2 spawnSize = (Vector2)spawnBounds.size - new Vector2(objectRadius, objectRadius);

        List<Vector2> positions = Utils.GetSpawnPositions(objectRadius, new Bounds(spawnCentre, spawnSize));
        int playerIndex = UnityEngine.Random.Range(0, positions.Count);
        Vector2 playerPosition = positions[playerIndex];
        positions.RemoveAt(playerIndex);

        // npcs have white colour
        player.GetComponent<SpriteRenderer>().color = npcColour;
        NpcPool.GetInstance().PlaceObjects(player, positions);

        // player has red colour
        player.GetComponent<SpriteRenderer>().color = playerColour;
        Player.GetInstance().PlacePlayer(player, playerPosition);

        // exit target
        targetLineSegment = Boundary.GetInstance().GetTargetLineSegment();
    }

    // Update is called once per frame
    void Update()
    {
        NpcPool.GetInstance().NpcUpdate(targetLineSegment, speed);
        Player.GetInstance().PlayerUpdate(speed);
    }
}
