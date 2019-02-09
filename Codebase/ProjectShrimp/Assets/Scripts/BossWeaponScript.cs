using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossWeaponScript : MonoBehaviour {

    
	// Use this for initialization
	void Start () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().DamagePlayerHP(2);
        }
    }

}
