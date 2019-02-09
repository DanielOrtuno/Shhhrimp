using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class KitchenHangerScript : MonoBehaviour {

    GameObject finalBoss;
    Rigidbody rb;

	// Use this for initialization
	void Start () {
        rb = GetComponent<Rigidbody>();
    }
	
	// Update is called once per frame
	void Update () {
        if (GameObject.Find("Boss").GetComponent<FinalBossScript>().bossHealth == 0)
            rb.useGravity = true;
	}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag == "FinalBoss")
        {
            finalBoss = collision.gameObject;
            finalBoss.transform.parent.parent.gameObject.GetComponent<Rigidbody>().useGravity = true; 
        }
        
    }
}
