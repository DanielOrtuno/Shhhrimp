using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingObjectScript : MonoBehaviour {

    Rigidbody rb;
    GameObject rope;
    CameraLogic cameraLogic;
    public bool isFalling;
    GameObject ring = null;
    GameObject sphere = null;

    // Use this for initialization
    void Start ()
    {
        rope = transform.parent.gameObject;
        rb = GetComponent<Rigidbody>();
        cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (isFalling)
        {
            GameObject objectCol = other.gameObject;

            if (objectCol.tag == "Ground")
                isFalling = false;

            if (other.gameObject.tag == "Enemy")
            {
                //Check enemy type

                if (objectCol.GetComponent<GuardScript>() != null) //Normal guard
                {
                    objectCol.GetComponent<GuardScript>().KillGuard();
                }
                else if (objectCol.GetComponent<DroneGuard>() != null)  //Drone guard
                {
                    objectCol.GetComponent<DroneGuard>().KillDrone();
                }
                else if (objectCol.GetComponent<HeavyGuardScript>() != null) //Heavy guard
                {
                    objectCol.GetComponent<HeavyGuardScript>().KillGuard();
                }
            }

            if (other.tag == "FinalBoss")
            {
                other.transform.parent.parent.GetComponent<FinalBossScript>().DamageBossHP(1);
                other.gameObject.SetActive(false);
            }

        }
    }

    private void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.tag == "Ground" || collision.transform.tag == "GrappleSpot" || collision.transform.tag == "TrapDoor")
        {
            isFalling = false;
            rb.useGravity = false;
            StartCoroutine(Fade());

            cameraLogic.Shake(.3f, .2f);

            GetComponent<AudioSource>().Play();
            transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ring = transform.Find("Sound").Find("Ring").gameObject;
            sphere = transform.Find("Sound").Find("Sphere").gameObject;

            StartCoroutine(Break());
        }
    }

    public void DropObject()
    {
        isFalling = true;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.velocity = new Vector3(0, -10, 0);
        gameObject.transform.parent = null;
        Destroy(rope);
    }

    IEnumerator Fade()
    {
        yield return new WaitForSeconds(1f);
        GetComponent<Animator>().SetTrigger("Fade");
        yield return new WaitForSeconds(1.5F);
        Destroy(gameObject);
    }

    IEnumerator Break()
    {
        ring.SetActive(true);
        sphere.SetActive(true);
        ring.GetComponent<Animator>().Play("Noise Animation");
        transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        cameraLogic.Shake(.1f, .1f);


        yield return new WaitForSeconds(.5f);
        Destroy(sphere);
        Destroy(ring);
        Destroy(transform.gameObject);

    }
}
