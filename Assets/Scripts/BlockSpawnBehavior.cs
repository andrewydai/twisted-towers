using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnBehavior : MonoBehaviour
{
    public float blockDropSpeed;
    private Queue<GameObject> nextBlocks = new Queue<GameObject>();

    public void SpawnBlock()
    {
        var block = Instantiate(this.nextBlocks.Dequeue(), transform.position, transform.rotation);
        var blockBehavior = block.GetComponent<BlockBehavior>();
        blockBehavior.setBlockSpeed(blockDropSpeed);
        blockBehavior.setSpawnBehavior(this);
    }

    public void AddToQueue(GameObject[] blocksToAdd)
    {
        Debug.Log(blocksToAdd[0]);
        foreach (GameObject block in blocksToAdd)
        {
            this.nextBlocks.Enqueue(block);
        }
    }
}
