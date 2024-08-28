using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviourSingleton<SpawnManager>
{
    private List<SpawnPoint> spawnPoints;

    public void RegisterSpawnPoint(SpawnPoint newSpawnPoint)
    {
        if (spawnPoints.Contains(newSpawnPoint))
        {
            Debug.LogError("Spawn point already registered!");
            return;
        }

        spawnPoints.Add(newSpawnPoint);
    }

    public void DeregisterSpawnPoint(SpawnPoint spawnPoint)
    {
        if (!spawnPoints.Contains(spawnPoint))
        {
            Debug.LogError("This spawn point has not been registered");
            return;
        }

        spawnPoints.Remove(spawnPoint);
    }

    public void RespawnFromSuitablePoint()
    {
       
    }
}
