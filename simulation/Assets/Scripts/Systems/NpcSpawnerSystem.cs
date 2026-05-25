using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
[CreateAfter(typeof(InitializationSystemGroup))]
partial struct NpcSpawnerSystem : ISystem
{
    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        Boundary boundary = SystemAPI.GetSingleton<Boundary>();
        foreach ((
            RefRO<NpcSpawner> npcSpawner,
            Entity entity)
            in SystemAPI.Query<
            RefRO<NpcSpawner>>().WithEntityAccess())
        {

            UnsafeList<float2> spawnPositions = boundary.spawnPositionsUnsafe;
            
            for (int i = 0; i < spawnPositions.Length; i++)
            {
                Entity npcEntity = state.EntityManager.Instantiate(npcSpawner.ValueRO.npcEntity);
                SystemAPI.SetComponent(npcEntity, LocalTransform.FromPosition(new float3(
                    spawnPositions[i][0],
                    spawnPositions[i][1],
                    0
                )));
            }
            boundary.spawnPositionsUnsafe.Dispose();
            entityCommandBuffer.RemoveComponent<NpcSpawner>(entity);
        }
    }
}
