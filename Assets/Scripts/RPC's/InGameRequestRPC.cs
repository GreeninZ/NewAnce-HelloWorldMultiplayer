using Unity.NetCode;

public struct InGameRequestRPC : IRpcCommand
{
    public bool IsHost;
}
