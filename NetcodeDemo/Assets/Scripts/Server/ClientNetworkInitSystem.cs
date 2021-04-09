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

// This group only exists on the client, effectively restricting this system to be client only.
[UpdateInGroup(typeof(ClientInitializationSystemGroup))]
public class ClientNetworkInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
#if UNITY_EDITOR
        // If running from the editor, we want to read the IP from the "Client auto connect address"
        // setting from the "Multiplayer PlayMode Tools" window. It defaults to localhost.
        var ep = NetworkEndPoint.Parse(ClientServerBootstrap.RequestedAutoConnect, 7979);
#else
        // If running from a standalone player, we connect to localhost. In a real world scenario,
        // we would allow the user to control this (multiplayer menu, matchmaking, lobby, etc.).
        var ep = NetworkEndPoint.LoopbackIpv4.WithPort(7979);
#endif
        // Connect.
        World.GetExistingSystem<NetworkStreamReceiveSystem>().Connect(ep);

        // We want the system to only run once, so we disable it immediately.
        Enabled = false;
    }
}
