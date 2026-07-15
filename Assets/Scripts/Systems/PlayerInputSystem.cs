using Unity.Entities;
using Unity.NetCode;
using UnityEngine;

[UpdateInGroup(typeof(GhostInputSystemGroup))]
partial class PlayerInputSystem : SystemBase //Because of new Input system
{
    private InputBindings bindings;

    protected override void OnCreate()
    {
        bindings = new InputBindings();
        bindings.Player.Enable();
    }

    //No burst due to SystemBase
    protected override void OnUpdate()
    {
        var moveVector = bindings.Player.Move.ReadValue<Vector2>();

        foreach (var playerInput in SystemAPI.Query<RefRW<PlayerInputComponent>>().WithAll<GhostOwnerIsLocal>())
        {
            playerInput.ValueRW.moveVector = moveVector;
        }
    }
}
