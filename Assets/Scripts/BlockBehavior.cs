using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    public GameObject highlight;
    public float width;
    public float rotatedWidth;
    
    private float baseBlockSpeed;
    private float maxBlockSpeed;
    private float currentBlockSpeed;
    private float blockAcceleration;
    private bool isAccelerating;
    private float tickTime;

    private bool isDropping;
    private float despawnY = -12;
    private GameObject highlightObj;
    private BlockSpawnBehavior spawnBehavior;
    private Quaternion initRotation;

    // Start is called before the first frame update
    void Start()
    {
        this.highlightObj = Instantiate(this.highlight, this.transform);
        this.highlightObj.transform.localPosition = Vector3.forward;
        this.initRotation = this.transform.rotation;
        this.SetHighlightWidth(this.width);
        this.isDropping = true;
        // if this is undefined, set it to false, else keep it what it was (fixes some strange ordering issues)
        this.isAccelerating = !this.isAccelerating ? false : this.isAccelerating;
        this.tickTime = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDropping)
        {
            float step = this.currentBlockSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, this.despawnY), step);

            float currentTickTime = this.tickTime + Time.deltaTime;
            if (currentTickTime >= this.blockAcceleration)
            {
                this.ChangeSpeed();
            }
            else
            {
                this.tickTime = currentTickTime;
            }
        }

        if (transform.position.y <= despawnY)
        {
            GameObject.Destroy(gameObject);
        }
    }
        

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (this.isDropping)
        {
            this.isDropping = false;
            gameObject.GetComponent<Rigidbody2D>().gravityScale = 3;
            GameObject.Destroy(this.highlightObj);
            this.spawnBehavior.SpawnBlock(this.currentBlockSpeed);
        }
    }

    public void SetBlockSpeedAttrs(float currentBlockSpeed, float baseBlockSpeed, float maxBlockSpeed, float blockAcceleration)
    {
        this.currentBlockSpeed = currentBlockSpeed;
        this.baseBlockSpeed = baseBlockSpeed;
        this.maxBlockSpeed = maxBlockSpeed;
        this.blockAcceleration = blockAcceleration;
    }

    public void SetSpawnBehavior(BlockSpawnBehavior spawnBehavior)
    {
        this.spawnBehavior = spawnBehavior;
    }

    public void Rotate()
    {
        this.transform.Rotate(0, 0, -90);
        this.highlightObj.transform.rotation = this.initRotation;
        if (this.highlightObj.transform.localScale.x == this.width)
        {
            this.SetHighlightWidth(this.rotatedWidth);
        }
        else
        {
            this.SetHighlightWidth(this.width);
        }

    }

    public void AccelerateDropSpeed()
    {
        this.tickTime = 0;
        this.isAccelerating = true;
    }

    public void DeccelerateDropSpeed()
    {
        this.tickTime = 0;
        this.isAccelerating = false;
    }

    private void ChangeSpeed()
    {
        if (this.isAccelerating)
        {
            if (this.currentBlockSpeed < this.maxBlockSpeed)
            {
                this.currentBlockSpeed = Mathf.Min(this.maxBlockSpeed, this.currentBlockSpeed + 0.1f);
            }
        }
        else
        {
            if (this.baseBlockSpeed < this.currentBlockSpeed)
            {
                this.currentBlockSpeed = Mathf.Max(this.baseBlockSpeed, this.currentBlockSpeed - 0.1f);
            }
        }
    }
    
    private void SetHighlightWidth(float width)
    {
        this.highlightObj.transform.localScale = (highlightObj.transform.localScale * new Vector2(0, 1)) + (Vector2.one * width);
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && this.isDropping)
        {
            this.spawnBehavior.SpawnBlock(this.currentBlockSpeed);
        }
    }
}
