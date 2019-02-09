using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletScript : MonoBehaviour {

    public short damage;
    public bool canShoot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerController>().DamagePlayerHP(damage);
            gameObject.SetActive(false);
        }

    }

    private void OnCollisionEnter(Collision collision)
    {
        gameObject.SetActive(false);
    }
}
