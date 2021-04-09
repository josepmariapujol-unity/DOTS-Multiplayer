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
public class ClientThinInputInitSystem : SystemBase
{
    protected override void OnCreate()
    {
        // Thin clients have ThinClientComponent singleton, by requiring it for update
        // we ensure this system only runs on thin clients.
        RequireSingletonForUpdate<ThinClientComponent>();

        // Since we are on a client, there is only one CommandTargetComponent to be found,
        // but we should ensure it exists before the system runs so we can replace it.
        RequireSingletonForUpdate<CommandTargetComponent>();
    }

    protected override void OnUpdate()
    {
        // Usually the command target is a ghost, but nothing mandates it. Since we only
        // care about have an input buffer in the thin client, we create an entity with
        // only that component, and we set it as our command target.
        var entity = EntityManager.CreateEntity(typeof(PlayerInput));
        SetSingleton(new CommandTargetComponent {targetEntity = entity});

        // This system does initialization and should only run once, so we can disable it.
        Enabled = false;
    }
}
