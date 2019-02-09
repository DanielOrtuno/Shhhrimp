using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarketLevelKeys : MonoBehaviour {

    //public GameObject door;

    private void Start()
    {
        //doorScript = door.GetComponent<Door>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            Door.collectedKeys++;
            Destroy(gameObject);
        }
       
    }
}
