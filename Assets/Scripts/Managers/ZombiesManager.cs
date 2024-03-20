using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesManager
{
    private ZombiesData zombiesData;
    private Transform target;

    public delegate void GameOverDelegate();

    public GameOverDelegate gameOverCallback;

    public ZombiesManager(ZombiesData zombiesData, Transform target, GameOverDelegate gameOverDelegate)
    {
        this.zombiesData = zombiesData;
        this.target = target;
    }

    public void Initialize()
    {
        /*foreach (var zombie in zombiesData.zombies)
        {
            zombie.spawnPosition = zombie.zombieAI.transform.position;
        }*/
    }

    public void HandleZombies()
    {
        foreach (var zombie in zombiesData.zombies)
        {
            if (zombie != null)
            {
                Vector3 targretDirection = zombie.transform.position - target.position;
                targretDirection.Normalize();
                zombie.transform.Translate(targretDirection * zombiesData.moveSpeed);

                if (zombie.transform.position.z <= 0.2f)
                {
                    gameOverCallback?.Invoke();
                }
            }
        }
    }
}
