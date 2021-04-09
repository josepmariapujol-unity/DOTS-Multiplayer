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
public class ServerInputInitSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var ecb = new EntityCommandBuffer(Allocator.Temp);

        Entities
            .WithChangeFilter<CommandTargetComponent>()
            .ForEach((Entity connectionEntity
                , ref CommandTargetComponent commandTarget
                , in NetworkIdComponent networkId
                , in SpawnedPlayer player) =>
            {
                // The purpose of the CommandTargetComponent is to indicate the entity where
                // the input buffer associate with this connection is located, we use that
                // value as a way to detect if this connection has already been initialized.
                if (commandTarget.targetEntity == Entity.Null)
                {
                    // The input buffer is just a regular dynamic buffer.
                    ecb.AddBuffer<PlayerInput>(player.Value);

                    // A ghost owner component simply stores the network ID for the client
                    // connection that owns the ghost. This value is set on the server and
                    // will be automatically replicated to the clients.
                    ecb.SetComponent(player.Value, new GhostOwnerComponent
                    {
                        NetworkId = networkId.Value
                    });

                    // Indicate that the player ghost is the entity containing the input buffer.
                    commandTarget.targetEntity = player.Value;
                }
            }).Run();

        ecb.Playback(EntityManager);
        ecb.Dispose();
    }
}
