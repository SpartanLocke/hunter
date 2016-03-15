using UnityEngine;
using System.Collections;

public class createNewRoom : MonoBehaviour {

    public GameObject tree;
    public int numTrees;
    public GameObject player;
	// Use this for initialization
	void Start () {
	    
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnCollisionEnter2D()
    {
        GameObject cabin = GameObject.Find("cabin");
        if (cabin != null)
        {
            Destroy(cabin);
        }
        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        GameObject[] trees = GameObject.FindGameObjectsWithTag("random");
        for (int i =0; i<trees.Length; i++)
        {
            Destroy(trees[i]);
        }
        for (int i = 0; i < numTrees; i++)
        {
            Vector3 pos = new Vector3(Random.Range(-width/2 + 2.0f, width/2 - 2.0f), Random.Range(-height/2 + 2.0f, height/2 - 2.0f));
            GameObject obj = Instantiate(tree, pos, Quaternion.identity) as GameObject;
            obj.tag = "random";
        }
        
        Debug.Log(transform.position.y);
        //player.GetComponent<Rigidbody2D>().transform.position = new Vector2(0, 0);
        Vector2 ve = new Vector2(0, 0);
        player.transform.position = ve;
        

    }
}
