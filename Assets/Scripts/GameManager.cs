using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CatapultData catapultData;
    public ZombiesData zombiesData;

    private float input_StartMouseY;
    private float input_LastMouseY;
    
    private CatapultManager catapultManager;
    private ZombiesManager zombiesManager;


    private void Start()
    {
        catapultManager = new CatapultManager(catapultData, zombiesData, InstantiateObject, DestroyZombie);
        catapultManager.Initialize();

        zombiesManager = new ZombiesManager(zombiesData, catapultData.catapult);
        zombiesManager.Initialize();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            catapultData.isDragging = true;
            input_StartMouseY = Input.mousePosition.y;
        }
        if (catapultData.isDragging)
        {
            catapultManager.HandleCatapult(true, input_StartMouseY, input_LastMouseY, Input.mousePosition);

            input_LastMouseY = Input.mousePosition.y;
        }

        if (Input.GetMouseButtonUp(0))
        {
            catapultData.isDragging = false;
            catapultData.isBombReleased = true;
        }

        if (catapultData.isBombReleased)
        {
            //catapultData.isBombReleased = false;
            catapultData.canBombCheckCollision = true;
            catapultManager.HandleCatapult(false, input_StartMouseY, input_LastMouseY, Input.mousePosition);
            
            if (Quaternion.Angle(catapultData.catapultArm.rotation, Quaternion.Euler(catapultData.startRotation)) < 0.1f)
            {
                catapultData.isBombReleased = false;
                catapultManager.Shoot();
                
                Debug.Log("Cannon Force: " + catapultData.bombForce);
            }
        }

        if (catapultData.canBombCheckCollision)
        {
            catapultManager.HandleBomb();
        }
        
        zombiesManager.HandleZombies();
    }

    // Demonstrating how we can use ths function with delegate
    private GameObject InstantiateObject(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        return Instantiate(prefab, position, rotation);
    }

    private void DestroyZombie(GameObject zombie)
    {
        StartCoroutine(ResetZombie(zombie));
    }

    // Use of another simple utility function to destroy the game object
    public static void RemoveObject(GameObject gameObjectToRemove, float time)
    {
        Destroy(gameObjectToRemove, time);
    }

    IEnumerator ResetZombie(GameObject zombie)
    {
        yield return new WaitForSeconds(4f);
        
        zombie.transform.position = zombiesData.spawnPsotion;
        zombie.GetComponent<Animator>().SetTrigger("Reset");
    }
}
