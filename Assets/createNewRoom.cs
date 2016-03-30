using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class createNewRoom : MonoBehaviour {

    public int idNum;

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnCollisionEnter2D()
    {
        gameManager.instance.roomId = idNum;
    }
             

    }
