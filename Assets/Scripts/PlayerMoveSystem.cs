using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;

[UpdateInGroup(typeof(PredictedSimulationSystemGroup))]
partial struct PlayerMoveSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerInputComponent>();
        state.RequireForUpdate<PlayerMovementSpecsComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        foreach (var (playerInput, movementSpecs,physicsVelocity) in SystemAPI.Query<RefRO<PlayerInputComponent>, RefRO<PlayerMovementSpecsComponent>,RefRW<PhysicsVelocity>>().WithAll<GhostOwnerIsLocal,Simulate>())
        {
            var moveVec = new float3(playerInput.ValueRO.moveVector.x, 0f, playerInput.ValueRO.moveVector.y);
            moveVec = math.normalizesafe(moveVec);

            physicsVelocity.ValueRW.Linear += moveVec * movementSpecs.ValueRO.acceleration * SystemAPI.Time.DeltaTime;

            float3 horizontalVel = new float3(physicsVelocity.ValueRW.Linear.x, 0f, physicsVelocity.ValueRW.Linear.z);

            float currentSpeed = math.length(horizontalVel);

            if (currentSpeed > movementSpecs.ValueRO.maxSpeed)
            {
                horizontalVel = horizontalVel / currentSpeed * movementSpecs.ValueRO.maxSpeed; //converting to normalized vector and multipling by maxSpeed

                physicsVelocity.ValueRW.Linear = new float3(horizontalVel.x, physicsVelocity.ValueRW.Linear.y, horizontalVel.z);
            }
        }
    }
}
