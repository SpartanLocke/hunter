using UnityEngine;
using System.Collections;

public class movement : MonoBehaviour {

    public float walkSpeed = 1f;
    public Rigidbody2D rb;
    public GameObject projectile;
    public GameObject muzzle;
    public GameObject muzzleStart;
    public float rotationScale;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody2D>();
	}

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("z"))
        {
            GameObject obj = Instantiate(projectile, muzzle.transform.position, Quaternion.identity) as GameObject;
            projectile proj = obj.GetComponent<projectile>();
            proj.changeDirection(muzzle.transform.position - muzzleStart.transform.position);
        }
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            //GameObject obj = Instantiate(projectile, transform.position + bulletOffset, Quaternion.AngleAxis(180, Vector3.forward)) as GameObject;
            //projectile proj = obj.GetComponent<projectile>();
            //proj.changeDirection(Vector2.down);
        }
        if (Input.GetKey("x"))
        {
            //GameObject obj = Instantiate(projectile, transform.position + bulletOffset, Quaternion.AngleAxis(270, Vector3.forward)) as GameObject;
            //projectile proj = obj.GetComponent<projectile>();
            //proj.changeDirection(Vector2.right);
            transform.Rotate(Vector3.back * rotationScale);
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            //GameObject obj = Instantiate(projectile, transform.position + bulletOffset, Quaternion.AngleAxis(90, Vector3.forward)) as GameObject;
            //projectile proj = obj.GetComponent<projectile>();
            //proj.changeDirection(Vector2.left);
            transform.Rotate(Vector3.forward * rotationScale);
        }
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            gameManager.instance.createCorn(transform.position + Vector3.forward);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            gameManager.instance.createSalt(transform.position + Vector3.forward);
        }
        if (Input.GetKeyDown("c"))
        {
            gameManager.instance.buy(false);
        }
        if (Input.GetKeyDown("5"))
        {
            gameManager.instance.buy(true);
        }
        if (Input.GetKeyDown("0"))
        {
            PlayerPrefs.DeleteAll();
        }
    }

    void FixedUpdate () {
        rb.velocity = Vector2.zero;
        if (Input.GetKey(KeyCode.UpArrow))
        {
            rb.velocity += walkSpeed * Vector2.up;
        }
        if (Input.GetKey(KeyCode.DownArrow))
        {
            rb.velocity += walkSpeed * Vector2.down;
        }
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            rb.velocity += walkSpeed * Vector2.left;
        }
        if (Input.GetKey(KeyCode.RightArrow))
        {
            rb.velocity += walkSpeed * Vector2.right;
        }
        
    }
}
