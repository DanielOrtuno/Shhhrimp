using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    static public int collectedKeys;
    bool neardoor;

    void Start()
    {
        if (transform.tag == "Door" || transform.tag == "MarketLevelObjectiveDoor" || transform.tag == "ResturantDoor")
        {
            anim = transform.parent.parent.GetComponent<Animator>();
            transform.parent = null;
            collectedKeys = 0;
        }
    }

    void Update()
    {
        if (neardoor)
        {
            if (transform.tag == "MarketLevelObjectiveDoor")
            {
                if (Input.GetKeyDown(KeyCode.E) && collectedKeys == 3)
                {
                    if (Input.GetKeyDown(KeyCode.E) && !anim.GetBool("isOpen"))
                        StartCoroutine(OpenDoor());
                    else if (Input.GetKeyDown(KeyCode.E))
                        StartCoroutine(CloseDoor());
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E) && collectedKeys != 3)
                        gameObject.transform.Find("LockedDoorSound").GetComponent<AudioSource>().Play();
                }
            }

            if (transform.tag == "ResturantDoor")
            {
                if (Input.GetKeyDown(KeyCode.E) && collectedKeys != 1)
                {
                    gameObject.transform.Find("LockedDoorSound").GetComponent<AudioSource>().Play();
                }
                else
                {
                    if (Input.GetKeyDown(KeyCode.E) && !anim.GetBool("isOpen"))
                        StartCoroutine(OpenDoor());
                    else if (Input.GetKeyDown(KeyCode.E))
                        StartCoroutine(CloseDoor());
                }
            }

            if (transform.tag == "Door")
            {
                if (Input.GetKeyDown(KeyCode.E) && !anim.GetBool("isOpen"))
                    StartCoroutine(OpenDoor());
                else if (Input.GetKeyDown(KeyCode.E))
                    StartCoroutine(CloseDoor());
            }

            if (transform.tag == "TrapDoor")
            {
                if (Input.GetKeyDown(KeyCode.E))
                {
                    if (!transform.parent.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("TrapDoor"))
                        StartCoroutine(Rotate());
                }
            }

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag == "Enemy" && transform.tag != "MarketLevelObjectiveDoor" && transform.tag != "ResturantDoor")
            if (anim.GetBool("isOpen"))
                StartCoroutine(CloseDoor());

        if (other.transform.tag == "Player")
            neardoor = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && transform.tag != "MarketLevelObjectiveDoor" && transform.tag != "ResturantDoor")
            if (!anim.GetBool("isOpen"))
                StartCoroutine(OpenDoor());


        if (other.transform.tag == "Player")
            neardoor = true;
    }
    
    IEnumerator Rotate()
    {
        transform.parent.GetComponent<Animator>().Play("TrapDoor");
        transform.Find("Collider").GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(.5f);
        transform.Find("Collider").GetComponent<BoxCollider>().enabled = true;
    }

    IEnumerator OpenDoor()
    {
        anim.Play("DoorOpen");
        anim.transform.Find("Door").GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(.75f);
        anim.transform.Find("Door").GetComponent<BoxCollider>().enabled = true;
        anim.SetBool("isOpen", true);
    }

    IEnumerator CloseDoor()
    {
        anim.Play("DoorClose");
        anim.transform.Find("Door").GetComponent<BoxCollider>().enabled = false;
        yield return new WaitForSeconds(.9f);
        anim.transform.Find("Door").GetComponent<BoxCollider>().enabled = true;
        anim.SetBool("isOpen", false);
    }
}
