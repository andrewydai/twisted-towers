using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnBehavior : MonoBehaviour
{
    public float blockDropSpeed;
    public PlayerControllerBehavior playerControllerBehavior;
    public GameManagerBehavior gameManagerBehavior;
    private Queue<GameObject> nextBlocks = new Queue<GameObject>();


    public void SpawnBlock()
    {
        var block = Instantiate(this.nextBlocks.Dequeue(), transform.position, transform.rotation);
        playerControllerBehavior.SetBlock(block);
        var blockBehavior = block.GetComponent<BlockBehavior>();
        blockBehavior.setBlockSpeed(blockDropSpeed);
        blockBehavior.setSpawnBehavior(this);

        if (this.nextBlocks.Count <= 2)
        {
            this.gameManagerBehavior.AddBlocksToSpawnerQueue();
        }
    }

    public void AddToQueue(GameObject[] blocksToAdd)
    {
        foreach (GameObject block in blocksToAdd)
        {
            this.nextBlocks.Enqueue(block);
        }
    }
}
