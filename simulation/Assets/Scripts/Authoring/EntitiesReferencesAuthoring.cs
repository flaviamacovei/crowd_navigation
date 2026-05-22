using UnityEngine;
using Unity.Entities;

public class EntitiesReferencesAuthoring : MonoBehaviour
{
    public GameObject npcGameObject;
    public class Baker : Baker<EntitiesReferencesAuthoring>
    {
        public override void Bake(EntitiesReferencesAuthoring authoring)
        {
            Entity entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new EntitiesReferences
            {
                npcEntity = GetEntity(authoring.npcGameObject, TransformUsageFlags.Dynamic)
            });
        }
    }
}

public struct EntitiesReferences : IComponentData
{
    public Entity npcEntity;
}
