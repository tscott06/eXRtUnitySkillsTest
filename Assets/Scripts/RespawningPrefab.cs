using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRespawningPrefab", menuName = "Respawn/RespawningPrefab"), Obsolete]
public class RespawningPrefab : ScriptableObject
{
    [SerializeField] GameObject prefab;
    [SerializeField] string spawnID;
}
