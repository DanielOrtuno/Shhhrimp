using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelScript : MonoBehaviour
{

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Keypad0))
            SceneManager.LoadScene("Tyler_Sandbox");

        if (Input.GetKeyDown(KeyCode.Keypad1))
            SceneManager.LoadScene("Daniel_Sandbox");

        if (Input.GetKeyDown(KeyCode.Keypad2))
            SceneManager.LoadScene("Drew_Sandbox");

        if (Input.GetKeyDown(KeyCode.Keypad3))
            SceneManager.LoadScene("Joe_Sandbox");

        if (Input.GetKeyDown(KeyCode.Keypad4))
            SceneManager.LoadScene("Cameron_Sandbox");

        if (Input.GetKeyDown(KeyCode.Keypad5))
            SceneManager.LoadScene("Kitchen_Level");

        if (Input.GetKeyDown(KeyCode.Keypad6))
            SceneManager.LoadScene("Market_Level");

        if (Input.GetKeyDown(KeyCode.Keypad7))
            SceneManager.LoadScene("Resturant_Level");

    }
}