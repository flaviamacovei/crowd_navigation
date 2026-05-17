using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;
using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;

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

        SetNpcTarget();
        
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
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void SetNpcTarget()
    {
        EntityManager entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery entityQuery = new EntityQueryBuilder(Allocator.Temp).WithAll<NpcMover>().Build(entityManager);

        Rectangle exitRectangle = Boundary.GetInstance().GetExitRectangle();
        Vector2[] targetLineSegment2D = exitRectangle.GetRightLineSegment("world");

        float3 targetLineSegmentStart = new float3(targetLineSegment2D[0].x, targetLineSegment2D[0].y, 0);
        float3 targetLineSegmentStop = new float3(targetLineSegment2D[1].x, targetLineSegment2D[1].y, 0);
        

        NativeArray<Entity> entityArray = entityQuery.ToEntityArray(Allocator.Temp);
        NativeArray<NpcMover> npcMoverArray = entityQuery.ToComponentDataArray<NpcMover>(Allocator.Temp);

        for (int i = 0; i < npcMoverArray.Length; i++)
        {
            NpcMover npcMover = npcMoverArray[i];
            npcMover.targetLineSegmentStart = targetLineSegmentStart;
            npcMover.targetLineSegmentStop = targetLineSegmentStop;

            entityManager.SetComponentData(entityArray[i], npcMover);
        }
    }
}
