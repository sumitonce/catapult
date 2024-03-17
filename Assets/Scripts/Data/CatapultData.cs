using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class CatapultData
{
    
    [Header("Catapult")]
    public Transform catapult;
    public Transform catapultArm;
    public LayerMask bombLayerMask;
    public LayerMask bombImpactLayerMask;
    public float rotationSpeed = 50f;
    public float armResetSpeed = 1f;
    public float powerMultiplier = 1f;
    public float bombConstantForce = 15f;

    [Header("Projectile")] 
    public Rigidbody bomb;
    public GameObject explosionFX;

    [HideInInspector]
    public float shootPower;
    [HideInInspector]
    public Vector3 startRotation;
    [HideInInspector]
    public bool isDragging;
    [HideInInspector]
    public bool isBombReleased;
    [HideInInspector]
    public bool canBombCheckCollision;
    [HideInInspector]
    public float bombForce;
    [HideInInspector]
    public Vector3 bombDefaultPosition;
}
