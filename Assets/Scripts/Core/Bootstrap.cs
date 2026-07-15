using UnityEngine;
using Unity.NetCode;
public class Bootstrap : ClientServerBootstrap
{
    public override bool Initialize(string defaultWorldName)
    {
        AutoConnectPort = 7979; // Unity's default

        return base.Initialize(defaultWorldName);
    }
}
