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
        Boundary boundary = SystemAPI.GetSingleton<Boundary>();
        NpcMoverJob npcMoverJob = new NpcMoverJob
        {
            targetLineSegmentStart = boundary.targetLineSegmentStart,
            targetLineSegmentStop = boundary.targetLineSegmentStop,
            distanceThreshold = 0.1f,
        };
        npcMoverJob.ScheduleParallel();
    }
}

[BurstCompile]
public partial struct NpcMoverJob : IJobEntity
{
    public float3 targetLineSegmentStart;
    public float3 targetLineSegmentStop;
    public float distanceThreshold;
    public void Execute(ref LocalTransform localTransform, ref NpcMover npcMover, ref PhysicsVelocity physicsVelocity)
    {
        float3 targetPosition = Utils.GetClosestPointOnTarget3D(new[] {targetLineSegmentStart, targetLineSegmentStop}, localTransform.Position);
        
        float3 moveDirection = targetPosition - localTransform.Position;

        if (math.lengthsq(moveDirection) < distanceThreshold)
        {
            npcMover.queuedForDestruction = true;
        }
        else
        {
            moveDirection = math.normalize(moveDirection);
            physicsVelocity.Linear = moveDirection * npcMover.moveSpeed;
            physicsVelocity.Angular = float3.zero;
        }
    }
}