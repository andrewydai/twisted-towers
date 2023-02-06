using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBehavior : MonoBehaviour
{
    public GameObject[] blockPrefabs;
    private List<BlockSpawnBehavior> spawners;
    private BlockGenerator blockGenerator;

    // Start is called before the first frame update
    void Start()
    {
        this.blockGenerator = new BlockGenerator(this.blockPrefabs);
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
            blocks[i] = this.blockGenerator.UpdateAndGetNextBlock();
            this.blockGenerator.Print();
        }
        return blocks;
    }
}

public class BlockGenerator
{
    // the current total count, used for calculating probabilities.
    // It essnetially represents (number of blocks spawned) * (number of type of blocks - 1)
    // This is because each time a block spawns, we want to add to the count of all other blocks, 
    // incrementing the count by (number of type of blocks - 1), i.e. 6 if we have 7 types of blocks
    private int totalCount;
    // an array of all the block spawn probability instances
    private BlockProbabilityRange[] blockProbabilityArr;

    public BlockGenerator(GameObject[] blockPrefabs)
    {
        int blockCount = blockPrefabs.Length;
        this.blockProbabilityArr = new BlockProbabilityRange[blockCount];
        // our initial total count is the (number of type of blocks), which creates an initial probability
        // of all block spawns as 1 / (number of type of blocks) i.e. 1/7
        this.totalCount = blockCount;
        // our initial prevUpperRange is 0, so for the first block, the range it covers would be [0, ~0.143] (1/7)
        float prevUpperRange = 0f;
        // for each block type
        for(int i = 0; i < blockCount; i++)
        {
            // make a BlockProbabilityRange instance with the current prevUpperRange, initial total count, and prefab
            BlockProbabilityRange blockProbRange =
                new BlockProbabilityRange(blockPrefabs[i], this.totalCount, prevUpperRange);
            // add it to our array
            this.blockProbabilityArr[i] = blockProbRange;
            // update the prev range 
            prevUpperRange = blockProbRange.upperRange;
        }
    }

    public GameObject UpdateAndGetNextBlock()
    {
        // to avoid iterating twice, we find the next block and update in the same pass of our array
        // this requires us to calculate some things now like the next total count
        int nextTotalCount = this.totalCount + this.blockProbabilityArr.Length - 1;
        // calculate the next block by grabbing a random number between 0 - 1 (and seeing what range it falls in)
        float randomNum = Random.Range(0f, 1f);
        // like the constructor, our initial range is 0f, and will be used to update the probabilities
        float prevUpperRange = 0f;
        GameObject nextBlock = null;
        // for each block probability range instance
        for (int i = 0; i < this.blockProbabilityArr.Length; i++)
        {
            BlockProbabilityRange blockProb = this.blockProbabilityArr[i];
            // determine whether this block is being selected as the next block i.e. does it's range contain the
            // random number generated
            bool isNext = false;
            if ((randomNum <= blockProb.upperRange || i == this.blockProbabilityArr.Length - 1) && nextBlock == null)
            {
                nextBlock = blockProb.prefab;
                isNext = true;
            }
            // update the blocks probability of being selected and range
            blockProb.UpdateRange(nextTotalCount, prevUpperRange, isNext);
            prevUpperRange = blockProb.upperRange;
        }
        // update the totalCount and return the selected block's prefab
        totalCount = nextTotalCount;
        return nextBlock;
    }

    // method to help debug with prints
    public void Print()
    {
        string printString = " | ";
        float prevUpperRange = 0;
        for (int i = 0; i < this.blockProbabilityArr.Length; i++)
        {
            BlockProbabilityRange block = this.blockProbabilityArr[i];
            printString += block.Print(prevUpperRange) + " | ";
            prevUpperRange = block.upperRange;
        }
        Debug.Log(printString);
    }
}

public class BlockProbabilityRange
{
    // block's prefab
    public GameObject prefab;
    // current count, essentially the number of times other blocks have been chosen
    private int currentCount;
    // the upper range of the range that this block's probabiltiy lies in. Essentially,
    // the size of the range should increase the more it gets passed over in the selection
    // phase. Note that the range increasing =/= upperRange increasing
    public float upperRange;

    public BlockProbabilityRange(GameObject prefab, int totalCount, float prevUpperRange)
    {
        this.prefab = prefab;
        this.currentCount = 0;
        // update the range initially, this is intended to give an initial 1 / totalCount probability
        this.UpdateRange(totalCount, prevUpperRange, false);
    }

    public void UpdateRange(float totalCount, float prevUpperRange, bool isNext)
    {
        // if this block is next, don't increment it
        if (!isNext)
        {
            this.currentCount++;
        }
        // recalculate the probability with the added counts and the upperRange
        float probability = this.currentCount / totalCount;
        this.upperRange = probability + prevUpperRange;
    }

    // method to help debug with prints
    public string Print(float prevUpperRange)
    {
        float probability = this.upperRange - prevUpperRange;
        return this.prefab.GetInstanceID() + ":::" + probability;
    }
}