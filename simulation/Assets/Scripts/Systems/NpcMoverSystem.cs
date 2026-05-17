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
        NpcMoverJob npcMoverJob = new NpcMoverJob();
        npcMoverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct NpcMoverJob : IJobEntity
{
    public void Execute(ref LocalTransform localTransform, in NpcMover npcMover, ref PhysicsVelocity physicsVelocity)
    {
        float3 targetPosition = Utils.GetClosestPointOnTarget3D(new[] {npcMover.targetLineSegmentStart, npcMover.targetLineSegmentStop}, localTransform.Position);

        float3 moveDirection = targetPosition - localTransform.Position;
        moveDirection = math.normalize(moveDirection);

        physicsVelocity.Linear = moveDirection * npcMover.moveSpeed;
        physicsVelocity.Angular = float3.zero;
    }
}