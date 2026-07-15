using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.NetCode;

[WorldSystemFilter(WorldSystemFilterFlags.ClientSimulation)]
partial struct SendInGameRequestClientSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<NetworkId>();
    }

    //[BurstCompile] Due to ClientServerBootstrap class usage 
    public void OnUpdate(ref SystemState state)
    {
        var entitiesNonInGame = SystemAPI.QueryBuilder().WithAll<NetworkId>().WithNone<NetworkStreamInGame>().Build().ToEntityArray(Allocator.Temp);

        EntityCommandBuffer ecb = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>().CreateCommandBuffer(state.WorldUnmanaged);

        var IsHost = ClientServerBootstrap.ServerWorld != null; //a way to check if a player is a host

        foreach (var entity in entitiesNonInGame)
        {
            ecb.AddComponent(entity, new NetworkStreamInGame());

            Entity rpcEntity = ecb.CreateEntity();
            ecb.AddComponent(rpcEntity, new SendRpcCommandRequest());
            ecb.AddComponent(rpcEntity, new InGameRequestRPC
            {
                IsHost = IsHost //we send it to RPC to receive it on the server
            });
        }
    }
}
