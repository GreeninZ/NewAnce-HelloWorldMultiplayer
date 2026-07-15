using Unity.NetCode;

public struct InGameRequestRPC : IRpcCommand
{
    public bool IsHost; //Used for later determination of host system to use correct cube (red one)
}
