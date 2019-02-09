using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMaker : MonoBehaviour
{
    PlayerController player;

    GameObject ring = null;
    GameObject sphere = null;

    public AudioClip noiseMakerSound;

    CameraLogic cameraLogic;

    void Start()
    {
        cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
        player = FindObjectOfType<PlayerController>();
    }

    void Update()
    {
        if (transform.position.y < -100)
        {
            player.GetComponent<PlayerController>().noiseMakerCount++;
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.tag == "Spikes")
        //{
        //    player.GetComponent<PlayerController>().noiseMakerCount++;
        //    cameraLogic.Shake(.1f, .1f);
        //    Destroy(gameObject);
        //}
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "player")
        {
            transform.gameObject.GetComponent<Rigidbody>().useGravity = false;
            transform.gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            ring = transform.Find("GameObject").Find("Ring").gameObject;
            sphere = transform.Find("GameObject").Find("Sphere").gameObject;

            StartCoroutine(Break());
            AudioSource.PlayClipAtPoint(noiseMakerSound, Camera.main.transform.position);
            FindObjectOfType<GameManagerController>().NoiseMakerEffect.transform.position = transform.position;
            FindObjectOfType<GameManagerController>().NoiseMakerEffect.Play(true);
        }
        
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
