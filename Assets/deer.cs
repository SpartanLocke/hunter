using UnityEngine;
using System.Collections;

public class deer : MonoBehaviour {
    public float health;
    private float maxHealth;
	// Use this for initialization
	void Start () {
        maxHealth = health;
	}
	
	// Update is called once per frame
	void Update () {
	    if (health < 1)
        {
            gameManager.instance.deerKill();
            Destroy(this.gameObject);
        }
	}

    void OnCollisionEnter2D(Collision2D coll)
    {
        //probably a janky thing to do but it works
        //should instead check if its an instance of a bullet gameobject or something like that
        if (coll.collider.name == "bullet(Clone)")
        {
            health -= 1;
            gameObject.GetComponent<Renderer>().material.color = new Color(1f, 1f, 1f, health/maxHealth);
        }
        
    }
}
