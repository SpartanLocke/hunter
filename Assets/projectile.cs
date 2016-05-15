using UnityEngine;
using System.Collections;

public class projectile : MonoBehaviour {

    private Vector2 direction = new Vector2(0, 0);
    public float speed;
    private Rigidbody2D rb;
	// Use this for initialization
    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), gameManager.instance.player.GetComponent<Collider2D>());
    }

	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void changeDirection(Vector2 dir)
    {
        dir.Normalize();
        float ang = Mathf.Acos(Vector2.Dot(dir, Vector2.up)) * (180f/Mathf.PI);
        ang *= dir.x > 0 ? -1 : 1;
        transform.Rotate(Vector3.forward, ang);
        direction = dir;
        rb.velocity = speed * direction;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        if (coll.collider.gameObject.name == "sapling(Clone)")
        {
            coll.collider.gameObject.GetComponent<SpriteRenderer>().sprite = gameManager.instance.stumpSpr;
            coll.collider.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
            tree sap = coll.collider.gameObject.GetComponent<tree>();
            gameManager.instance.cutDown(sap.index);
        }
        Destroy(this.gameObject);
    }
    
}
