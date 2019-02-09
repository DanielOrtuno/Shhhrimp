using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossVisionScript : MonoBehaviour {

    static public bool isSpotted;
    public float speed;
    public float weaponSpeed;

    GameObject bossWeapon;
    GameObject bossVision;

    [SerializeField]
    GameObject pointA = null;
    [SerializeField]
    GameObject pointB = null;

    static public Vector3 playerXPosition;

    // Use this for initialization
    void Start () {
        isSpotted = false;
        bossWeapon = transform.parent.Find("BossWeapon").gameObject;
        bossVision = transform.Find("BossVision").gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isSpotted = true;
            if (isSpotted)
            {
                // Getting the player's X position
                playerXPosition = new Vector3(other.transform.position.x, bossWeapon.transform.position.y, bossWeapon.transform.position.z);
            }
            // Disabling the boss's vision object
            bossVision.gameObject.SetActive(false);
            gameObject.GetComponent<CapsuleCollider>().enabled = false;
        }
        
    }

    // Update is called once per frame
    void Update () {
        if (gameObject.transform.parent.GetComponent<FinalBossScript>().bossHealth != 0)
        {
            if (isSpotted != true)
            {
                // Boss is looking for the player; vision object is moving back and forth between points.
                transform.position = Vector3.Lerp(pointA.transform.position, pointB.transform.position, Mathf.PingPong(Time.time * speed, 1.0f));
            }
        }
    }

    
}
