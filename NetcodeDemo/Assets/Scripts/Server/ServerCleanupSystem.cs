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
public class ServerCleanupSystem : SystemBase
{
    private EntityQuery m_ClosedConnections;

    protected override void OnCreate()
    {
        // Closed connections awaiting cleanup are missing the NetworkIdComponent
        // but still have the system state component SpawnedPlayer.
        m_ClosedConnections = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(SpawnedPlayer)},
            None = new ComponentType[] {typeof(NetworkIdComponent)}
        });
    }

    protected override void OnUpdate()
    {
        // Extract all SpawnedPlayer components from the query. Each one only contains
        // a single Entity field, so the array can be aliased as an array of entities.
        var players = m_ClosedConnections.ToComponentDataArray<SpawnedPlayer>(Allocator.Temp);
        EntityManager.DestroyEntity(players.Reinterpret<Entity>());

        // Removing the system state component will effectively destroy the entity.
        EntityManager.RemoveComponent<SpawnedPlayer>(m_ClosedConnections);
    }
}
