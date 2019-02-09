using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeavyGuardScript : MonoBehaviour {

    bool push;
    #region Private variables
    //float firingTimer;
    public AudioSource enemyShootingAudio;

    public GameObject InitialPatrolObject;
    public GameObject FinalPatrolObject;

    Animator animator;
    bool isSearching;
    new Rigidbody rigidbody;
    #endregion

    #region Public variables

    public enum GuardStates { patrol = 0, alert, chase, freeze, dead, stunned };
    public GuardStates state;

    public bool isPlayerInSight;
    public bool canMove;
    public bool isFiring;
    public bool canShoot;
    public bool arrivedToInterest;
    public bool isNormalGuard;

    public float stateTimer;
    public float currentSpeed;

    public float minStoppingDistance;
    public float maxStoppingDistance;
    public float speed;

    public bool audioPlayedAlready;
    #endregion

    #region Unity Variables

    public Vector3 initialPatrolingPosition;
    public Vector3 finalPatrolingPosition;
    public Vector3 destination;

    public Sprite questionMark;
    public Sprite exclamationMark;
    public Sprite stunnedIcon;

    public GameObject[] bulletPool;
    public GameObject player;
    public new GameObject light;
    public GameObject playerGhost;

    public SpriteRenderer sr;
    public SpriteRenderer commandRenderer;

    public GuardDetectionScript guardDetectionScript;
    public GuardGroupScript guardGroupScript;
    GuardNavigationScript guardNavigationScript;


    #endregion

    #region Base functions

    void Start()
    {
        Initialize();
    }

    void Update()
    {
        
        if (state == GuardStates.dead)
            return;

        if (state != GuardStates.dead)
            UpdateLoop();

        if (canMove)
        {
            if (state == GuardStates.chase)
                currentSpeed = 4;
            else if (state == GuardStates.alert || state == GuardStates.patrol)
                currentSpeed = 2.5f;
        }
        else
            currentSpeed = 0;

    }

    protected void Initialize()
    {
        push = false;
        //Set Patrolling vectors

        InitialPatrolObject = transform.Find("FinalPatrolPosition").gameObject;
        initialPatrolingPosition = InitialPatrolObject.transform.position;
        InitialPatrolObject.transform.parent = null;

        FinalPatrolObject = transform.Find("InitialPatrolPosition").gameObject;
        finalPatrolingPosition = FinalPatrolObject.transform.position;
        FinalPatrolObject.transform.parent = null;

        //Set stateTimer
        stateTimer = 7f;

        isFiring = false;

        arrivedToInterest = false;

        //Set sprite renderer for states
        sr = transform.Find("StateIndicator").gameObject.GetComponent<SpriteRenderer>();

        //Set sprite renderer for command
        if (transform.Find("Command"))
            commandRenderer = transform.Find("Command").gameObject.GetComponent<SpriteRenderer>();

        //Set player object
        player = GameObject.Find("Player");

        //Set GuardDetectionScript
        guardDetectionScript = transform.Find("GuardDetectionComponent").GetComponent<GuardDetectionScript>();

        //Set flashlight
        light = transform.Find("Shoulder").Find("Arm").Find("Light").gameObject;


        // Unparent player ghost
        playerGhost.transform.parent = null;

        guardGroupScript = transform.parent.GetComponent<GuardGroupScript>();

        guardNavigationScript = GameObject.Find("NavigationComponent").GetComponent<GuardNavigationScript>();

        enemyShootingAudio = gameObject.transform.Find("ShootingSound").GetComponent<AudioSource>();

        for (int i = 0; i < bulletPool.Length; i++)
        {
            bulletPool[i].transform.parent = null;
        }


        destination = initialPatrolingPosition;

        rigidbody = GetComponent<Rigidbody>();

        animator = transform.Find("HeavyGuard").GetComponent<Animator>();
        isNormalGuard = false;

    }

    protected void UpdateLoop()
    {
        animator.SetBool("IsMoving", false);

        if (Vector3.Distance(player.transform.position, transform.position) < 1f && !push)
        {
            PlayerController playerController = player.GetComponent<PlayerController>();
            playerController.AddImpact((player.transform.position - transform.position), 40f);
            playerController.DamagePlayerHP(1);
            state = GuardStates.chase;
            guardDetectionScript.interest = player.transform;
            push = true;
            animator.SetTrigger("Push");
        }

        if (player != null)
        {
            if (player.GetComponent<PlayerController>().isHidden)
                isPlayerInSight = false;

            if (player.GetComponent<PlayerController>().GetPlayerHP() <= 0)
                playerGhost.SetActive(false);

            playerGhost.transform.eulerAngles = player.transform.eulerAngles;
        }

        sr.transform.eulerAngles = new Vector3(0, 0, 0);

        switch (state)
        {
            case GuardStates.patrol:
                {
                    speed = 2;
                    // Hide Icon
                    sr.sprite = null;

                    guardNavigationScript.MoveToDestination(transform, destination, speed, canMove);
                }
                break;

            case GuardStates.alert:
                {
                    if (audioPlayedAlready == false)
                    {
                        gameObject.transform.Find("AlertSound").GetComponent<AudioSource>().Play();
                        audioPlayedAlready = true;
                    }

                    speed = 2;
                    // Show Icon
                    sr.color = Color.yellow;
                    sr.sprite = questionMark;

                    AlertStateTimer();


                    if (Vector3.Distance(transform.position, destination) > 2)
                    {
                        transform.LookAt(destination);
                        guardNavigationScript.MoveToDestination(transform, destination, speed, canMove);
                    }
                    else
                    {
                        //if (isNormalGuard)
                           animator.SetBool("IsMoving", false);
                        if (arrivedToInterest == false)
                        {
                            StopCoroutine(ReactToDeadGuard());
                            StartCoroutine(ReactToDeadGuard());
                        }
                        arrivedToInterest = true;
                    }

                }
                break;

            case GuardStates.chase:
                {
                    speed = 4;

                    ////Show icon                   
                    sr.color = Color.red;
                    sr.sprite = exclamationMark;

                    ChaseStateTimer();


                    if (Vector3.Distance(player.transform.position, transform.position) < .2f)
                    {
                        isPlayerInSight = true;
                        player.GetComponent<PlayerController>().inSpotLightTimer = .1f;
                    }
                    else
                    {
                        if (!isPlayerInSight)
                        {

                            if (Vector3.Distance(transform.position, destination) > .5)
                            {
                                if (!arrivedToInterest)
                                {
                                    if (guardGroupScript.playerGhost.transform.position.x > transform.position.x)
                                    {
                                        transform.eulerAngles = new Vector3(0, 90, 0);
                                    }
                                    else
                                    {
                                        transform.eulerAngles = new Vector3(0, -90, 0);
                                    }
                                }

                                guardNavigationScript.MoveToDestination(transform, destination, speed, canMove);
                            }
                            else
                            {
                                guardNavigationScript.MoveToDestination(transform, destination, speed, canMove);
                            }
                        }
                        else
                        {
                            if (Vector3.Distance(player.transform.position, transform.position) > .5)
                                if (player.transform.position.x > transform.position.x)
                                {
                                    transform.eulerAngles = new Vector3(0, 90, 0);
                                }
                                else
                                {
                                    transform.eulerAngles = new Vector3(0, -90, 0);
                                }


                            if (Vector3.Distance(destination, transform.position) > 4)
                                guardNavigationScript.MoveToDestination(transform, destination, speed, canMove);

                            // transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, transform.position.z));

                        }
                    }

                }
                break;

            case GuardStates.freeze:
                {

                    if (isPlayerInSight && !isFiring)
                    {
                        isFiring = true;
                        StartCoroutine("FireTimingEnum");
                    }
                }
                break;

            case GuardStates.stunned:
                {
                    isPlayerInSight = false;
                    canMove = false;
                    canShoot = false;
                }
                break;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "PatrolPoint" && state == GuardStates.patrol)
        {
            if (other.gameObject == FinalPatrolObject || other.gameObject == InitialPatrolObject)
            {
                if (destination == initialPatrolingPosition)
                {
                    destination = finalPatrolingPosition;

                    InitialPatrolObject.SetActive(false);
                    FinalPatrolObject.SetActive(true);
                }
                else
                {
                    destination = initialPatrolingPosition;

                    InitialPatrolObject.SetActive(true);
                    FinalPatrolObject.SetActive(false);
                }

                transform.LookAt(destination);
                StartCoroutine(PatrolRotationEnum());

            }
        }

        if (other.tag == "Smoke" && state != GuardStates.dead)
        {
            StopCoroutine(StunnedEnum());
            StartCoroutine(StunnedEnum());
        }


        if (other.tag == "Box" && state == GuardStates.dead)
        {
            Destroy(gameObject);
        }
    }

    #endregion

    #region Guard behavior

    public void AlertStateTimer()
    {
        // Timer logic
        if (isPlayerInSight)
        {
            if (playerGhost)
                playerGhost.SetActive(false);

            destination = new Vector3(playerGhost.transform.position.x, transform.position.y, transform.position.z);

            stateTimer = 7f;

        }
        else
        {
            playerGhost.SetActive(true);
            stateTimer -= Time.deltaTime;
        }

        if (stateTimer <= 0)
        {
            audioPlayedAlready = false;
            state = GuardStates.patrol;
            guardDetectionScript.isListening = false;
            guardDetectionScript.lookAtPlayer = false;

            destination = initialPatrolingPosition;
            guardDetectionScript.ResetDirection();
            arrivedToInterest = false;
            InitialPatrolObject.SetActive(true);
            FinalPatrolObject.SetActive(false);

            playerGhost.SetActive(false);


            transform.LookAt(destination);
            speed = 2.5f;
        }

        if (guardDetectionScript.isListening == false)
            guardDetectionScript.interest = playerGhost.transform;
    }

    public void ChaseStateTimer()
    {
        if (isPlayerInSight)
        {
            isSearching = false;
            arrivedToInterest = false;

            destination = player.transform.position;

            if (canShoot && !isFiring)
            {
                isFiring = true;
                StartCoroutine(FireTimingEnum());
            }
        }
        else
        {
            destination = new Vector3(guardGroupScript.playerGhost.transform.position.x, transform.position.y, transform.position.z);

            if (arrivedToInterest)
            {
                if (!isSearching)
                {
                    isSearching = true;
                    StartCoroutine(SearchEnum());
                }

            }
            else
            {
                if (Vector3.Distance(destination, transform.position) < .5f)
                {
                    arrivedToInterest = true;
                }

            }


            guardDetectionScript.ResetDirection();




            //guardDetectionScript.interest = guardGroupScript.playerGhost.transform;
        }
    }

    virtual public void Fire(int index, short _damage)
    {
        // Origin Vector
        Vector3 bulletPos;

        if (transform.eulerAngles.y == 90)
        {
            bulletPos = new Vector3(transform.position.x + .5f, transform.position.y, transform.position.z);

        }
        else
        {
            bulletPos = new Vector3(transform.position.x - .5f, transform.position.y, transform.position.z);

        }


        //Assing new position to the object
        bulletPool[index].transform.position = bulletPos;
        bulletPool[index].transform.eulerAngles = new Vector3(0, 0, light.transform.eulerAngles.z + 180);

        bulletPool[index].SetActive(true);

        bulletPool[index].GetComponent<TrailRenderer>().Clear();

        bulletPool[index].GetComponent<BulletScript>().damage = _damage;

        bulletPool[index].GetComponent<Rigidbody>().velocity = -light.transform.right * Time.deltaTime * -2500;


    }


    public void ReactToSound(Vector3 soundPosition)
    {
        if(state != GuardStates.dead && state != GuardStates.stunned)
        {
            StopCoroutine(ReactToDeadGuard());

            if (!isPlayerInSight)
            {
                destination = new Vector3(soundPosition.x, transform.position.y, transform.position.z);

                playerGhost.transform.position = soundPosition;

                if (state != GuardStates.chase)
                    state = GuardStates.alert;



                guardDetectionScript.interest = playerGhost.transform;
                guardDetectionScript.lookAtPlayer = true;

                guardDetectionScript.isListening = true;
                stateTimer = 7f;
                transform.LookAt(destination);
                arrivedToInterest = false;
            }
        }
        
    }

    public virtual void KillGuard()
    {

        animator.SetTrigger("Die");
        StopAllCoroutines();
        state = GuardStates.dead;
        light.SetActive(false);
        transform.Find("GuardDetectionComponent").gameObject.SetActive(false);
        light.transform.parent.gameObject.SetActive(false);
        sr.gameObject.SetActive(false);


        playerGhost.SetActive(false);

    }

    #endregion

    #region Coroutines

    public IEnumerator FireTimingEnum()
    {
        yield return new WaitForSeconds(.8f);
        if (isPlayerInSight && canShoot)
        {
            light.transform.parent.Find("Muzzle Flash").gameObject.SetActive(true);
            Fire(0, 1);
            enemyShootingAudio.pitch = Random.Range(.5f, .69f);
            enemyShootingAudio.Play();
        }
        yield return new WaitForSeconds(.1f);
        light.transform.parent.Find("Muzzle Flash").gameObject.SetActive(false);

        yield return new WaitForSeconds(.9f);
        isFiring = false;
    }

    public IEnumerator PatrolRotationEnum()
    {
        canMove = false;

        yield return new WaitForSeconds(2f);

        transform.LookAt(destination);
        canMove = true;

    }

    public IEnumerator ReactToDeadGuard()
    {
        animator.SetBool("IsMoving", false);
        guardDetectionScript.LookAtTarget(guardDetectionScript.interest);
        yield return new WaitForSeconds(2f);

        guardDetectionScript.ResetDirection();
        yield return new WaitForSeconds(1f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

        yield return new WaitForSeconds(1f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

        yield return new WaitForSeconds(1f);
        guardDetectionScript.isListening = false;

        if (state != GuardStates.chase)
        {
            state = GuardStates.patrol;
        }


        destination = initialPatrolingPosition;
        playerGhost.SetActive(false);
        arrivedToInterest = false;
        transform.LookAt(destination);
        guardDetectionScript.ResetDirection();

        InitialPatrolObject.SetActive(true);
        FinalPatrolObject.SetActive(false);
    }

    public IEnumerator StunnedEnum()
    {
        sr.color = new Color(1, 1, 1);
        sr.sprite = stunnedIcon;

        isFiring = true;
        bool Duque = false;
        if (state == GuardStates.chase)
            Duque = true;

        animator.SetBool("IsMoving", false);
        state = GuardStates.stunned;

        yield return new WaitForSeconds(3f);

        if (!Duque)
        {
            state = GuardStates.freeze;
            light.SetActive(true);
            guardDetectionScript.ResetDirection();
        }
        else
        {
            state = GuardStates.chase;
            isFiring = false;
            yield break;
        }

        yield return new WaitForSeconds(2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);

        yield return new WaitForSeconds(2f);

        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y + 180, transform.eulerAngles.z);
        guardDetectionScript.isListening = false;

        yield return new WaitForSeconds(.5f);
        state = GuardStates.patrol;
        canMove = true;
        canShoot = true;
    }


    public IEnumerator SearchEnum()
    {

        if (!isPlayerInSight)
        {

            if (transform.eulerAngles.y >= 90)
            {
                destination += new Vector3(8, 0, 0);
                //arrivedToInterest = false;
            }
            else
            {
                destination -= new Vector3(8, 0, 0);
                // arrivedToInterest = false;
            }
        }


        yield return new WaitForSeconds(1f);

        if (!isPlayerInSight)
        {

            transform.eulerAngles += new Vector3(0, 180, 0);

            if (transform.eulerAngles.y >= 90)
            {
                destination += new Vector3(8, 0, 0);
                //arrivedToInterest = false;
            }
            else
            {
                destination -= new Vector3(8, 0, 0);
                // arrivedToInterest = false;
            }

        }

        yield return new WaitForSeconds(1f);

        if (!isPlayerInSight)
        {

            transform.eulerAngles += new Vector3(0, 180, 0);

            if (transform.eulerAngles.y >= 90)
            {
                destination += new Vector3(8, 0, 0);
                //arrivedToInterest = false;
            }
            else
            {
                destination -= new Vector3(8, 0, 0);
                // arrivedToInterest = false;
            }


        }

        yield return new WaitForSeconds(1f);

        if (!isPlayerInSight)
        {

            transform.eulerAngles += new Vector3(0, 180, 0);

            destination = playerGhost.transform.position;

        }
    }

    #endregion
    
}
