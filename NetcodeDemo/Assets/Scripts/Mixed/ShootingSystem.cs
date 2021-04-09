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

[UpdateInGroup(typeof(GhostPredictionSystemGroup))]
public class ShootingSystem : SystemBase
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
        var deltaTime = Time.DeltaTime;

        
        Entities
            .ForEach((DynamicBuffer<PlayerInput> playerInput
                , ref Translation translation
                , ref Rotation rotation
                , in PredictedGhostComponent prediction) =>
            {
                var enemyPos = new float3(3, 0, 3);

                var directionToShoot =
                    new float3(enemyPos.x - translation.Value.x, 0, enemyPos.z - translation.Value.z);
                
                //Debug.Log(directionToShoot + ": directionToShoot");
                
                //Debug.Log(translation.Value + ": position");

                directionToShoot = math.normalize(directionToShoot);
                
                rotation.Value = math.slerp(rotation.Value, quaternion.LookRotation(directionToShoot, math.up()),
                    deltaTime);
                
                
            }).Schedule();
    }
}
