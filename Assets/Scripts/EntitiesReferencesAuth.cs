using Unity.Entities;
using UnityEngine;

public class EntitiesReferencesAuth: MonoBehaviour
{
    [SerializeField] private GameObject hostPlayer;
    [SerializeField] private GameObject clientPlayer;

    public class Baker : Baker<EntitiesReferencesAuth>
    {
        public override void Bake(EntitiesReferencesAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.None);
            AddComponent(entity, new EntitiesReferences
            {
                hostEntity = GetEntity(authoring.hostPlayer, TransformUsageFlags.Dynamic),
                clientEntity = GetEntity(authoring.clientPlayer, TransformUsageFlags.Dynamic)
            });
        }
    }

}
public struct EntitiesReferences : IComponentData
{
    public Entity hostEntity;
    public Entity clientEntity;
}

