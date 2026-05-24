using Unity.Burst;
using Unity.Entities;
using UnityEngine;

partial struct BoundarySpawnerSystem : ISystem
{

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer entityCommandBuffer = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);
        // Boundary boundary = SystemAPI.GetSingleton<Boundary>();
        foreach ((
            RefRO<BoundarySpawner> boundarySpawner,
            RefRO<Boundary> boundary,
            Entity entity)
            in SystemAPI.Query<
            RefRO<BoundarySpawner>,
            RefRO<Boundary>>().WithEntityAccess())
        {

            state.EntityManager.Instantiate(boundary.ValueRO.boundaryObject);
            entityCommandBuffer.RemoveComponent<BoundarySpawner>(entity);
        }

    }

}
