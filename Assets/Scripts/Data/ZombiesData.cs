using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ZombiesData
{
    public List<GameObject> zombies;
    public float moveSpeed = 2f;
    public ZombieColliderBounds zombieColliderBounds;
    public Vector3 spawnPsotion;
}

[Serializable]
public struct ZombieColliderBounds
{
    public float height;
    public float yCenter;
}
