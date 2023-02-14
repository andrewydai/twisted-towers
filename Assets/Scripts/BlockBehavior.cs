using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBehavior : MonoBehaviour
{
    public GameObject highlight;
    public float width;
    public float rotatedWidth;
    private float blockSpeed;
    private bool isDropping;
    private float despawnY = -12;
    private GameObject highlightObj;
    private BlockSpawnBehavior spawnBehavior;

    // Start is called before the first frame update
    void Start()
    {
        this.highlightObj = Instantiate(this.highlight, this.transform);
        this.highlightObj.transform.localPosition = Vector3.zero;
        this.highlightObj.transform.localScale = (highlightObj.transform.localScale * new Vector2(0, 1)) + (Vector2.one * this.width);
        this.isDropping = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (this.isDropping)
        {
            float step = this.blockSpeed * Time.deltaTime;
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(transform.position.x, this.despawnY), step);
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
            this.spawnBehavior.SpawnBlock();
        }
    }

    public void setBlockSpeed(float speed)
    {
        this.blockSpeed = speed;
    }

    public void setSpawnBehavior(BlockSpawnBehavior spawnBehavior)
    {
        this.spawnBehavior = spawnBehavior;
    }

    private void OnDestroy()
    {
        if (gameObject.scene.isLoaded && this.isDropping)
        {
            this.spawnBehavior.SpawnBlock();
        }
    }
}
