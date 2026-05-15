using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class Controller : MonoBehaviour
{
    public GameObject player;
    float objectRadius;
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

        // npcs get colour based on their position
        List<Color> colours = Utils.GetNpcColours(positions, spawnBounds);

        // player has red colour
        player.GetComponent<SpriteRenderer>().color = Color.red;
        // Player.GetInstance().PlacePlayer(player, playerPosition);

        Rectangle emitterLocation = Boundary.GetInstance().GetFarEdge();
        Rectangle attractorLocation = Boundary.GetInstance().GetExitRectangle();
        Vector2 boundarySize = Boundary.GetInstance().GetWorldSize();
        float attractorRange = Math.Max(boundarySize.x, boundarySize.y);

        SpriteRenderer playerRenderer = player.GetComponent<SpriteRenderer>();
        Collider2D playerCollider = player.GetComponent<Collider2D>();
        NpcParticles.GetInstance().CreateParticleEmitter(emitterLocation, objectRadius, playerRenderer);
        NpcParticles.GetInstance().CreateParticleAttractor(attractorLocation, playerCollider, attractorRange);
        // NpcParticles.GetInstance().PlaceObjects(player, positions, colours);
    }

    // Update is called once per frame
    void Update()
    {
        Player.GetInstance().PlayerUpdate(speed);
    }
}
