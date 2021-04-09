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

[GhostComponent(OwnerSendType = SendToOwnerType.SendToNonOwner)]
public struct PlayerInput : ICommandData
{
    [GhostField] public uint Tick { get; set; }
    [GhostField] public float2 Movement;   
    
    [GhostField] public float Jump;
}