using System;using System.Collections;
using System.Collections.Generic;
using UnityEditor.UI;
using UnityEngine;

public class CatapultManager
{
    private CatapultData catapultData;
    private ZombiesData zombiesData;


    // Delegate to instantiate gameobject - demonstrating the use of delegates
    public delegate GameObject InstantiateDelegate(GameObject prefab, Vector3 position, Quaternion rotation);
    private InstantiateDelegate instantiateCallback;

    public delegate void DestroyZombieDelegate(GameObject zombie);
    private DestroyZombieDelegate destroyZombieCallback;

    public CatapultManager(CatapultData catapultData, ZombiesData zombiesData, InstantiateDelegate instantiateDelegate, DestroyZombieDelegate destroyZombieDelegate)
    {
        this.catapultData = catapultData;
        this.zombiesData = zombiesData;
        instantiateCallback = instantiateDelegate;
        destroyZombieCallback = destroyZombieDelegate;
    }

    // Initialize default values
    public void Initialize()
    {   
        catapultData.startRotation = catapultData.catapultArm.eulerAngles;
        catapultData.bombDefaultPosition = catapultData.bomb.transform.position;
    }

    // Catapult handler - Catapult rotation
    public void HandleCatapult(bool isDragging, float startMouseY, float lastMouseY, Vector3 currentMousePosition)
    {
        if (isDragging)
        {
            float deltaY = currentMousePosition.y - startMouseY;
            float changeInY = currentMousePosition.y - lastMouseY;
            
            //Debug.Log("Change in Y: " + changeInY);
            if (Mathf.Abs(changeInY) > 0.1f)
            {
                //Debug.Log("Dragging");
                float rotationAmount = deltaY * catapultData.rotationSpeed;
            
                catapultData.catapultArm.Rotate(new Vector3(-rotationAmount, 0f, 0f));

                catapultData.shootPower = Mathf.Abs(deltaY * catapultData.powerMultiplier);

                catapultData.bombForce = catapultData.bombConstantForce * catapultData.shootPower;
            }
        }
        
        if (!isDragging)
        {
            catapultData.catapultArm.rotation = Quaternion.RotateTowards(catapultData.catapultArm.rotation, Quaternion.Euler(catapultData.startRotation), catapultData.shootPower);
        }
        
        ClampCatapultRotation();
    }

    private void ClampCatapultRotation()
    {
        Vector3 currentRotation = catapultData.catapultArm.eulerAngles;
        currentRotation.x = NormalizeAngle(currentRotation.x);
        if (currentRotation.x < 360f)
            currentRotation.x += 360f;
        currentRotation.x = Mathf.Clamp(currentRotation.x, 275f, 345f);
        
        catapultData.catapultArm.eulerAngles = currentRotation;
    }

    public void Shoot()
    {
        catapultData.bomb.transform.SetParent(null);
        catapultData.bomb.isKinematic = false;
        catapultData.bomb.AddForce(catapultData.catapultArm.forward * catapultData.bombForce, ForceMode.Impulse);
    }

    public void HandleBomb()
    {
        Vector3 origin = catapultData.bomb.position - (catapultData.bomb.transform.forward * 0.01f);
        Collider[] colliders = Physics.OverlapSphere(origin, 0.2f, catapultData.bombLayerMask);

        if (colliders.Length > 0)
        {
            instantiateCallback?.Invoke(catapultData.explosionFX,
                catapultData.bomb.position + new Vector3(0f, 0.08f, 0f), Quaternion.identity);

            Collider[] impactedZombies = Physics.OverlapSphere(catapultData.bomb.transform.position, 0.5f, catapultData.bombImpactLayerMask);
            
            catapultData.bomb.isKinematic = true;
            catapultData.bomb.velocity = Vector3.zero;
            catapultData.bomb.gameObject.SetActive(false);
            
            foreach (var zombie in impactedZombies)
            {
                //zombiesData.zombies.Remove(zombie);
                zombie.GetComponent<Animator>().SetTrigger("Die");
                destroyZombieCallback?.Invoke(zombie.gameObject);
                //GameManager.RemoveObject(zombie.gameObject, 4f);
            }
            
            ResetBomb();
        }
    }

    private void ResetBomb()
    {
        catapultData.bomb.transform.position = catapultData.bombDefaultPosition;
        catapultData.bomb.transform.SetParent(catapultData.catapultArm);
        catapultData.bomb.gameObject.SetActive(true);

        catapultData.canBombCheckCollision = false;
    }

    // Get the normalized angle
    float NormalizeAngle(float angle)
    {
        angle = angle % 360f;

        if (angle > 180)
            angle -= 360f;
        else if (angle < -180)
            angle += 360;

        return angle;
    }
    
}
