using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameManager : MonoBehaviour
{
    public GameObject tree;
    public GameObject exit;
    public GameObject player;
    public float xOff;
    public float yOff;
    public float exitOffset;
    public int maxRoom;
    public int roomId;
    public int oldRoomId;
    private float screenWidth;
    private float screenHeight;
    public int width;
    public int height;
    public float density;

    public static gameManager instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    //Awake is always called before any Start functions
    void Awake()
    {
        //Check if instance already exists
        if (instance == null)

            //if not, set instance to this
            instance = this;

        //If instance already exists and it's not this:
        else if (instance != this)

            //Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
            Destroy(gameObject);

        //Sets this to not be destroyed when reloading scene
        DontDestroyOnLoad(gameObject);


    }

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("player");
        screenHeight = Camera.main.orthographicSize * 2.0f;
        screenWidth = (screenHeight * Screen.width) / Screen.height;
        startRoom(1);
    }

    // Update is called once per frame
    void Update()
    {
        if (oldRoomId != roomId)
        {
            GameObject cabin = GameObject.Find("cabin");
            if (cabin != null)
            {
                Destroy(cabin);
            }

            GameObject[] trees = GameObject.FindGameObjectsWithTag("tree");
            for (int i = 0; i < trees.Length; i++)
            {
                Destroy(trees[i]);
            }
            GameObject[] exits = GameObject.FindGameObjectsWithTag("exit");
            for (int i = 0; i < exits.Length; i++)
            {
                Destroy(exits[i]);
            }

            //make this teleport work
            
            if (player.transform.position.y < -screenHeight/2 || player.transform.position.y > screenHeight/2)
            {
                //why i cant just reference the player variable and instead have to find the player i dont know
                player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(player.transform.position.x, -player.transform.position.y));
                
            } else
            {
                player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(-player.transform.position.x, player.transform.position.y));
            }
            //player.transform.position += Vector3.MoveTowards(player.transform.position, new Vector3(0, 0, 0), 1);
            oldRoomId = roomId;
            //new WaitForSeconds(1f);
            startRoom(roomId);
            
        }
    }

    void startRoom(int num)
    {
        //make it where the room you came from gets saved as new room
        string layout = PlayerPrefs.GetString(num.ToString());
        if (layout == "")
        {
            layout = createRoom(num);
            PlayerPrefs.SetString(num.ToString(), layout);
        }
        //layout is backwards
        Debug.Log(layout);
        int row = -1;
        int cellNum = 0;
        for (int i = 0; i < layout.Length; i++)
        {
            string cell = "";
            if (layout[i] == ' ' || layout[i] == '\n')
            {
                continue;
            }
            else
            {                
                cell = layout.Substring(i, layout.IndexOf(" ", i + 1) - i);
                i = layout.IndexOf(" ", i + 1);
            }
            
            float column = cellNum % (width);
            if (column == 0) row++;
            Vector3 offset = Camera.main.ViewportToWorldPoint(new Vector3(0.0f, 0.0f));
            offset.z = 0;
            Vector3 pos = new Vector3( (column / width) * screenWidth, ((float) row / height) * screenHeight);
            pos += offset; //bottom left corner is approx (-7, -5)

            int levelNum;
            bool isInt = int.TryParse(cell, out levelNum);
            if (isInt)
            {
                float yedge = (screenHeight / height);
                if (row == 0)
                {
                    yedge *= -1;
                }
                else if (row == height - 1)
                {
                    yedge *= 1;
                }
                else
                {
                    yedge *= 0;
                }
                float xedge = (screenWidth / width);
                if (column == 0)
                {
                    //Line = pos.y;
                    xedge *= -1;
                } else if (column == width - 1)
                {
                    xedge *= 1;
                } else
                {
                    xedge *= 0;
                }
                
                //move them to middle of the cell, then apply the edge offset +/-/0
                pos.x += .5f * (screenWidth/width) + xedge;
                pos.y += .5f * (screenHeight / height) + yedge; 
                GameObject obj3 = Instantiate(exit, pos, Quaternion.identity) as GameObject;
                createNewRoom newRoom = obj3.GetComponent<createNewRoom>();
                newRoom.idNum = levelNum;
            }
            else if (cell == "t")
            {
                pos.x += tree.GetComponent<SpriteRenderer>().bounds.size.x + xOff;
                pos.y += tree.GetComponent<SpriteRenderer>().bounds.size.y + yOff;
                GameObject obj = Instantiate(tree, pos, Quaternion.identity) as GameObject;
            }
            cellNum++;
        }

    }

    string createRoom(int num)
    {
        string layout = "";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if (i == 0 || i == height - 1 || j == 0 || j == width - 1 || Random.Range(0, 100) < density)
                {
                    if (j == width / 2 && (i == 0 || i == height - 1))
                    {
                        bool isNew = false;
                        int newNum = 0;
                        while (!isNew)
                        {
                            newNum = Random.Range(2, maxRoom);
                            if (PlayerPrefs.GetString(newNum.ToString()) == "")
                            {
                                isNew = true;
                            }
                        }
                        layout += newNum.ToString() +" ";
                    }
                    else
                    {
                        layout += "t ";
                    }

                }
                else
                {
                    layout += "b ";
                }
            }
            layout += "\n";
        }
        return layout;
    }

}
