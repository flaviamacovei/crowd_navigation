using UnityEngine;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Collections;
using System;

public class NpcSpawnerAuthoring : MonoBehaviour
{
    public GameObject npcGameObject;
    public class Baker : Baker<NpcSpawnerAuthoring>
    {
        public override void Bake(NpcSpawnerAuthoring authoring)
        {
            Vector2 objectSize = authoring.npcGameObject.GetComponent<Renderer>().bounds.size;
            float radius = Math.Max(objectSize.x, objectSize.y);
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NpcSpawner
            {
                npcEntity = GetEntity(authoring.npcGameObject, TransformUsageFlags.Dynamic),
                radius = radius,
            });
        }
    }
}

public struct NpcSpawner : IComponentData
{
    public Entity npcEntity;
    public float radius;
}
