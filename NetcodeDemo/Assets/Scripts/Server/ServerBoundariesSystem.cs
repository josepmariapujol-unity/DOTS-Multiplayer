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
public class ServerBoundariesSystem : SystemBase
{
    protected override void OnUpdate()
    {
        // Define the 2D boundaries of the playing field.
        var min = new float2(-10, -5);
        var max = new float2(+10, +5);

        // Iterate over all ghosts, clamping their translations to the area defined above.
        Entities
            .WithAll<GhostComponent>()
            .ForEach((ref Translation translation) =>
            {
                translation.Value.xz = math.clamp(translation.Value.xz, min, max);

                // Also draw a line at the capsule position, to visualize server state.
                Debug.DrawLine(translation.Value, translation.Value + new float3(0, 2, 0));
            }).Run();

        // Debug lines to visualize the boundaries in the scene view.
        var a = new float3(min.x, 0, min.y);
        var b = new float3(max.x, 0, min.y);
        var c = new float3(max.x, 0, max.y);
        var d = new float3(min.x, 0, max.y);
        Debug.DrawLine(a, b);
        Debug.DrawLine(b, c);
        Debug.DrawLine(c, d);
        Debug.DrawLine(d, a);
    }
}