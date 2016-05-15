using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public int treeProb;
    public GameObject blackScreen;
    private GameObject black;
    private GameObject endBlack;
    public GameObject deer;
    private string enterDir;
    public List<List<int>> roomLayout;
    public Vector2 curCoords;
    public List<int> assignedNums;
    public GameObject corn;
    public GameObject salt;
    public int cornCount, saltCount;
    public Text cornText;
    public Text saltText;
    public bool droppedItem = false;
    public float trackProb;
    public float Exp;
    public bool oldTracks;
    public bool deerSpawn;
    public bool trackSpawn;
    public GameObject tracks;
    public List<List<int>> seenTracks = new List<List<int>>();
    public List<GameObject> tentList = new List<GameObject>();
    public List<Vector3> openings;
    public int maxTreeProb;
    public Sprite stumpSpr;
    public Sprite albino;
    public GameObject sapling;
    private string layout;
    public int dayNumber;
    public int treeGrowthRate;
    public List<string> layoutList;
    public int cornProb;
    public float saltDecayRate;
    public int saltProb;
    public float playTime;
    public bool endStart = false;
    private bool one = false;
    private bool two = false;
    public float countDownTime;
    public int moneyCount;
    public int cornCost;
    public int saltCost;
    public Text moneyText;
    public int deerMoney;
    public int treeMoney;
    private bool deerKilled = false;

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
        //DontDestroyOnLoad(gameObject);


    }

    // Use this for initialization
    void Start()
    {
        SceneManager.UnloadScene("start");
        player = GameObject.Find("player");
        tentList.Add(GameObject.Find("tent"));
        screenHeight = Camera.main.orthographicSize * 2.0f;
        screenWidth = (screenHeight * Screen.width) / Screen.height;
        saltCount = PlayerPrefs.GetInt("saltCount", 0);
        cornCount = PlayerPrefs.GetInt("cornCount", 0);
        moneyCount = PlayerPrefs.GetInt("moneyCount", 0);
        playTime += moneyCount;
        cornText.text = cornCount.ToString();
        saltText.text = saltCount.ToString();
        moneyText.text = moneyCount.ToString();
        dayNumber = PlayerPrefs.GetInt("dayNumber") +1;
        PlayerPrefs.SetInt("dayNumber", dayNumber);
        oldRoomId = roomId;
        loadRoomLayout();
        startRoom(roomId);
    }

    // Update is called once per frame
    void Update()
    {
        playTime -= Time.deltaTime;
        if (playTime < 0 && !endStart)
        {
            endStart = true;
            endBlack = Instantiate(blackScreen, new Vector3(0, 0, -2), Quaternion.identity) as GameObject;
            Color color = endBlack.GetComponent<Renderer>().material.color;
            color.a = .05f;
            endBlack.GetComponent<Renderer>().material.color = color;
            StartCoroutine(dayEnd());
        }
        if (playTime < 0)
        {
            Color color = endBlack.GetComponent<Renderer>().material.color;
            color.a = Mathf.Lerp(.05f, .9f, -(playTime)/countDownTime);
            endBlack.GetComponent<Renderer>().material.color = color;
            
        }
        if (oldRoomId != roomId)
        {
            if (tentList.Count != 0) {
                foreach (GameObject gm in tentList)
                {
                    cabinPos = gm.transform.position;
                    tentList.Remove(gm);
                    Destroy(gm);
                }
            }
            if (roomId == 1)
            {
                tentList.Add(Instantiate(cabinPrefab, cabinPos, Quaternion.identity) as GameObject);
            }
            GameObject[] removables = GameObject.FindGameObjectsWithTag("remove");
            for (int i = 0; i < removables.Length; i++)
            {
                Destroy(removables[i]);
            }
            droppedItem = false;
            //Debug.Log(player.transform.position);
            if (player.transform.position.y < (-screenHeight / 2) +1 || player.transform.position.y > (screenHeight / 2) -1)
            {
                //player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(player.transform.position.x, -player.transform.position.y));
                enterDir = player.transform.position.y > (screenHeight / 2) - 1 ? "up" : "down";
                curCoords += (enterDir == "up") ? Vector2.down : Vector2.up;
                player.transform.position = (enterDir == "up") ? openings[2] : openings[0];
                player.transform.rotation = (enterDir == "up") ? Quaternion.AngleAxis(0,Vector3.forward) : Quaternion.AngleAxis(180, Vector3.forward);
            }
            else
            {
                //player.GetComponent<Rigidbody2D>().MovePosition(new Vector2(-player.transform.position.x, player.transform.position.y));
                enterDir = player.transform.position.x > 0 ? "right" : "left";
                curCoords += (enterDir == "right") ? Vector2.right : Vector2.left;
                player.transform.position = (enterDir == "right") ? openings[3] : openings[1];
                player.transform.rotation = (enterDir == "right") ? Quaternion.AngleAxis(270, Vector3.forward) : Quaternion.AngleAxis(90, Vector3.forward);
            }
            
            //player.transform.position -= player.transform.position.normalized * .5f;
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
        player.transform.position += Vector3.back;
    }

    IEnumerator dayEnd()
    {
        yield return new WaitForSeconds(countDownTime);
        SceneManager.LoadScene("start");
    }

    void startRoom(int num)
    {
        //TODO: add end condition if maxrooms is reached
        playTime -= 3;
        layout = PlayerPrefs.GetString(num.ToString());
        if (layout == "")
        {
            layout = createRoom(num);
            PlayerPrefs.SetString(num.ToString(), layout);
        }
        //layout is backwards
        //Debug.Log(layout);
        int cornDay = PlayerPrefs.GetInt(num+"corn");
        int saltDay = PlayerPrefs.GetInt(num + "salt");
        bool roomSalt = (dayNumber - saltDay) < saltDecayRate && saltDay != 0;
        bool roomCorn = dayNumber - cornDay == 0;
        if (!roomSalt && !roomCorn)
        {
            Debug.Log(roomSalt +" "+roomCorn);
            PlayerPrefs.DeleteKey(num.ToString() + "items");
            if (dayNumber - cornDay > 1) PlayerPrefs.DeleteKey(num.ToString() + "corn");
            PlayerPrefs.DeleteKey(num.ToString() + "salt");
        }

        

        int trackIndex = 0;
        for (int k = 0;k < seenTracks.Count; k++)
        {
            int roomNumber = seenTracks[k][0];
            if (roomNumber == num)
            {
                trackIndex = seenTracks[k][1];
            }
        }
        deerSpawn = false;
        float deerProb = oldTracks ? trackProb : 0;
        int r = Random.Range(0, 100);
        if (r < deerProb && num != 1) deerSpawn = true;
        deerSpawn &= !deerKilled;
        trackSpawn = trackIndex != 0;
        if (!trackSpawn && !deerSpawn) // tracks aren't already present in this room and there isn't a deer about to spawn
        {
            int curTreeProb = PlayerPrefs.GetInt("p"+num.ToString());
            curTreeProb -= PlayerPrefs.GetInt("s" + roomId);
            curTreeProb += dayNumber - cornDay == 1 ? cornProb : 0;
            curTreeProb += roomSalt ? (dayNumber - saltDay) * saltProb : 0;
            curTreeProb = Mathf.Max(curTreeProb, 0);
            trackProb = oldTracks ? Exp * trackProb : curTreeProb;
            r = Random.Range(0, 100);
            if (r < trackProb && num != 1)
            {
                trackSpawn = true;
                trackSpawn &= !deerKilled;
                trackIndex = Random.Range((width * height) / 4, (3 * width * height) / 4);
            }
            
        }
        oldTracks = trackSpawn;

        if (deerSpawn) trackIndex = Random.Range((width * height) / 4, (3 * width * height) / 4);

        int row = -1;
        int cellNum = 0;
        List<int> growBackNums = new List<int>();
        layoutList = new List<string>();
        for (int i = 0; i < layout.Length; i++)
        {
            string cell = "";
            if (layout[i] == ' ' || layout[i] == '\n')
            {
                layoutList.Add(layout[i].ToString());
                continue;
            }
            else
            {                
                cell = layout.Substring(i, layout.IndexOf(" ", i + 1) - i);
                i = layout.IndexOf(" ", i + 1);
                layoutList.Add(cell);
                layoutList.Add(" ");
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
                Vector2 scaleVector = new Vector2(1, 1);
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
                    scaleVector = new Vector2(.2f, 2);
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
                    scaleVector = new Vector2(2, .2f);
                }
                
                //move them to middle of the cell, then apply the edge offset +/-/0
                pos.x += .5f * (screenWidth/width) + xedge;
                pos.y += .5f * (screenHeight / height) + yedge; 
                GameObject obj3 = Instantiate(exit, pos, Quaternion.identity) as GameObject;
                obj3.transform.localScale = scaleVector;
                createNewRoom newRoom = obj3.GetComponent<createNewRoom>();
                newRoom.idNum = levelNum;
            }
            else if (cell == "t")
            {
                pos.x += tree.GetComponent<SpriteRenderer>().bounds.size.x + xOff;
                pos.y += tree.GetComponent<SpriteRenderer>().bounds.size.y + yOff;
                GameObject obj = Instantiate(tree, pos, Quaternion.identity) as GameObject;
            }
            else if (cell == "s")
            {
                pos.x += tree.GetComponent<SpriteRenderer>().bounds.size.x + xOff;
                pos.y += tree.GetComponent<SpriteRenderer>().bounds.size.y + yOff;
                GameObject obj = Instantiate(sapling, pos, Quaternion.identity) as GameObject;
                tree sap = obj.GetComponent<tree>();
                sap.index = layoutList.Count;
            }
            else if (cell[0] == 'u')
            {
                pos.x += tree.GetComponent<SpriteRenderer>().bounds.size.x + xOff;
                pos.y += tree.GetComponent<SpriteRenderer>().bounds.size.y + yOff;
                GameObject obj = Instantiate(sapling, pos, Quaternion.identity) as GameObject;
                tree sap = obj.GetComponent<tree>();
                sap.index = layoutList.Count;
                int oldDay;
                bool isIntT = int.TryParse(cell.Remove(0,1), out oldDay);
                if (isIntT)
                {
                    if (dayNumber - oldDay > treeGrowthRate)
                    {
                        growBackNums.Add(layoutList.Count);
                        cellNum++;
                        continue;
                    }
                }
                obj.GetComponent<PolygonCollider2D>().enabled = false;
                obj.GetComponent<SpriteRenderer>().sprite = stumpSpr;
            }
            else if (cell == "b" && trackIndex >= i)
            {
                trackIndex = 0;
                pos.x += tree.GetComponent<SpriteRenderer>().bounds.size.x + xOff;
                pos.y += tree.GetComponent<SpriteRenderer>().bounds.size.y + yOff;
                if (trackSpawn)
                {
                    GameObject obj = Instantiate(tracks, pos, Quaternion.AngleAxis((num%4) * 90, Vector3.forward)) as GameObject;
                    seenTracks.Add(new List<int>{ num, i });
                } else if (deerSpawn){
                    GameObject obj = Instantiate(deer, pos, Quaternion.identity) as GameObject;
                    if ((num%300) == 2) obj.GetComponent<SpriteRenderer>().sprite = albino;
                }
            }
            cellNum++;
        }
        createItems(num);
        foreach (int k in growBackNums)
        {
            growBack(k);
        }
    }

    void createItems(int num)
    {
        string items = PlayerPrefs.GetString(num.ToString() + "items");
        int count = 0;
        string type = "";
        float x =-10;
        float y =-10;
        bool isX = false;
        bool isY = false;
        if (items != "")
        {
            droppedItem = true;
            for (int i = 0; i < items.Length; i++)
            {
                string cell = "";
                if (items[i] == ' ')
                {
                    continue;
                }
                else
                {
                    cell = items.Substring(i, items.IndexOf(" ", i + 1) - i);
                    i = items.IndexOf(" ", i + 1);
                }
                count++;
                if (count == 1) type = cell;
                else if (count == 2)
                {
                    isX = float.TryParse(cell, out x);
                }
                else if (count == 3)
                {
                    isY = float.TryParse(cell, out y);
                }
                if (isX && isY)
                {
                    if (type == "corn")
                    {
                        GameObject obj = Instantiate(corn, new Vector3(x, y), Quaternion.identity) as GameObject;
                    }
                    else if (type == "salt")
                    {
                        GameObject obj = Instantiate(salt, new Vector3(x, y), Quaternion.identity) as GameObject;
                    }
                }
            }
        }
    }

    public void buy(bool salt)
    {
        int oldMoney = moneyCount;
        moneyCount -= salt ? saltCost : cornCost;
        if (moneyCount < 0)
        {
            moneyCount = oldMoney;
            return;
        }
        moneyText.text = moneyCount.ToString();
        PlayerPrefs.SetInt("moneyCount", moneyCount);
        if (salt)
        {
            saltCount++;
            PlayerPrefs.SetInt("saltCount", saltCount);
            saltText.text = saltCount.ToString();
        }
        else
        {
            cornCount++;
            PlayerPrefs.SetInt("cornCount", cornCount);
            cornText.text = cornCount.ToString();
        }
        
    }

    public void deerKill()
    {
        moneyCount += deerMoney;
        moneyText.text = moneyCount.ToString();
        PlayerPrefs.SetInt("moneyCount", moneyCount);
        deerKilled = true;
    }

    public void cutDown(int i)
    {
        PlayerPrefs.SetInt("s"+roomId, PlayerPrefs.GetInt("s" + roomId) + 1);
        layoutList[i - 2] = "u" + dayNumber.ToString();
        //Debug.Log(layout);
        layout = "";
        foreach (string c in layoutList)
        {
            layout += c;
        }
        //Debug.Log(layout);
        moneyCount+= treeMoney;
        moneyText.text = moneyCount.ToString();
        PlayerPrefs.SetInt("moneyCount", moneyCount);
        PlayerPrefs.SetString(roomId.ToString(), layout);
    }

    public void growBack(int i)
    {
        PlayerPrefs.SetInt("s" + roomId, PlayerPrefs.GetInt("s" + roomId) - 1);
        //Debug.Log(layout);
        layoutList[i - 2] = "s";
        layout = "";
        foreach (string c in layoutList)
        {
            layout += c;
        }
        //Debug.Log(layout);
        PlayerPrefs.SetString(roomId.ToString(), layout);
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
            saveRoomLayout();
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
        maxTreeProb = Mathf.Min(maxTreeProb, 100);
        treeProb = num % maxTreeProb;
        PlayerPrefs.SetInt("p" + num.ToString(), treeProb);
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
                            newCell += roomLayout[(int)curCoords.y + 1][(int)curCoords.x];
                        } else if (i == height - 1)
                        {
                            newCell += roomLayout[(int)curCoords.y - 1][(int)curCoords.x];
                        }
                    } else if (i == height / 2) {
                        if (j == 0) {
                            newCell += roomLayout[(int)curCoords.y][(int)curCoords.x - 1];
                        } else if (j == width - 1)
                        {
                            newCell += roomLayout[(int)curCoords.y][(int)curCoords.x + 1];
                        }
                    } else
                    {
                        newCell += "t";
                    }
                    layout += newCell + " ";
                }
                else
                {
                    int t = Random.Range(0, 100);
                    if ( t < treeProb &&
                                !(j == width / 2 && (i == 1 || i == height - 2)) &&
                                !(i == height / 2 && (j == 1 || i == width - 2)) &&
                                num !=1){
                        layout += "s ";
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

    public void createSalt(Vector3 pos)
    {
        if (saltCount > 0 && !droppedItem)
        {
            droppedItem = true;
            GameObject obj = Instantiate(salt, pos, Quaternion.identity) as GameObject;
            PlayerPrefs.SetString(roomId.ToString() + "items", "salt " + pos.x + " " + pos.y +" ");
            saltCount--;
            PlayerPrefs.SetInt("saltCount", saltCount);
            PlayerPrefs.SetInt(roomId.ToString() + "salt", dayNumber);
            saltText.text = saltCount.ToString();
        }
    }

    public void createCorn(Vector3 pos)
    {
        if (cornCount > 0 && !droppedItem)
        {
            droppedItem = true;
            GameObject obj = Instantiate(corn, pos, Quaternion.identity) as GameObject;
            PlayerPrefs.SetString(roomId.ToString() + "items", "corn " + pos.x + " " + pos.y+ " ");
            cornCount--;
            PlayerPrefs.SetInt("cornCount",cornCount);
            PlayerPrefs.SetInt(roomId.ToString() + "corn", dayNumber);
            cornText.text = cornCount.ToString();
        }
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
        //Debug.Log(dee);
        PlayerPrefs.SetString("roomLayout", dee);
    }
}
