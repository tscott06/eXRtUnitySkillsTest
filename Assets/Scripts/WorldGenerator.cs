using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldGenerator : MonoBehaviour
{
    [SerializeField] private Vector2Int gridDimensions;
    [SerializeField] private float tileSize = 1;

    [SerializeField] private List<TileData> possibleTiles;


    private List<GameObject> spawnedGOs = new List<GameObject>();
    //private Dictionary<Vector2Int, GameObject> spawnedGODictionary = new Dictionary<Vector2Int, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        GenerateGrid();
    }

    private void Update()
    {
       if(Input.GetKeyDown(KeyCode.G))
        {
            GenerateGrid();
        }
    }

    public void GenerateGrid()
    {
        ClearGrid();

        int tileCount = possibleTiles.Count;

        float startXOffset = gridDimensions.x * .5f * tileSize * -1;
        float startYOffset = gridDimensions.y * .5f * tileSize * -1;

        Vector2 startGridPosition = (Vector2)transform.position - new Vector2(startXOffset, startYOffset);
        Vector2 currentGridPostion = startGridPosition;

        for (int i = 0; i < gridDimensions.x; i++)
        {
            for(int j = 0; j < gridDimensions.y; j++)
            {
                int randomTileIndex = Random.Range(0, tileCount);

                TileData tileToUse = possibleTiles[randomTileIndex];
                GameObject PrefabToSpawn = tileToUse.Prefab;

                Vector3 SpawnPoint = new Vector3(currentGridPostion.x, 0, currentGridPostion.y);

                int rotateToApply = 0;

                if (tileToUse.Rules.CanRotate)
                {
                    rotateToApply = 90 * Random.Range(0, 3);
                }

                Quaternion rotation = Quaternion.Euler(0, rotateToApply, 0);

                Instantiate(PrefabToSpawn, SpawnPoint, rotation, transform);
                spawnedGOs.Add(gameObject);

                currentGridPostion.x += tileSize;
            }

            currentGridPostion.y += tileSize;
            currentGridPostion.x = startGridPosition.x;
        }
    }

    public void ClearGrid()
    {
        foreach (GameObject item in spawnedGOs) 
        {
            Destroy(item);
        }

        spawnedGOs.Clear();
    }
}
