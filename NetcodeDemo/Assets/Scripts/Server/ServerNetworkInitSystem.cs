using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Transforms;
using UnityEngine;

// This group only exists on the server, effectively restricting this system to be server only.
[UpdateInGroup(typeof(ServerInitializationSystemGroup))]
public class ServerNetworkInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Accept connections from any IP on port 7979 (chosen arbitrarily).
        var ep = NetworkEndPoint.AnyIpv4.WithPort(7979);

        // Start listening for incoming connections.
        World.GetExistingSystem<NetworkStreamReceiveSystem>().Listen(ep);

        // We want the system to only run once, so we disable it immediately.
        Enabled = false;
    }
}
