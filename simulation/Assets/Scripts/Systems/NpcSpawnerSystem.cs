using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

[CreateAfter(typeof(InitializationSystemGroup))]
partial struct NpcSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        // Debug.Log("entered");
        // EntitiesReferences entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        // Entity npcEntity = state.EntityManager.Instantiate(entitiesReferences.npcEntity);
        // Debug.Log("spawned");
        // SystemAPI.SetComponent(npcEntity, LocalTransform.FromPosition(new float3(2, 2, 0)));
    }
}
