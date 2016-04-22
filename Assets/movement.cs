using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour {

    public float walkSpeed = 1f;
    public Rigidbody2D rb;
    public GameObject projectile;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.identity) as GameObject;
            projectile proj = obj.GetComponent<projectile>();
            proj.changeDirection(Vector2.up);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.AngleAxis(180, Vector3.forward)) as GameObject;
            projectile proj = obj.GetComponent<projectile>();
            proj.changeDirection(Vector2.down);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.AngleAxis(270, Vector3.forward)) as GameObject;
            projectile proj = obj.GetComponent<projectile>();
            proj.changeDirection(Vector2.right);
        }
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            GameObject obj = Instantiate(projectile, transform.position, Quaternion.AngleAxis(90, Vector3.forward)) as GameObject;
            projectile proj = obj.GetComponent<projectile>();
            proj.changeDirection(Vector2.left);
        }
        if (Input.GetKeyDown("0"))
        {
            PlayerPrefs.DeleteAll();
        }
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
    }
}
