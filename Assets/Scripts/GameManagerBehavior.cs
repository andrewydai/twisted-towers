using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    private List<BlockSpawnBehavior> spawners;

    // Start is called before the first frame update
    void Start()
    {
        GameObject[] initialBlocks = RandomizeBlocks(10);
        this.spawners = new List<BlockSpawnBehavior>();
        var spawnerObjects = GameObject.FindGameObjectsWithTag("BlockSpawner");
        foreach (GameObject spawnerObject in spawnerObjects)
        {
            BlockSpawnBehavior spawnBehavior = spawnerObject.GetComponent<BlockSpawnBehavior>();
            spawnBehavior.AddToQueue(initialBlocks);
            spawnBehavior.SpawnBlock();
            this.spawners.Add(spawnBehavior);
        }
    }

    private GameObject[] RandomizeBlocks(int blockCount)
    {
        GameObject[] blocks = new GameObject[blockCount];
        for (int i = 0; i < blockCount; i++)
        {
            blocks[i] = this.blockPrefabs[Random.Range(0, this.blockPrefabs.Length)];
        }
        return blocks;
    }
}
