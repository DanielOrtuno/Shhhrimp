using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneManagerScript : MonoBehaviour {

    GameObject smokeScreen;
    public string sceneName;
	// Use this for initialization
	void Start ()
    {
        smokeScreen = GameObject.Find("Canvas").transform.Find("SmokeScreen").gameObject;

        Time.timeScale = 1f;
        StartCoroutine(TurnOffSmokeScreen());
       // animator.SetBool("State", true);
	}
	
    IEnumerator TurnOffSmokeScreen()
    {
        smokeScreen.GetComponent<Animator>().Play("SmokeScreenOff");
        yield return new WaitForSeconds(1);
        smokeScreen.SetActive(false);

    }

    IEnumerator LoadSceneEnum(string sceneName)
    {
        smokeScreen.SetActive(true);

        smokeScreen.GetComponent<Animator>().Play("SmokeScreenOn");

        yield return new WaitForSeconds(.6f); 
        Time.timeScale = 0f;
        GameObject.Find("Canvas").transform.Find("WinScreen").gameObject.SetActive(true);

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.transform.tag == "Player")
        {

            StartCoroutine(LoadSceneEnum(sceneName));
        }
    }
}
