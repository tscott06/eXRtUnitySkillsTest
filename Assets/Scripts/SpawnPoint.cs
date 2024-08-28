using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SpawnPoint : MonoBehaviour
{
    public GameObject prefabToSpawn;
    public Transform spawnTransform;

    public bool SpawnAtStart;

    private GameObject spawnInstance;

    public UnityEvent onSpawn;
    public UnityEvent onDespawn;


    private void Start()
    {
        if(spawnTransform == null)
            spawnTransform = transform;

        if (SpawnAtStart)
            Spawn();
    }

    public void Spawn()
    {
        if(prefabToSpawn == null)
        {
            Debug.LogError("No prefab set to spawn!", this);
            return;
        }

        if(spawnInstance != null)
        {
            Debug.LogError("Already spawned!", this);
            return;
        }

        spawnInstance = Instantiate(prefabToSpawn, spawnTransform.position, spawnTransform.rotation);
    }

    public void Despawn()
    {
        Destroy(spawnInstance);
    }

    public void Respawn()
    {
        Despawn();
        Spawn();
    }

  
}
