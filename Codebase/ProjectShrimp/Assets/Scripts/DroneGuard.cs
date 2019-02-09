using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneGuard : MonoBehaviour
{
    public float distanceFromOrigin;
    public float speed;
    public float rotationSpeed;
    public float rotationAngle1;
    public float rotationAngle2;
    public float range;
    public float timer;

    bool isPlayerInSight;
    bool isDead;
    public bool canShoot;
    bool flipper;

    Animator animator;
    GameObject bullet;
    GameObject gun;
    float initialPos;

    CameraLogic cameraLogic;




    void Start()
    {
        gun = transform.Find("Gun").gameObject;

        animator = GetComponent<Animator>();
        bullet = transform.Find("Bullet").gameObject;

        isDead = false;
        isPlayerInSight = false;
        timer = 0;

        initialPos = transform.position.x;

        cameraLogic = GameObject.Find("Main Camera").GetComponent<CameraLogic>();
    }

    void Update()
    {

        if (!isPlayerInSight && !isDead)
            if (flipper)
                if (transform.position.x < initialPos + distanceFromOrigin)
                    transform.position = Vector3.MoveTowards(transform.position, transform.position + new Vector3(distanceFromOrigin, 0, 0), speed * Time.deltaTime);
                else
                    flipper = !flipper;
            else if (!flipper)
                if (transform.position.x > initialPos - distanceFromOrigin)
                    transform.position = Vector3.MoveTowards(transform.position, transform.position - new Vector3(distanceFromOrigin, 0, 0), speed * Time.deltaTime);
                else
                    flipper = !flipper;
    }


    void LateUpdate()
    {
        if (bullet.activeSelf)
            canShoot = false;
        else
            canShoot = true;

        if (!isDead)
        {
            RaycastHit hit;

            int layerMask = LayerMask.GetMask("Player", "Default", "Ground");

            if (Physics.Raycast(transform.position, -gun.transform.up, out hit, 5f, layerMask))
            {
                if (hit.transform.tag == "Player")
                    if (hit.transform.GetComponent<PlayerController>().GetPlayerHP() != 0)
                    {
                        hit.transform.GetComponent<PlayerController>().isStunned = true;
                        hit.transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = false;
                        hit.transform.GetComponent<PlayerController>().inSpotLightTimer = .25f;
                        isPlayerInSight = true;


                        cameraLogic.Shake(.1f, .1f);
                    }
                    else
                    {
                        isPlayerInSight = false;
                        hit.transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = true;
                    }
                else
                {
                    isPlayerInSight = false;
                    FindObjectOfType<PlayerController>().isStunned = false;
                    FindObjectOfType<PlayerController>().transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = true;
                }
            }
            else
            {
                isPlayerInSight = false;
                FindObjectOfType<PlayerController>().isStunned = false;
                FindObjectOfType<PlayerController>().transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = true;
            }
        }
    }


    public void KillDrone()
    {
        isPlayerInSight = false;
        isDead = true;
        gameObject.layer = 14;
        transform.Find("Gun").Find("Light").gameObject.SetActive(false);
        GetComponent<BoxCollider>().isTrigger = false;
        GetComponent<Rigidbody>().useGravity = true;
        animator.Play("DroneGuardDeath");

        //PlaySound;
        //Turn on gravity for a second

    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
    }
}
