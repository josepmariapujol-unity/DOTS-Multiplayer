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
[UpdateInGroup(typeof(ServerSimulationSystemGroup))]
public class ServerSpawnSystem : SystemBase
{
    // The system should only run when there are new connections to process.
    // But because it also needs a singleton that will always exist, if we don't
    // explicitly require the connection query the system would keep running indefinitely.
    private EntityQuery NewConnectionsQuery;

    protected override void OnCreate()
    {
        RequireSingletonForUpdate<GameConfig>();
        RequireForUpdate(NewConnectionsQuery);
    }

    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        var playerPrefab = GetSingleton<GameConfig>().PlayerPrefab;

        Entities
            .WithStoreEntityQueryInField(ref NewConnectionsQuery)
            .WithNone<SpawnedPlayer>()
            .ForEach((Entity connectionEntity, in NetworkIdComponent networkId) =>
            {
                var player = ecb.Instantiate(playerPrefab);

                // The SpawnedPlayer component stores the prefab for cleanup on
                // disconnection. But the presence of the component also allows the
                // system to detect which connections didn't have a prefab spawned yet.
                ecb.AddComponent(connectionEntity, new SpawnedPlayer {Value = player});

                // Put all the spawned players at an offset to make it easy to check how
                // many clients are connected.
                ecb.SetComponent(player, new Translation {Value = new float3(0, 0, networkId.Value)});
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}