using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.NetCode;
using Unity.Networking.Transport;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;

// This group exists on both client and server, this system will thus run on both.
[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class MixedSimulationSystem : SystemBase
{
    private GhostPredictionSystemGroup m_ServerSimulationSystemGroup;

    protected override void OnCreate()
    {
        // Calling GetExistingSystem is expensive, so we cache the result in a field.
        m_ServerSimulationSystemGroup = World.GetExistingSystem<GhostPredictionSystemGroup>();
    }

    protected override void OnUpdate()
    {
        // In the event we have multiple ticks to run to catch up with ServerTick,
        // this system will be updated multiples times with sequential values of
        // PredictingTick counting up to ServerTick.
        var tick = m_ServerSimulationSystemGroup.PredictingTick;

        // The Time property is adjusted by the system group so its value remains
        // consistent through the iteration. In other words, don't worry about it.
        var deltaTime = Time.DeltaTime * 5;

        Entities
            .ForEach((DynamicBuffer<PlayerInput> playerInput
                , ref Translation translation
                , ref PhysicsVelocity physics
                , in PredictedGhostComponent prediction) =>
            {
                // When a system updates an entity can already have a newer state than
                // the system is currently predicting, in which case it must not run
                // for that specific entity. This test has no effect on the server,
                // since the server will never roll back.
                if (!GhostPredictionSystemGroup.ShouldPredict(tick, prediction))
                    return;

                // If no data is available, it means we haven't received anything from the
                // client. If the input buffer is starved, the last input is extrapolated.
                if (playerInput.GetDataAtTick(tick, out var data))
                {
                    translation.Value.xz += data.Movement * deltaTime;

                    physics.Linear.y += data.Jump;
                }
                
            }).Schedule();
    }
}