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
            RefRO<LocalTransform> localTransform,
            RefRO<NpcMover> npcMover,
            Entity entity)
            in SystemAPI.Query<
                RefRO<LocalTransform>,
                RefRO<NpcMover>>().WithEntityAccess())
        {
            float distanceThreshold = 0.1f;
            float3 targetPosition = Utils.GetClosestPointOnTarget3D(new[] {npcMover.ValueRO.targetLineSegmentStart, npcMover.ValueRO.targetLineSegmentStop}, localTransform.ValueRO.Position);

            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            if (math.lengthsq(moveDirection) < distanceThreshold)
            {
                entityCommandBuffer.DestroyEntity(entity);
            }
        }
    }
}
