using UnityEngine;
using Unity.Entities;

public class NpcMoverAuthoring : MonoBehaviour
{
    public float moveSpeed;

    public class Baker : Baker<NpcMoverAuthoring>
    {
        public override void Bake(NpcMoverAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new NpcMover
            {
                moveSpeed = authoring.moveSpeed,
            });
        }
    }
}

public struct NpcMover : IComponentData
{
    public float moveSpeed;
}
