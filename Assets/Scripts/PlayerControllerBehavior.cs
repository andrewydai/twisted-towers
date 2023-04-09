using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBehavior : MonoBehaviour
{
    public float stepSize;
    private GameObject currentBlock;
    public float tickLength;
    public string leftKey;
    public string rightKey;
    public string rotateClockWiseKey;
    public string rotateCounterClockWiseKey;
    private float currentTick;
    private string previousFramesKey;
    private IDictionary<string, Vector3> movementVectors;
    // TODO: figure out multiple keys being held 

    private void Start()
    {
        this.currentTick = 0;
        this.movementVectors = new Dictionary<string, Vector3>()
        {
            {leftKey, Vector3.left * this.stepSize },
            {rightKey, Vector3.right * this.stepSize },
        };
    }

    private void Update()
    {
        if (Input.GetKey(this.leftKey))
        {
            this.currentBlock.transform.position = this.currentBlock.transform.position + Vector3.left * this.stepSize;
        }
        else if (Input.GetKey(this.rightKey))
        {
            this.currentBlock.transform.position = this.currentBlock.transform.position + Vector3.right * this.stepSize;
        }
        else if (Input.GetKey(this.rotateClockWiseKey))
        {
            this.currentBlock.GetComponent<BlockBehavior>().Rotate();
        }
        //else if (Input.GetKey(this.rotateCounterClockWiseKey))
        //{

        //}
    }

    // Update is called once per frame
    //void Update()
    //{
    //    string currentKey = this.GetCurrentKey();

    //    if (currentKey == null)
    //    {
    //        this.currentTick = 0;
    //        return;
    //    }

    //    if (currentKey != this.previousFramesKey)
    //    {
    //        this.currentBlock.transform.position = this.currentBlock.transform.position + this.movementVectors[currentKey];
    //    }
    //    else
    //    {
    //        if (this.currentTick < this.tickLength)
    //        {
    //            this.currentTick += Time.deltaTime;
    //        }
    //        else
    //        {
    //            this.currentTick = 0;
    //            this.currentBlock.transform.position = this.currentBlock.transform.position + this.movementVectors[currentKey];
    //        }
    //    }

    //    this.previousFramesKey = currentKey;
    //}

    //private string GetCurrentKey()
    //{
    //    if (Input.GetKey(leftKey))
    //    {
    //        return leftKey;
    //    }
    //    if (Input.GetKey(rightKey))
    //    {
    //        return rightKey;
    //    }
    //    return null;
    //}

    public void SetBlock(GameObject block)
    {
        this.currentBlock = block;
    }
}
