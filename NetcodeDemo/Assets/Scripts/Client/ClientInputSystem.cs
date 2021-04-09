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
[UpdateInGroup(typeof(GhostInputSystemGroup))]
public class ClientInputSystem : SystemBase
{
    private ClientSimulationSystemGroup m_ClientSimulationSystemGroup;

    protected override void OnCreate()
    {
        // Calling GetExistingSystem is expensive, so we cache the result in a field.
        m_ClientSimulationSystemGroup = World.GetExistingSystem<ClientSimulationSystemGroup>();
    }

    protected override void OnUpdate()
    {
        // The player input for the local client is located on the entity pointed at by
        // the CommandTargetComponent from the connection.
        var targetEntity = GetSingleton<CommandTargetComponent>().targetEntity;

        if (targetEntity == Entity.Null)
            return;

        var inputBuffer = GetBuffer<PlayerInput>(targetEntity);

        // AddCommandData is an extension method that adds an ICommandData to a dynamic
        // buffer, treating it like a ring buffer. Be careful not to use Add instead.
        inputBuffer.AddCommandData(new PlayerInput
        {
            // Every input stores the network tick at which it was recorded.
            Tick = m_ClientSimulationSystemGroup.ServerTick,

            // Note that we are using the "Raw" variants of the GetAxis functions,
            // because avoiding input smoothing will make the network artifacts more
            // visible. That's useful for a tutorial where the point is to understand
            // what's going on, but in a real scenario that would be the exact inverse.
            Movement = new float2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")),
            
            Jump = (Input.GetAxisRaw("Jump"))
        });
    }
}