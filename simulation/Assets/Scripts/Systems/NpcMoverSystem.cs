using Unity.Burst;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Physics;
using UnityEngine;

partial struct NpcMoverSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach ((
            RefRW<LocalTransform> localTransform, 
            RefRO<NpcMover> npcMover,
            RefRW<PhysicsVelocity> physicsVelocity) 
            in SystemAPI.Query<
                RefRW<LocalTransform>, 
                RefRO<NpcMover>,
                RefRW<PhysicsVelocity>>())
        {

            float3 targetPosition = Utils.GetClosestPointOnTarget3D(new[] {npcMover.ValueRO.targetLineSegmentStart, npcMover.ValueRO.targetLineSegmentStop}, localTransform.ValueRO.Position);

            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            physicsVelocity.ValueRW.Linear = moveDirection * npcMover.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}
