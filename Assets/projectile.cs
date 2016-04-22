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
        direction = dir;
        rb.velocity = speed * direction;
    }

    void OnCollisionEnter2D(Collision2D coll)
    {
        Destroy(this.gameObject);
    }
    
}
