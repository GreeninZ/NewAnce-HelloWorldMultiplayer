using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Transforms;

[WorldSystemFilter(WorldSystemFilterFlags.ServerSimulation)]
partial struct ProcessInGameRequestServerSystem : ISystem
{
    private EntitiesReferences entitiesReferences;
    private NetworkTime networkTime;

    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<InGameRequestRPC>();

        
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        networkTime = SystemAPI.GetSingleton<NetworkTime>();

        entitiesReferences = SystemAPI.GetSingleton<EntitiesReferences>();

        foreach (var (rpcRequest, inGameRequest, entity) in SystemAPI.Query<RefRO<ReceiveRpcCommandRequest>, RefRO<InGameRequestRPC>>().WithEntityAccess())
        {
            Entity connSourceEntity = rpcRequest.ValueRO.SourceConnection;

            ecb.AddComponent(connSourceEntity, new NetworkStreamInGame());

            ecb.DestroyEntity(entity);

            SpawnPlayer(connSourceEntity, entity, inGameRequest.ValueRO, ecb, ref state);
        }
    }

    private void SpawnPlayer(in Entity connectionEntity, in Entity rpcEntity, in InGameRequestRPC inGameRequest, in EntityCommandBuffer ecb, ref SystemState state)
    {
        Entity entityToInstaniate = inGameRequest.IsHost ? entitiesReferences.hostEntity : entitiesReferences.clientEntity; //Here we determine who is Host and set correct cube

        Entity spawnedEntity = ecb.Instantiate(entityToInstaniate);

        NetworkId networkID = SystemAPI.GetComponent<NetworkId>(connectionEntity);

        var random = Random.CreateFromIndex((uint)rpcEntity.Index ^ networkTime.ServerTick.TickIndexForValidTick + 1);

        if(inGameRequest.IsHost)
            ecb.AddComponent(spawnedEntity, new HostPlayerTag());

        ecb.SetComponent(spawnedEntity, LocalTransform.FromPosition(
            random.NextFloat3(
                new float3(-4f, 1f, -4f),
                new float3(4f, 1f, 4f)
            )));

        ecb.SetComponent(spawnedEntity, new GhostOwner
        {
            NetworkId = networkID.Value
        });
    }

}
