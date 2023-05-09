using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawnBehavior : MonoBehaviour
{
    public float blockBaseDropSpeed;
    public float maxBlockDropSpeed;
    public float blockAcceleration;
    public PlayerControllerBehavior playerControllerBehavior;
    public GameManagerBehavior gameManagerBehavior;
    private Queue<GameObject> nextBlocks = new Queue<GameObject>();


    public void SpawnBlock(float prevSpeed = 0)
    {
        float startSpeed = prevSpeed == 0 ? this.blockBaseDropSpeed : prevSpeed;
        var block = Instantiate(this.nextBlocks.Dequeue(), transform.position, transform.rotation);
        this.playerControllerBehavior.SetBlock(block);
        var blockBehavior = block.GetComponent<BlockBehavior>();
        blockBehavior.SetBlockSpeedAttrs(startSpeed, this.blockBaseDropSpeed, this.maxBlockDropSpeed, this.blockAcceleration);
        blockBehavior.SetSpawnBehavior(this);

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
