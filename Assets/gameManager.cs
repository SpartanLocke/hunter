using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class gameManager : MonoBehaviour
{
    public GameObject tree;
    public GameObject exit;
    public GameObject player;
    private GameObject cabin;
    public GameObject cabinPrefab;
    private Vector3 cabinPos;
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
    public float treeProb;
    public GameObject blackScreen;
    private GameObject black;
    public GameObject deer;
    public float deerProb;
    private string enterDir;
    public List<List<int>> roomLayout;
    public Vector2 curCoords;
    public List<int> assignedNums;

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
        cabin = GameObject.Find("cabin");
        screenHeight = Camera.main.orthographicSize * 2.0f;
        screenWidth = (screenHeight * Screen.width) / Screen.height;
        oldRoomId = roomId;
        loadRoomLayout();
        startRoom(roomId);
    }

    // Update is called once per frame
    void Update()
    {
        if (oldRoomId != roomId)
        {
            if (cabin != null) {
                cabinPos = cabin.transform.position;
                Destroy(cabin);
            }
            if (roomId == 1)
            {
                cabin = Instantiate(cabinPrefab, cabinPos, Quaternion.identity) as GameObject;
            }
            GameObject[] removables = GameObject.FindGameObjectsWithTag("remove");
            for (int i = 0; i < removables.Length; i++)
            {
                Destroy(removables[i]);
            }
            
            
            if (player.transform.position.y < -screenHeight / 2 || player.transform.position.y > screenHeight / 2)
            {
                //player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(player.transform.position.x, -player.transform.position.y));
                enterDir = player.transform.position.y > screenHeight / 2 ? "up" : "down";
                curCoords += (enterDir == "up") ? Vector2.down : Vector2.up;
                player.transform.position = new Vector3(player.transform.position.x, -player.transform.position.y);
            }
            else
            {
                //player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(-player.transform.position.x, player.transform.position.y));
                enterDir = player.transform.position.x > screenWidth / 2 ? "right" : "left";
                curCoords += (enterDir == "right") ? Vector2.right : Vector2.left;
                player.transform.position = new Vector3(-player.transform.position.x, player.transform.position.y);
            }
            player.transform.position -= player.transform.position.normalized * .5f;
            black = Instantiate(blackScreen, new Vector3(0,0,-1), Quaternion.identity) as GameObject;
            player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
            oldRoomId = roomId;
            startRoom(roomId);
            StartCoroutine(blackOut());            
        }
    }

    IEnumerator blackOut()
    {
        yield return new WaitForSeconds(1);
        Destroy(black);
        player.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeRotation;
    }

    void startRoom(int num)
    {
        //TODO: add end conditoin if maxrooms is reached
        string layout = PlayerPrefs.GetString(num.ToString());
        if (layout == "")
        {
            layout = createRoom(num);
            PlayerPrefs.SetString(num.ToString(), layout);
        }
        //layout is backwards
        //Debug.Log(layout);
        Debug.Log(curCoords.x + " " + curCoords.y);
        
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
            else if (cell == "d")
            {
                pos.x += tree.GetComponent<SpriteRenderer>().bounds.size.x + xOff;
                pos.y += tree.GetComponent<SpriteRenderer>().bounds.size.y + yOff;
                GameObject obj = Instantiate(deer, pos, Quaternion.identity) as GameObject;
            }
            cellNum++;
        }

    }

    string createRoom(int num)
    {
        if (num == 1) //this only happens on a fresh install
        {
            roomLayout = new List<List<int>>();
            for (int i = 0; i < 3; i++)
            {
                roomLayout.Add(createRow(3));
            }
            roomLayout[1][1] = 1;
            curCoords = new Vector2(1,1);
        } else if (curCoords.x == 0 || curCoords.x == roomLayout[0].Count-1 || curCoords.y == 0 || curCoords.y == roomLayout.Count-1)
        {
            if (enterDir == "down")
            {
                roomLayout.Add(createRow(roomLayout[0].Count));
            }
            if (enterDir == "up")
            {
                roomLayout.Insert(0,createRow(roomLayout[0].Count));
                curCoords += Vector2.up;
            }
            else
            {
                int nextRoom = 0;
                for (int i=0; i < roomLayout.Count; i++)
                {
                    nextRoom = Random.Range(2, maxRoom);
                    while (assignedNums.Contains(nextRoom))
                    {
                        nextRoom = Random.Range(2, maxRoom);
                    }
                    if (enterDir == "right") roomLayout[i].Add(nextRoom);
                    else if (enterDir == "left")
                    {
                        roomLayout[i].Insert(0, nextRoom);
                    }
                    assignedNums.Add(nextRoom);
                }
            }
            if (enterDir == "left")
            {
                curCoords += Vector2.right;
            }
            saveRoomLayout();
        }
        string layout = "";
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                if ((i == 0 || i == height - 1 || j == 0 || j == width - 1))
                {
                    string newCell = "";
                    if (j == width / 2) {
                        if (i == 0)
                        {
                            newCell += roomLayout[(int)curCoords.y+1][(int)curCoords.x];
                        } else if (i == height - 1)
                        {
                            newCell += roomLayout[(int)curCoords.y - 1][(int)curCoords.x];
                        }
                    } else if (i == height / 2) { 
                        if (j == 0) {
                            newCell += roomLayout[(int)curCoords.y][(int)curCoords.x -1];
                        } else if (j == width - 1)
                        {
                            newCell += roomLayout[(int)curCoords.y][(int)curCoords.x + 1];
                        }
                    } else
                    {
                        newCell += "t";
                    }
                    layout += newCell+" ";
                }
                else
                {
                    int r = Random.Range(0, 100);
                    int t = Random.Range(0, 100);
                    if (r < deerProb)
                    {
                        layout += "d ";
                    } else if ( t < treeProb &&
                                !(j == width / 2 && (i == 1 || i == height - 2)) &&
                                !(i == height / 2 && (j == 1 || i == width - 2)) ){
                        layout += "t ";
                    } else
                    {
                        layout += "b ";
                    }
                }
            }
            layout += "\n";
        }
        return layout;
    }

    List<int> createRow(int length)
    {
        List<int> rooms = new List<int>();
        int nextRoom = 0;
        for (int j = 0; j < length; j++)
        {
            nextRoom = Random.Range(2, maxRoom);
            while (assignedNums.Contains(nextRoom))
            {
                nextRoom = Random.Range(2, maxRoom);
            }
            rooms.Add(nextRoom);
            assignedNums.Add(nextRoom);
        }
        return rooms;
    }

    void loadRoomLayout()
    {
        string dee = PlayerPrefs.GetString("roomLayout");
        Debug.Log(dee);
        roomLayout = new List<List<int>>();
        List<int> rooms = new List<int>();
        for (int i = 0; i < dee.Length; i++)
        {
            string cell = "";
            if (dee[i] == ' ')
            {
                continue;
            }
            else if (dee[i] == '\n')
            {
                roomLayout.Add(rooms);
                rooms = new List<int>();
            }
            else
            {
                cell = dee.Substring(i, dee.IndexOf(" ", i + 1) - i);
                i = dee.IndexOf(" ", i + 1);
                int roomNum;
                bool isInt = int.TryParse(cell, out roomNum);
                if (isInt)
                {
                    rooms.Add(roomNum);
                    if (roomNum == 1)
                    {
                        curCoords = new Vector2(rooms.Count-1, roomLayout.Count);
                    }
                }
                else
                {
                    Debug.LogError("Non integer room ID");
                }
            }
        }
    }

    void saveRoomLayout()
    {
        string dee = "";
        for (int i = 0; i < roomLayout.Count; i++)
        {
            for (int j = 0; j < roomLayout[i].Count; j++)
            {
                dee += roomLayout[i][j] + " ";
            }
            dee += "\n";
        }
        Debug.Log(dee);
        PlayerPrefs.SetString("roomLayout", dee);
    }
}
