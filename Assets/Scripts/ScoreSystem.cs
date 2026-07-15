using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct ScoreSystem : ISystem
{
    private int hostCurrentScore;
    private int clientCurrentScore;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<PlayerInputComponent>();
    }

    //[BurstCompile] Debug.Log at the end
    public void OnUpdate(ref SystemState state)
    {
        var allPlayerEntities = SystemAPI.QueryBuilder().WithAll<PlayerInputComponent>().Build().ToEntityArray(Allocator.Temp);
        if (allPlayerEntities.Length < 2) return;

        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        NetworkTime networkTime = SystemAPI.GetSingleton<NetworkTime>();

        DynamicBuffer<ScoreBuffer> scoreBuffer = SystemAPI.GetSingletonBuffer<ScoreBuffer>();


        foreach (var (localTransform, physicsVelocity, entity) in SystemAPI.Query<RefRW<LocalTransform>, RefRW<PhysicsVelocity>>().WithAll<PlayerInputComponent>().WithEntityAccess())
        {
            allPlayerEntities = SystemAPI.QueryBuilder().WithAll<PlayerInputComponent>().Build().ToEntityArray(Allocator.Temp);

            if (localTransform.ValueRO.Position.y < -5f)
            {
                RespawnPlayer(entity, ref localTransform.ValueRW, ref physicsVelocity.ValueRW, networkTime);

                Score(entity, allPlayerEntities, ref scoreBuffer, ref state);
            }
        }
        allPlayerEntities.Dispose();
    }
    private void RespawnPlayer(in Entity entity, ref LocalTransform localTransform, ref PhysicsVelocity physicsVelocity, NetworkTime networkTime)
    {
        var random = Unity.Mathematics.Random.CreateFromIndex(networkTime.ServerTick.TickIndexForValidTick ^ (uint)entity.Index + 1);

        localTransform.Position =
            random.NextFloat3(
                new float3(-5f, 1f, -5f),
                new float3(5f, 1f, 5f)
            );

        physicsVelocity.Linear = float3.zero;
        physicsVelocity.Angular = float3.zero;
    }
    private void Score(in Entity fallenEntity, NativeArray<Entity> allPlayerEntities, ref DynamicBuffer<ScoreBuffer> scoreBuffer, ref SystemState state)
    {
        if (SystemAPI.HasComponent<HostPlayerTag>(fallenEntity))
        {
            scoreBuffer.Add(new ScoreBuffer
            {
                hostEntity = allPlayerEntities[0],
                clientEntity = allPlayerEntities[1],
                hostScore = hostCurrentScore,
                clientScore = ++clientCurrentScore
            });
        }
        else if (allPlayerEntities[1] == fallenEntity)
        {
            scoreBuffer.Add(new ScoreBuffer
            {
                hostEntity = allPlayerEntities[0],
                clientEntity = allPlayerEntities[1],
                hostScore = ++hostCurrentScore,
                clientScore = clientCurrentScore
            });
        }

        var lastBufferElement = scoreBuffer.ElementAt(scoreBuffer.Length - 1);

        Debug.Log($"Host : {lastBufferElement.hostScore} \n" +
            $"Client : {lastBufferElement.clientScore}");
    }

    
}
