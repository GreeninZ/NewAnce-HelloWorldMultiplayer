using Unity.Entities;
using UnityEngine;

public class PlayerMovementSpecsAuth : MonoBehaviour
{
    [SerializeField] private float acceleration = 35f;
    [SerializeField] private float maxSpeed = 10f;
    public class Baker: Baker<PlayerMovementSpecsAuth>
    {
        public override void Bake(PlayerMovementSpecsAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerMovementSpecsComponent
            {
                acceleration = authoring.acceleration,
                maxSpeed = authoring.maxSpeed
            });
        }
    }
}
public struct PlayerMovementSpecsComponent: IComponentData
{
    public float acceleration;
    public float maxSpeed;
}
