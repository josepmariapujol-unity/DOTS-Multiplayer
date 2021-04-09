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

[GenerateAuthoringComponent]
public struct GameConfig : IComponentData
{
    public Entity PlayerPrefab;
}