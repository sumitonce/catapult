using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombiesManager
{
    private ZombiesData zombiesData;
    private Transform target;

    public ZombiesManager(ZombiesData zombiesData, Transform target)
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
            }
        }
    }
}
