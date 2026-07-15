using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using UnityEngine;

public class PlayerInputAuth: MonoBehaviour
{
    public class Baker: Baker<PlayerInputAuth>
    {
        public override void Bake(PlayerInputAuth authoring)
        {
            var entity = GetEntity(TransformUsageFlags.Dynamic);
            AddComponent(entity, new PlayerInputComponent());
        }
    }
}
public struct PlayerInputComponent: IInputComponentData
{
    public float2 moveVector;
}
