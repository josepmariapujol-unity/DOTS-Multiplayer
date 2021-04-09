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
[UpdateInGroup(typeof(ClientSimulationSystemGroup))]
public class ClientInputInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Prevents the system from running before a connection is established.
        RequireSingletonForUpdate<NetworkIdComponent>();
    }

    protected override void OnUpdate()
    {
        var connectionEntity = GetSingletonEntity<NetworkIdComponent>();
        var networkId = GetComponent<NetworkIdComponent>(connectionEntity).Value;
        var ecb = new EntityCommandBuffer(Allocator.Temp);
        bool found = false;

        Entities
            .ForEach((Entity ghostEntity, in GhostOwnerComponent owner) =>
            {
                if (owner.NetworkId == networkId)
                {
                    found = true;

                    // The input buffer is just a regular dynamic buffer.
                    ecb.AddBuffer<PlayerInput>(ghostEntity);

                    // A ghost owner component simply stores the network ID for the client
                    // connection that owns the ghost. This value is set on the server and
                    // will be automatically replicated to the clients.
                    ecb.SetComponent(connectionEntity, new CommandTargetComponent
                    {
                        targetEntity = ghostEntity
                    });

                    // The name of an entity is a debug only feature, but when looking
                    // at the ghosts in the "Window > DOTS > Entities" window this name
                    // will make it very easy to figure out which one is "our" ghost.
                    ecb.SetName(ghostEntity, "Player (local)");
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();

        // As soon as we found "our" ghost, we can disable the system update.
        if (found) Enabled = false;
    }
}