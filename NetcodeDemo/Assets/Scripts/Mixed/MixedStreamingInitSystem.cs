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

// Not putting this system in any particular group causes it to exist on both client and server.
public class MixedStreamingInitSystem : SystemBase
{
    private EntityQuery m_MissingStreamInGame;

    protected override void OnCreate()
    {
        // Ensuring this singleton exists is equivalent to waiting for the entity scene
        // to be fully loaded, which guarantees that the ghost prefab is loaded.
        RequireSingletonForUpdate<GameConfig>();

        m_MissingStreamInGame = GetEntityQuery(new EntityQueryDesc
        {
            All = new ComponentType[] {typeof(NetworkIdComponent)},
            None = new ComponentType[] {typeof(NetworkStreamInGame)}
        });

        // Because of the EntityQuery used by the singleton, we have to explicitly require
        // this one too, otherwise the system would keep running with nothing to process.
        RequireForUpdate(m_MissingStreamInGame);
    }

    protected override void OnUpdate()
    {
        // Adding the NetworkStreamInGame tag component to the connection entity
        // (on both the client and server sides) activates streaming.
        EntityManager.AddComponent<NetworkStreamInGame>(m_MissingStreamInGame);
    }
}
