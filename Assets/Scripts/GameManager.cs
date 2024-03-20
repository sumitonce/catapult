using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public CatapultData catapultData;
    public ZombiesData zombiesData;
    public UIData uiData;

    private float input_StartMouseY;
    private float input_LastMouseY;
    
    private CatapultManager catapultManager;
    private ZombiesManager zombiesManager;

    private bool isGameStarted;
    private int zombiesKilled;
    private int targetToKillZombies = 7;


    private void Start()
    {
        uiData.startPanel.SetActive(true);
        
        // Creating instance of catapult manager
        catapultManager = new CatapultManager(catapultData, zombiesData, InstantiateObject, DestroyZombie);
        catapultManager.Initialize();

        // Creating instance of zombie manager
        zombiesManager = new ZombiesManager(zombiesData, catapultData.catapult, GameOver);
        //zombiesManager.Initialize();
    }

    private void Update()
    {
        // Wait until the game is started
        if (!isGameStarted)
            return;

        // Input to check if input dragging is started or not
        if (Input.GetMouseButtonDown(0))
        {
            catapultData.isDragging = true;
            input_StartMouseY = Input.mousePosition.y;
        }
        if (catapultData.isDragging)
        {
            // While dragging controlling the catapult arm
            catapultManager.HandleCatapult(true, input_StartMouseY, input_LastMouseY, Input.mousePosition);

            input_LastMouseY = Input.mousePosition.y;
        }

        if (Input.GetMouseButtonUp(0))
        {
            catapultData.isDragging = false;
            catapultData.isBombReleased = true;
        }
        
        // Controlling the bomb behaviour after shooting
        if (catapultData.isBombReleased)
        {
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
        zombiesKilled += 1;

        // Check if target number of zombies killed
        if (zombiesKilled >= targetToKillZombies)
        {
            GameWin();
        }

        StartCoroutine(ResetZombie(zombie));
    }

    // Use of another simple utility function to destroy the game object
    /*public static void RemoveObject(GameObject gameObjectToRemove, float time)
    {
        Destroy(gameObjectToRemove, time);
    }*/

    IEnumerator ResetZombie(GameObject zombie)
    {
        // Setting zombie colliders height and center to the smaller size
        CapsuleCollider zombieCollider = zombie.GetComponent<CapsuleCollider>();
        zombieCollider.center = new Vector3(zombieCollider.center.x, 0f, zombieCollider.center.z);
        zombieCollider.height = 0f;

        yield return new WaitForSeconds(4f);

        // Setting zombie colliders height and center to the default size
        zombieCollider.center = new Vector3(zombieCollider.center.x, zombiesData.zombieColliderBounds.yCenter,
            zombieCollider.center.z);
        zombieCollider.height = 2f; //zombiesData.zombieColliderBounds.height;
        
        // Adding the zombie back to the zombies list
        zombiesData.zombies.Add(zombie);
        zombie.transform.position = zombiesData.spawnPsotion;
        zombie.GetComponent<Animator>().SetTrigger("Reset");
    }

    private void StartGame()
    {
        isGameStarted = true;
    }

    private void GameWin()
    {
        isGameStarted = false;
        zombiesKilled = 0;
        
        uiData.gameWinPanel.SetActive(true);
    }

    private void GameOver()
    {
        Debug.Log("Game Over");
        isGameStarted = false;
        
        uiData.gameOverPanel.SetActive(true);
    }

    #region UI

    public void OnClick_StartGame()
    {
        uiData.startPanel.SetActive(false);
        uiData.tutorialHand.SetActive(true);
        
        Invoke(nameof(StartGame), 1f);
    }

    public void OnClick_RestartGame()
    {
        uiData.gameWinPanel.SetActive(false);
        uiData.gameWinPanel.SetActive(false);

        SceneManager.LoadScene(0);
    }


    #endregion
}
