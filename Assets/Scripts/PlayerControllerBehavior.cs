using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControllerBehavior : MonoBehaviour
{
    public float stepSize;
    private GameObject currentBlock;
    public float tickLength;
    // key vars
    public string leftKey;
    public string rightKey;
    public string rotateClockWiseKey;
    private string prevDownKey;
    private string[] keyList;
    private string[] holdableKeys;
    // action threshold vars
    public float maxActionThresholdTime;
    public float minActionThresholdTime;
    public float thresholdDecrementRate;
    private float keyHoldTime;
    private float actionThresholdTime;
    
    private void Start()
    {
        // instantiate the list of all keys
        this.keyList = new string[] { this.leftKey, this.rightKey, this.rotateClockWiseKey };
        // instantiate the list of keys that can be held down
        this.holdableKeys = new string[] { this.leftKey, this.rightKey };
        // instantiate initial values of the action threshold
        this.keyHoldTime = 0;
        this.actionThresholdTime = maxActionThresholdTime;
    }

    private void Update()
    {
        // keep track of a list of keys that were pressed down this frame
        // and keys that are held down this frame
        List<string> currentDownKeys = new List<string>();
        List<string> currentHeldKeys = new List<string>();
        // keep track of whether a key being held down this frame was 
        // the most recent pressed down key
        bool matchedPrevDownKey = false;
        // for each key in the list of keys we have
        foreach (string key in this.keyList)
        {
            // if the key is being held down
            if (Input.GetKey(key))
            {
                // add it to the key held list
                currentHeldKeys.Add(key);
                // and if it matches the most recent key pressed down
                if (key == this.prevDownKey)
                {
                    // record that that key is held down still
                    matchedPrevDownKey = true;
                }
            }
            // if the key is pressed down this frame, add it to the key down list
            if (Input.GetKeyDown(key))
            {
                currentDownKeys.Add(key);
            }
        }

        // now we resolve which key we want to do resolve for this frame. The order of preference is:
        // 1. key is pressed down this frame
        // 2. key is held down and was the most recent pressed down
        // 3. key is held down
        // in the case of ties we arbitrarily choose the first
        if (currentDownKeys.Count > 0)
        {
            this.ResolveDownKey(currentDownKeys[0]);
        }

        else if (matchedPrevDownKey)
        {
            this.ResolveHeldKey(this.prevDownKey);
        }

        else if (currentHeldKeys.Count > 0)
        {
            this.ResolveHeldKey(currentHeldKeys[0]);
        }
    }

    private void ResolveDownKey(string key)
    {
        // if the key is pressed down, resolve it without condition
        this.prevDownKey = key;
        this.ResolveInput(key);
        // reset any variables related to action threshold
        this.keyHoldTime = 0;
        this.actionThresholdTime = maxActionThresholdTime;
    }

    private void ResolveHeldKey(string key)
    {
        // if the key is held down, check that it's a holdable key
        if(Array.IndexOf(this.holdableKeys, key) > -1)
        {
            // if the key has been held for longer than the action threshold
            // resolve the action and reset
            float currentKeyHoldTime = this.keyHoldTime + Time.deltaTime;
            if (currentKeyHoldTime >= actionThresholdTime)
            {
                this.ResolveInput(key);
                this.keyHoldTime = 0;
                // decrement the threshold until it reaches the minimum (accelerates)
                this.DecrementThreshold();
            }
            // else increment the holding time
            else
            {
                this.keyHoldTime = currentKeyHoldTime;
            }
        }
    }

    private void ResolveInput(string input)
    {
        if (input == this.leftKey)
        {
            this.currentBlock.transform.position = this.currentBlock.transform.position + Vector3.left * this.stepSize;
        }
        else if (input == this.rightKey)
        {
            this.currentBlock.transform.position = this.currentBlock.transform.position + Vector3.right * this.stepSize;
        }
        else if (input == this.rotateClockWiseKey)
        {
            this.currentBlock.GetComponent<BlockBehavior>().Rotate();
        }
    }

    private void DecrementThreshold()
    {
        if (this.actionThresholdTime > this.minActionThresholdTime)
        {
            float nextActionThresholdTime = this.actionThresholdTime - thresholdDecrementRate;
            this.actionThresholdTime = Mathf.Max(nextActionThresholdTime, this.minActionThresholdTime);
        }
    }

    public void SetBlock(GameObject block)
    {
        this.currentBlock = block;
    }
}
