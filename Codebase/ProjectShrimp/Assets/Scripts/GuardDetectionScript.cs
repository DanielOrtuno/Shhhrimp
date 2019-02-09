using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardDetectionScript : MonoBehaviour
{
    #region private variables

    float fieldOfView;
    public float detectionMeter;
    GuardScript guardScript;
    HeavyGuardScript heavyGuardScript;
    Quaternion originalRotation;
    GameObject player;

    public GameObject playerGhost;

    AudioSource enemyAudio;
    public bool alertSoundPlayed;
    public bool chaseSoundPlayed;
    public bool playerInVisionCone;

    #endregion

    public float range;

    public GameObject rayCastOrigin;

    public bool lookAtPlayer;

    public Transform interest;

    public bool isListening;

    // Use this for initialization
    void Start()
    {
        if (transform.parent.GetComponent<GuardScript>() != null)
        {
            guardScript = gameObject.transform.parent.GetComponent<GuardScript>();
        }
        else
        {
            heavyGuardScript = gameObject.transform.parent.GetComponent<HeavyGuardScript>();
        }

        fieldOfView = 18;
        player = GameObject.Find("Player");
        originalRotation = transform.rotation;
        alertSoundPlayed = false;
        playerInVisionCone = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (guardScript != null)
        {
            if (guardScript.state == GuardScript.GuardStates.dead || guardScript.state == GuardScript.GuardStates.stunned)
                return;

            if (guardScript.state != GuardScript.GuardStates.stunned || guardScript.state != GuardScript.GuardStates.dead)
            {

                if (Vector3.Distance(player.transform.position, transform.parent.position) < .8f && player.GetComponent<PlayerController>().GetPlayerHP() > 0 &&
                    !player.GetComponent<PlayerController>().isHidden && guardScript.state != GuardScript.GuardStates.stunned)
                {
                    if (guardScript.state != GuardScript.GuardStates.chase)
                        guardScript.guardGroupScript.AlertGroup();
                    else if (guardScript.state == GuardScript.GuardStates.chase)
                        guardScript.isPlayerInSight = true;


                    if (player.transform.position.x > transform.position.x)
                    {
                        transform.eulerAngles = new Vector3(0, -90, 0);
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 90, 0);
                    }

                }

                if (player != null)
                {
                    // Guard can see you when in chase mode and close to you
                    if ((Vector3.Distance(transform.position, player.transform.position) <= 2 && guardScript.state == GuardScript.GuardStates.chase))
                    {
                        guardScript.isPlayerInSight = true;
                        guardScript.state = GuardScript.GuardStates.chase;
                        lookAtPlayer = true;

                        return;
                    }

                    if (lookAtPlayer && Vector3.Angle(transform.forward, player.transform.position - guardScript.gameObject.transform.position) <= fieldOfView)
                        if (interest)
                            if (Vector3.Distance(interest.transform.position, transform.position) > .2f)
                                LookAtTarget(interest);

                    DrawRayCasts();

                }
            }
        }
        else
        {
            if (heavyGuardScript.state == HeavyGuardScript.GuardStates.dead || heavyGuardScript.state == HeavyGuardScript.GuardStates.stunned)
                return;

            if (heavyGuardScript.state != HeavyGuardScript.GuardStates.stunned || heavyGuardScript.state != HeavyGuardScript.GuardStates.dead)
            {

                if (Vector3.Distance(player.transform.position, transform.parent.position) < .8f && player.GetComponent<PlayerController>().GetPlayerHP() > 0 && !player.GetComponent<PlayerController>().isHidden &&  heavyGuardScript.state != HeavyGuardScript.GuardStates.stunned)
                {
                    if (heavyGuardScript.state != HeavyGuardScript.GuardStates.chase)
                        heavyGuardScript.guardGroupScript.AlertGroup();
                    else if (heavyGuardScript.state == HeavyGuardScript.GuardStates.chase)
                        heavyGuardScript.isPlayerInSight = true;


                    if (player.transform.position.x > transform.position.x)
                    {
                        transform.eulerAngles = new Vector3(0, -90, 0);
                    }
                    else
                    {
                        transform.eulerAngles = new Vector3(0, 90, 0);
                    }

                }

                if (player != null)
                {
                    // Guard can see you when in chase mode and close to you
                    if ((Vector3.Distance(transform.position, player.transform.position) <= 2 && heavyGuardScript.state == HeavyGuardScript.GuardStates.chase))
                    {
                        heavyGuardScript.isPlayerInSight = true;
                        heavyGuardScript.state = HeavyGuardScript.GuardStates.chase;
                        lookAtPlayer = true;

                        return;
                    }

                    if (lookAtPlayer && Vector3.Angle(transform.forward, player.transform.position - heavyGuardScript.gameObject.transform.position) <= fieldOfView)
                        if (Vector3.Distance(interest.transform.position, transform.position) > .2f)
                            LookAtTarget(interest);


                    DrawRayCasts();

                }
            }
        }

    }

    void DrawRayCasts()
    {
        if (guardScript != null)
        {
            int layerMask = LayerMask.GetMask("Player", "Default", "Ground");
            Vector3 raycastOrigin = transform.TransformDirection(Vector3.forward);

            if (Vector3.Angle(raycastOrigin, player.transform.position - guardScript.gameObject.transform.position) <= fieldOfView)
            {
                Debug.DrawRay(transform.position, player.transform.position - gameObject.transform.position, Color.yellow);
                RaycastHit hit;

                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, range, layerMask))
                {
                    if (hit.transform.tag == "Player" && !player.GetComponent<PlayerController>().isHidden)
                    {
                        if (player.GetComponent<PlayerController>().inSpotlight)
                        {
                            guardScript.playerGhost.transform.position = player.transform.position;
                            interest = player.transform;

                            guardScript.isPlayerInSight = true;
                            guardScript.canShoot = true;
                            guardScript.arrivedToInterest = false;
                            isListening = false;
                            lookAtPlayer = true;
                            guardScript.arm.SetActive(true);


                            if (hit.distance <= 3f)
                            {
                                guardScript.guardGroupScript.AlertGroup();
                            }
                            else
                            {
                                if (guardScript.state != GuardScript.GuardStates.chase)
                                {
                                    guardScript.state = GuardScript.GuardStates.alert;
                                }
                            }

                        }
                        else
                        {
                            if (hit.distance <= 3)
                            {
                                guardScript.playerGhost.transform.position = player.transform.position;
                                interest = player.transform;

                                guardScript.isPlayerInSight = true;
                                guardScript.canShoot = true;
                                guardScript.arrivedToInterest = false;
                                isListening = false;
                                lookAtPlayer = true;
                                guardScript.arm.SetActive(true);

                                guardScript.guardGroupScript.AlertGroup();
                            }
                            else
                            {
                                guardScript.isPlayerInSight = false;
                                playerInVisionCone = false;
                                if (!isListening)
                                    lookAtPlayer = false;
                            }

                        }
                    }
                    else
                    {
                        guardScript.isPlayerInSight = false;
                        playerInVisionCone = false;
                        if (!isListening)
                            lookAtPlayer = false;
                    }
                }
                else
                {
                    guardScript.isPlayerInSight = false;
                    playerInVisionCone = false;
                    if (!isListening)
                        lookAtPlayer = false;
                }
            }
            else
            {
                guardScript.isPlayerInSight = false;
                playerInVisionCone = false;
                if (!isListening)
                    lookAtPlayer = false;
            }
        }
        else
        {
            int layerMask = LayerMask.GetMask("Player", "Default", "Ground");
            Vector3 raycastOrigin = transform.TransformDirection(Vector3.forward);

            if (Vector3.Angle(raycastOrigin, player.transform.position - heavyGuardScript.gameObject.transform.position) <= fieldOfView)
            {
                Debug.DrawRay(transform.position, player.transform.position - gameObject.transform.position, Color.yellow);
                RaycastHit hit;

                if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, range, layerMask))
                {
                    if (hit.transform.tag == "Player" && !player.GetComponent<PlayerController>().isHidden)
                    {
                        if (player.GetComponent<PlayerController>().inSpotlight)
                        {
                            heavyGuardScript.playerGhost.transform.position = player.transform.position;
                            interest = player.transform;

                            heavyGuardScript.isPlayerInSight = true;
                            heavyGuardScript.canShoot = true;
                            heavyGuardScript.arrivedToInterest = false;
                            isListening = false;
                            lookAtPlayer = true;
                            heavyGuardScript.light.SetActive(true);

                            if (hit.distance <= 4f)
                            {
                                heavyGuardScript.guardGroupScript.AlertGroup();
                            }
                            else
                            {
                                if (heavyGuardScript.state != HeavyGuardScript.GuardStates.chase)
                                {
                                    heavyGuardScript.state = HeavyGuardScript.GuardStates.alert;
                                }
                            }

                        }
                        else
                        {
                            if (hit.distance <= 3)
                            {
                                heavyGuardScript.playerGhost.transform.position = player.transform.position;
                                interest = player.transform;

                                heavyGuardScript.isPlayerInSight = true;
                                heavyGuardScript.canShoot = true;
                                heavyGuardScript.arrivedToInterest = false;
                                isListening = false;
                                lookAtPlayer = true;
                                heavyGuardScript.light.SetActive(true);

                                heavyGuardScript.guardGroupScript.AlertGroup();
                            }
                            else
                            {
                                heavyGuardScript.isPlayerInSight = false;
                                playerInVisionCone = false;
                                if (!isListening)
                                    lookAtPlayer = false;
                            }
                        }
                    }
                    else
                    {
                        heavyGuardScript.isPlayerInSight = false;
                        playerInVisionCone = false;
                        if (!isListening)
                            lookAtPlayer = false;
                    }
                }
                else
                {
                    heavyGuardScript.isPlayerInSight = false;
                    playerInVisionCone = false;
                    if (!isListening)
                        lookAtPlayer = false;
                }
            }
            else
            {
                heavyGuardScript.isPlayerInSight = false;
                playerInVisionCone = false;
                if (!isListening)
                    lookAtPlayer = false;
            }
        }
    }

    public void LookAtTarget(Transform target)
    {
        if (target != null)
        {
            //Direction vector
            Vector3 direction = (target.position - transform.position).normalized;

            //Quaternion to represent the rotation
            Quaternion lookRotation = Quaternion.LookRotation(direction);

            //Rotate head
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 100);

            if (guardScript != null)
            {
                guardScript.arm.transform.parent.LookAt(target.position);
                guardScript.arm.transform.parent.eulerAngles = new Vector3(guardScript.arm.transform.parent.eulerAngles.x, guardScript.arm.transform.parent.eulerAngles.y, guardScript.arm.transform.parent.eulerAngles.z);
            }
            else
            {
                heavyGuardScript.light.transform.parent.parent.LookAt(target.position);
                heavyGuardScript.light.transform.parent.parent.eulerAngles = new Vector3(heavyGuardScript.light.transform.parent.parent.eulerAngles.x, heavyGuardScript.light.transform.parent.parent.eulerAngles.y, heavyGuardScript.light.transform.parent.parent.eulerAngles.z);
            }
        }

    }

    public void ResetDirection()
    {
        //Direction vector
        Vector3 direction = (rayCastOrigin.transform.position - transform.position).normalized;

        //Quaternion to represent the rotation
        Quaternion lookRotation = Quaternion.LookRotation(direction);

        //Rotate head
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, 10000);
        //guardScript.light.transform.parent.rotation = Quaternion.Slerp(guardScript.light.transform.parent.rotation, lookRotation, 100000);


        if (guardScript != null)
        {
            //Direction vector
            Vector3 direction2 = (rayCastOrigin.transform.position - guardScript.light.transform.parent.position).normalized;

            //Quaternion to represent the rotation
            Quaternion lookRotation2 = Quaternion.LookRotation(direction2);

            //Rotate head
            guardScript.arm.transform.parent.rotation = Quaternion.Slerp(guardScript.arm.transform.parent.rotation, lookRotation2, 10000);
        }
        else
        {
            //Direction vector
            Vector3 direction2 = (rayCastOrigin.transform.position - heavyGuardScript.light.transform.parent.position).normalized;

            //Quaternion to represent the rotation
            Quaternion lookRotation2 = Quaternion.LookRotation(direction2);

            //Rotate head
            heavyGuardScript.light.transform.parent.parent.rotation = Quaternion.Slerp(heavyGuardScript.light.transform.parent.parent.rotation, lookRotation2, 10000);
        }


        lookAtPlayer = false;
        interest = null;
        alertSoundPlayed = false;
        chaseSoundPlayed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy" && !guardScript.isPlayerInSight && guardScript.state != GuardScript.GuardStates.chase)
        {
            if (other.gameObject.GetComponent<GuardScript>())
            {
                if (other.gameObject.GetComponent<GuardScript>().state == GuardScript.GuardStates.dead)
                {
                    if (guardScript != null)
                    {
                        interest = other.gameObject.transform;
                        //playerGhost.transform.position = interest.position;

                        guardScript.light.SetActive(true);
                        lookAtPlayer = true;
                        guardScript.state = GuardScript.GuardStates.alert;
                        other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                        isListening = true;
                        StartCoroutine(guardScript.ReactToDeadGuard());
                    }
                    else
                    {
                        interest = other.gameObject.transform;
                        //playerGhost.transform.position = interest.position;

                        heavyGuardScript.light.SetActive(true);
                        lookAtPlayer = true;
                        heavyGuardScript.state = HeavyGuardScript.GuardStates.alert;
                        other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                        isListening = true;
                        StartCoroutine(heavyGuardScript.ReactToDeadGuard());
                    }

                }
            }
            else
            {
                if (other.gameObject.GetComponent<HeavyGuardScript>().state == HeavyGuardScript.GuardStates.dead)
                {
                    if (guardScript != null)
                    {
                        interest = other.gameObject.transform;
                        //playerGhost.transform.position = interest.position;

                        guardScript.arm.SetActive(true);
                        lookAtPlayer = true;
                        guardScript.state = GuardScript.GuardStates.alert;
                        other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                        isListening = true;
                        StartCoroutine(guardScript.ReactToDeadGuard());
                    }
                    else
                    {
                        interest = other.gameObject.transform;
                        //playerGhost.transform.position = interest.position;

                        heavyGuardScript.light.SetActive(true);
                        lookAtPlayer = true;
                        heavyGuardScript.state = HeavyGuardScript.GuardStates.alert;
                        other.gameObject.GetComponent<CapsuleCollider>().enabled = false;
                        isListening = true;
                        StartCoroutine(heavyGuardScript.ReactToDeadGuard());
                    }

                }
            }
        }
    }



}

