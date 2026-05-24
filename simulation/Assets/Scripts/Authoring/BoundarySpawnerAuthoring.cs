using UnityEngine;
using Unity.Entities;

public class BoundarySpawnerAuthoring : MonoBehaviour
{
    public class Baker : Baker<BoundarySpawnerAuthoring>
    {
        public override void Bake(BoundarySpawnerAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new BoundarySpawner());
        }
    }
}

public struct BoundarySpawner : IComponentData
{}