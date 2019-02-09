using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Smoke : MonoBehaviour
{


    PlayerController player;

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Player")
        {
            

            transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
            FindObjectOfType<GameManagerController>().SmokeBombEffect.transform.position = transform.position;
            FindObjectOfType<GameManagerController>().SmokeBombEffect.Play(true);
            
            StartCoroutine(Smoking());
        }
    }

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.tag == "Enemy")
    //    {
    //        if (other.GetComponent<GuardScript>())
    //        {
    //            StopCoroutine(other.GetComponent<GuardScript>().StunnedEnum());
    //            StartCoroutine(other.GetComponent<GuardScript>().StunnedEnum());
    //        }
    //        else if(other.GetComponent<HeavyGuardScript>())
    //        {
    //            StopCoroutine(other.GetComponent<HeavyGuardScript>().StunnedEnum());
    //            StartCoroutine(other.GetComponent<HeavyGuardScript>().StunnedEnum());
    //        }
    //    }
    //    if (other.tag == "SpotterGuard")
    //    {
    //        StopCoroutine(other.transform.Find("Detector").GetComponent<SpotterGuardScript>().StunnedEnum());
    //        StartCoroutine(other.transform.Find("Detector").GetComponent<SpotterGuardScript>().StunnedEnum());
    //    }
    //}

    IEnumerator Smoking()
    {
        transform.Find("GameObject").gameObject.SetActive(true);
        yield return new WaitForSeconds(3);
        player.smokeBombCount++;
        Destroy(gameObject);        
    }
}
