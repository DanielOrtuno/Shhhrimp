﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitGame : MonoBehaviour {
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.anyKey || Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("Main Menu");
        }
    }
}
