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
            
            Rectangle exitRectangle = Boundary.GetInstance().GetExitRectangle();
            Vector2[] targetLineSegment = exitRectangle.GetRightLineSegment("world");
			Vector2 targetPositionVector2 = Utils.GetClosestPointOnTarget(targetLineSegment, new Vector2(localTransform.ValueRO.Position.x, localTransform.ValueRO.Position.y));
            float3 targetPosition = new float3(targetPositionVector2.x, targetPositionVector2.y, 0);
            float3 moveDirection = targetPosition - localTransform.ValueRO.Position;
            moveDirection = math.normalize(moveDirection);

            physicsVelocity.ValueRW.Linear = moveDirection * npcMover.ValueRO.moveSpeed;
            physicsVelocity.ValueRW.Angular = float3.zero;
        }
    }
}
