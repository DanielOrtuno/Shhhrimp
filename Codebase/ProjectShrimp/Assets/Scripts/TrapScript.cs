using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapScript : MonoBehaviour {


    bool isArmed;
	// Use this for initialization
	void Start ()
    {
        isArmed = false;
        GetComponent<Collider>().isTrigger = isArmed;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Ground")
        {
            GetComponent<Collider>().isTrigger = isArmed = true;
            GetComponent<Rigidbody>().useGravity = false;
            GetComponent<BoxCollider>().size = new Vector3(.1f, .5f, 1);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Enemy" && isArmed)
        {
            if(other.GetComponent<GuardScript>())
            {
                other.GetComponent<GuardScript>().KillGuard();
                GetComponent<AudioSource>().Play();
                StartCoroutine(DestroyTrap());
            }
            else
            {
                other.GetComponent<HeavyGuardScript>().KillGuard();
                GetComponent<AudioSource>().Play();
                StartCoroutine(DestroyTrap());
            }
        }
    }

    IEnumerator DestroyTrap()
    {
        GetComponent<SpriteRenderer>().enabled = false;
        yield return new WaitForSeconds(1);
        Destroy(this.gameObject);
    }
}
