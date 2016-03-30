using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour {

    public float walkSpeed = 1f;
    public Rigidbody2D rb;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate () {
        rb.velocity = Vector2.zero;
        if (Input.GetKey("w"))
        {
            rb.velocity += walkSpeed * Vector2.up;
        }
        if (Input.GetKey("s"))
        {
            rb.velocity += walkSpeed * Vector2.down;
        }
        if (Input.GetKey("a"))
        {
            rb.velocity += walkSpeed * Vector2.left;
        }
        if (Input.GetKey("d"))
        {
            rb.velocity += walkSpeed * Vector2.right;
        }
        if (Input.GetKeyDown("0"))
        {
            PlayerPrefs.DeleteAll();
        }

    }
}
