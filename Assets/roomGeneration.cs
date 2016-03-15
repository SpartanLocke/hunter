using UnityEngine;
using System.Collections;

public class roomGeneration : MonoBehaviour {

    public GameObject tree;
    public GameObject newRoom;
    public bool start;
    public int numTrees;
	// Use this for initialization
	void Start () {
        startRoom();
    }

    // Update is called once per frame
    void Update () {
	
	}

    void startRoom()
    {
        float height = Camera.main.orthographicSize * 2.0f;
        float width = height * Screen.width / Screen.height;
        for (int i = 0; i < width / tree.GetComponent<SpriteRenderer>().bounds.size.x; i++)
        {
            Vector3 offset = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f));
            offset.z = 0;
            Vector3 pos = new Vector3(i * tree.GetComponent<SpriteRenderer>().bounds.size.x + .5f, .5f);

            if (i > width / 2 && i < width / 2 + tree.GetComponent<SpriteRenderer>().bounds.size.x)
            {
                GameObject obj3 = Instantiate(newRoom, pos + offset + new Vector3(0, -1.0f), Quaternion.identity) as GameObject;
                //GameObject obj4 = Instantiate(newRoom, pos + offset + new Vector3(0, height+1.0f), Quaternion.identity) as GameObject;
                continue;
            }
            GameObject obj = Instantiate(tree, pos + offset, Quaternion.identity) as GameObject;
            GameObject obj2 = Instantiate(tree, pos + offset + new Vector3(0, height - .5f), Quaternion.identity) as GameObject;
        }
        for (int i = 0; i < height / tree.GetComponent<SpriteRenderer>().bounds.size.y; i++)
        {
            Vector3 offset = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f));
            offset.z = 0;
            Vector3 pos = new Vector3(.25f, i * tree.GetComponent<SpriteRenderer>().bounds.size.y + .5f);

            GameObject obj = Instantiate(tree, pos + offset, Quaternion.identity) as GameObject;
            GameObject obj2 = Instantiate(tree, pos + offset + new Vector3(width - .5f, 0), Quaternion.identity) as GameObject;
        }
    }

    
}
