using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardGroupScript : MonoBehaviour
{

    public GameObject[] guards;
    public GameObject player;
    public GameObject chaseIcon;
    public GameObject playerGhost;
    public GameObject spotterGuard;
    bool areAlerted = false;
    public bool isPlayerInSight;
    public float stateTimer;
    bool wasSoundPlayed;
    bool turnOfGuards;
    [SerializeField]
    public GuardNavigationScript guardNavigationScript;
    public AudioClip alertSound;

    private void Start()
    {
        wasSoundPlayed = false;
        turnOfGuards = false;
        player = FindObjectOfType<PlayerController>().gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.GetComponent<PlayerController>().GetPlayerHP() <= 0)
        {
            playerGhost.SetActive(false);
            chaseIcon.SetActive(false);

            for (int i = 0; i < guards.Length; i++)
            {
                ResetGuard(guards[i]);
            }
        }

        if (areAlerted)
        {
            // Check if a guard can see the player
            for (int i = 0; i < guards.Length; i++)
            {
                GuardScript guardScript = null;
                HeavyGuardScript heavyGuardScript = null;

                // Get the guardscript
                if (guards[i].GetComponent<GuardScript>() != null)
                {
                    guardScript = guards[i].GetComponent<GuardScript>();
                }
                else
                {
                    heavyGuardScript = guards[i].GetComponent<HeavyGuardScript>();
                }

                if (guardScript != null)
                {
                    if (guardScript.isPlayerInSight)
                    {
                        isPlayerInSight = true;
                        break;
                    }
                    else
                    {
                        isPlayerInSight = false;
                    }
                }
                else
                {
                    if (heavyGuardScript.isPlayerInSight)
                    {
                        isPlayerInSight = true;
                        break;
                    }
                    else
                    {
                        isPlayerInSight = false;
                    }
                }
            }

            if (isPlayerInSight)
            {
                //Player can't hide
                player.GetComponent<PlayerController>().isChased = true;

                stateTimer = 10f;

                playerGhost.transform.position = player.transform.position;
                playerGhost.SetActive(false);
            }
            else
            {
                stateTimer -= Time.deltaTime;

                //Let player hide again
                player.GetComponent<PlayerController>().isChased = false;

                playerGhost.SetActive(true);
            }

            if (stateTimer <= 0 && !turnOfGuards)
            {
                wasSoundPlayed = false;
                turnOfGuards = true;
                chaseIcon.SetActive(false);
                areAlerted = false;
                if (spotterGuard != null)
                    spotterGuard.transform.Find("Detector").GetComponent<SpotterGuardScript>().isChilling = true;

                for (int i = 0; i < guards.Length; i++)
                {
                    ResetGuard(guards[i]);

                }
                playerGhost.SetActive(false);
                stateTimer = 10;
            }
        }
    }

    public void AlertGroup()
    {
        if(!wasSoundPlayed)
        {
            wasSoundPlayed = true;
            AudioSource.PlayClipAtPoint(alertSound, Camera.main.transform.position);
        }

        chaseIcon.SetActive(true);

        if (spotterGuard != null)
            spotterGuard.transform.Find("Detector").GetComponent<SpotterGuardScript>().isChilling = false;

        for (int i = 0; i < guards.Length; i++)
        {
            GuardScript guardScript = null;
            HeavyGuardScript heavyGuardScript = null;

            if (guards[i].GetComponent<GuardScript>())
            {
                guardScript = guards[i].GetComponent<GuardScript>();
            }
            else
            {
                heavyGuardScript = guards[i].GetComponent<HeavyGuardScript>();
            }

            if (guardScript != null)
            {
                if (guardScript.state != GuardScript.GuardStates.dead)
                {
                    AlertGuard(guardScript);
                }
            }
            else
            {
                if (heavyGuardScript.state != HeavyGuardScript.GuardStates.dead)
                {
                    AlertGuard(heavyGuardScript);
                }
            }
        }

        areAlerted = true;
        turnOfGuards = false;

        player.GetComponent<PlayerController>().NotNearGuard();
        //chaseIcon.SetActive(true);
    }

    void AlertGuard(GuardScript guard)
    {
        guard.state = GuardScript.GuardStates.chase;
        guard.commandRenderer.gameObject.SetActive(false);
        guard.guardDetectionScript.interest = playerGhost.transform;
        if (guard.destination.x < player.transform.position.x)
            guard.destination = player.transform.position - new Vector3(1, 0, 0);
        else
            guard.destination = player.transform.position + new Vector3(1, 0, 0);
        guard.guardDetectionScript.isListening = false;
        guard.playerGhost.SetActive(false);
        guard.light.SetActive(true);
        guard.arm.SetActive(true);
        // guard.canShoot = true;
        guard.guardDetectionScript.lookAtPlayer = true;
        turnOfGuards = false;

        guard.InitialPatrolObject.SetActive(false);
        guard.FinalPatrolObject.SetActive(false);

        guard.transform.Find("HitBox").gameObject.SetActive(false);

        guard.transform.Find("RedOutline").gameObject.SetActive(false);

    }

    void AlertGuard(HeavyGuardScript guard)
    {
        guard.state = HeavyGuardScript.GuardStates.chase;
        guard.guardDetectionScript.interest = playerGhost.transform;
        if (guard.destination.x < player.transform.position.x)
            guard.destination = player.transform.position - new Vector3(1, 0, 0);
        else
            guard.destination = player.transform.position + new Vector3(1, 0, 0);
        guard.guardDetectionScript.isListening = false;
        guard.playerGhost.SetActive(false);
        guard.light.SetActive(true);
        // guard.canShoot = true;
        guard.guardDetectionScript.lookAtPlayer = true;
        turnOfGuards = false;

        guard.InitialPatrolObject.SetActive(false);
        guard.FinalPatrolObject.SetActive(false);
    }

    void ResetGuard(GameObject _guard)
    {
        GuardScript guardScript = null;
        HeavyGuardScript heavyGuardScript = null;

        if (_guard.GetComponent<GuardScript>())
        {
            guardScript = _guard.GetComponent<GuardScript>();
        }
        else
        {
            heavyGuardScript = _guard.GetComponent<HeavyGuardScript>();
        }


        if (guardScript != null)
        {
            if (guardScript.state != GuardScript.GuardStates.dead)
            {
                guardScript.state = GuardScript.GuardStates.patrol;
                guardScript.guardDetectionScript.isListening = false;
                guardScript.guardDetectionScript.lookAtPlayer = false;

                guardScript.destination = guardScript.initialPatrolingPosition;
                guardScript.guardDetectionScript.ResetDirection();
                guardScript.arrivedToInterest = false;
                guardScript.InitialPatrolObject.SetActive(true);
                guardScript.FinalPatrolObject.SetActive(false);


                guardScript.transform.LookAt(guardScript.destination);
                guardScript.arm.SetActive(false);
                guardScript.speed = 2.5f;
                guardScript.audioPlayedAlready = false;
            }
        }
        else
        {
            if (heavyGuardScript.state != HeavyGuardScript.GuardStates.dead)
            {
                heavyGuardScript.state = HeavyGuardScript.GuardStates.patrol;
                heavyGuardScript.guardDetectionScript.isListening = false;
                heavyGuardScript.guardDetectionScript.lookAtPlayer = false;

                heavyGuardScript.destination = heavyGuardScript.initialPatrolingPosition;
                heavyGuardScript.guardDetectionScript.ResetDirection();
                heavyGuardScript.arrivedToInterest = false;
                heavyGuardScript.InitialPatrolObject.SetActive(true);
                heavyGuardScript.FinalPatrolObject.SetActive(false);


                heavyGuardScript.transform.LookAt(heavyGuardScript.destination);
                heavyGuardScript.light.SetActive(false);
                heavyGuardScript.speed = 2.5f;
                heavyGuardScript.audioPlayedAlready = false;
            }
        }

    }

}

