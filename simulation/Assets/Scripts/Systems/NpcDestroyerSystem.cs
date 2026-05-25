using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

partial struct NpcDestroyerSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        foreach ((
            RefRO<NpcMover> npcMover,
            Entity entity)
            in SystemAPI.Query<
                RefRO<NpcMover>>().WithEntityAccess())
        {
            if (npcMover.ValueRO.queuedForDestruction)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
