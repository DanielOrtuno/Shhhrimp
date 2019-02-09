using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DartScript : MonoBehaviour
{
    [SerializeField]
    public AudioClip breakingLightsSound;

    GameObject collision = null;

    Animator anim;

    float lifeExpectancy = 10;
    public int touched;
    bool lightDestroyed = false;

    private void OnTriggerEnter(Collider other)
    {
        if (gameObject.name == "Dart(Clone)")
        {
            if (other.transform.tag == "BreakableLight")
            {
                anim = other.transform.GetComponent<Animator>();

                other.transform.Find("LightsBreakingObject").gameObject.SetActive(true);
                collision = other.gameObject;

                other.transform.GetComponent<AudioSource>().pitch = Random.Range(.5f, 1);
                other.transform.GetComponent<AudioSource>().Play();
                lightDestroyed = true;
                StartCoroutine(Break());
            }
            else if (other.transform.tag == "FallingObjectRope")
            {
                other.transform.Find("FallingObject").gameObject.GetComponent<FallingObjectScript>().DropObject();
                lifeExpectancy = 2;
            }
            else if (other.transform.tag == "DroneGuard")
            {
                other.GetComponent<DroneGuard>().KillDrone();
                FindObjectOfType<PlayerController>().isStunned = false;
                FindObjectOfType<PlayerController>().transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = true;
                FindObjectOfType<PlayerController>().GetComponent<Animator>().SetBool("Stunned", false);
                FindObjectOfType<PlayerController>().GetComponent<Animator>().speed = 1;
                Destroy(gameObject);
            }
        }
        else
        {
            if (other.transform.tag == "GrappleSpot" && transform.name == "Grappling(Clone)")
            {
                FindObjectOfType<PlayerController>().isGrappling = true;
                FindObjectOfType<PlayerController>().gameObject.transform.Find("SFX").Find("GrapplingSound").GetComponent<AudioSource>().Play();
                other.GetComponent<BoxCollider>().enabled = false;
                Destroy(gameObject);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!lightDestroyed)
            if (collision.gameObject.layer == 0 || collision.gameObject.layer == 17)
                Destroy(gameObject);

    }
    private void OnDestroy()
    {
        if (transform.name == "Dart(Clone)")
            FindObjectOfType<PlayerController>().dartCount++;
        else
            FindObjectOfType<PlayerController>().grapplingCount++;
    }

    IEnumerator Break()
    {
        anim.SetBool("Destroyed", true);
        collision.transform.Find("LightsBreakingObject").Find("Ring").gameObject.GetComponent<Animator>().Play("LightBreaking");
        transform.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        collision.transform.Find("Mesh").gameObject.SetActive(false);
        collision.transform.Find("Cone").gameObject.SetActive(false);
        collision.transform.Find("Spotlight").gameObject.SetActive(false);

        yield return new WaitForSeconds(.5f);

        Destroy(collision);
        Destroy(gameObject);

    }

    // Light's audio
    void PlayBreakingLightsSound()
    {
        AudioSource.PlayClipAtPoint(breakingLightsSound, Camera.main.transform.position);
    }

    private void Update()
    {
        if (transform.name == "Dart(Clone)")
            if (lifeExpectancy <= 0)
            {
                Destroy(gameObject);
            }
            else
                lifeExpectancy -= Time.deltaTime;

        if (transform.name == "Grappling(Clone)")
        {
            GetComponent<LineRenderer>().SetPosition(0, FindObjectOfType<PlayerController>().transform.position);
            GetComponent<LineRenderer>().SetPosition(1, transform.position);
        }
    }
}
