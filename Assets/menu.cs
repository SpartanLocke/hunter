using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class menu : MonoBehaviour {

    public Text text;
    private bool startOk = false;
	// Use this for initialization
	void Start () {
        SceneManager.UnloadScene("main");
        StartCoroutine(startIt());
        text.text += " "+(PlayerPrefs.GetInt("dayNumber")+1); 
	}
	
	// Update is called once per frame
	void Update () {
	    if (Input.anyKeyDown && startOk)
        {
            SceneManager.LoadScene("main");
        }
	}

    IEnumerator startIt()
    {
        yield return new WaitForSeconds(2f);
        startOk = true;
    }
}
