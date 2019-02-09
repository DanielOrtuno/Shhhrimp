using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadRestLevel : MonoBehaviour {

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            SceneManager.LoadScene("Alley_Level");
    }
}
