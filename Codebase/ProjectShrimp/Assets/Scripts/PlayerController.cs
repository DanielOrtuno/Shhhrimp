using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    #region SerializedFields
    [SerializeField]
    public Sprite EKey;

    [SerializeField]
    public Sprite FKey;

    [SerializeField]
    public GameObject sprint;

    #endregion

    #region PlayerVariables
    public int noiseMakerCount;
    public int dartCount;
    public int smokeBombCount;
    public int boxCount;
    public int trapCount;
    public int grapplingCount;

    int layerMaskGrappling;
    int layerMaskDarts;
    int jumpCount;
    public ProjectileType projectile;

    public short playerHP;
    public ushort playerScore = 0;
    short dartsShot;

    public float inSpotLightTimer;
    public float noMoreBunnyHops;
    float freezeTimeWait;
    float speed;
    float jumpSpeed;
    float hidingSpotX;
    float climbingBarrier;
    float gravity;
    readonly float mass = 3.0f;

    public bool isStunned;
    public bool isChased;
    public bool inSpotlight;
    public bool isHidden;
    bool isClimbing;
    bool nearHidingSpot;
    bool nearClimbingSpot;
    bool nearGuardHitBox;
    bool dropObject;
    public bool isGrappling;

    string[] targets;
    #endregion

    #region UnityVariables
    // Vector3 lastestCheckpoint;
    Vector3 moveDirection;
    Vector3 mouse;
    Vector3 impact;
    Vector3[] dartLineRenderer;
    Vector3 playerZPos;

    Animator anim;
    Animator playerAnim;

    CharacterController controller;

    GameObject gameManager;
    GameObject noiseMaker;
    GameObject dart;
    GameObject smokeBomb;
    GameObject grappleObject;
    GameObject guard;
    GameObject breakableHidingSpot;
    GameObject box;
    GameObject trap;
    GameObject grappling;
    GameObject sprintDust;

    SpriteRenderer spriteRenderer;

    CameraLogic cameraLogic;
    #endregion

    public float projectileSpeed;

    public enum ProjectileType
    {
        Darts,
        NoiseMaker,
        SmokeBomb,
    }

    public void Start()
    {
        speed = 5.0F;
        jumpSpeed = 12.0F;
        hidingSpotX = 0;
        jumpCount = 0;
        playerHP = 4;
        dartsShot = 0;
        layerMaskGrappling = LayerMask.GetMask("Ground", "Default", "GrappleSpot");
        layerMaskDarts = LayerMask.GetMask("Ground", "Default", "Light", "GrappleSpot");

        climbingBarrier = 0;
        gravity = 35.0F;

        grapplingCount = 1;
        dartCount = 3;
        projectileSpeed = .3f;
        projectile = 0;
        freezeTimeWait = .1f;
        inSpotLightTimer = 0; ;

        isGrappling = false;
        isHidden = false;
        isClimbing = false;
        nearHidingSpot = false;
        nearClimbingSpot = false;
        nearGuardHitBox = false;
        inSpotlight = false;
        dropObject = true;
        isChased = false;
        isStunned = false;

        targets = new string[] { "BreakableLight", "FallingObjectRope", "DroneGuard" };

        moveDirection = Vector3.zero;
        mouse = Vector3.zero;
        impact = Vector3.zero;
        dartLineRenderer = new Vector3[2];

        grappleObject = null;
        noiseMaker = null;
        dart = null;
        smokeBomb = null;
        breakableHidingSpot = null;
        guard = null;
        box = null;
        trap = null;
        grappling = null;


        playerAnim = GetComponent<Animator>();
        anim = sprint.GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        gameManager = GameObject.Find("GameManager");
        spriteRenderer = GetComponent<SpriteRenderer>();
        cameraLogic = FindObjectOfType<CameraLogic>();
        sprintDust = transform.Find("SprintDust").gameObject;
        transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = true;
    }

    void Update()
    {
        UpdateAnimations();

        if (playerHP > 0)
        {
            if (inSpotLightTimer > 0)
            {
                inSpotlight = true;
                inSpotLightTimer -= Time.deltaTime;
            }
            else
                inSpotlight = false;

            if (!isStunned)
            {
                if (!isHidden)
                    controller.transform.position = new Vector3(transform.position.x, transform.position.y, 1);

                if (impact.magnitude > 0.2)//Push Player
                    controller.Move(impact * Time.deltaTime);
                impact = Vector3.Lerp(impact, Vector3.zero, Time.deltaTime);

                if (noMoreBunnyHops <= 0)
                    Sprint();
                else
                    GetComponent<Animator>().SetBool("Sprinting", false);

                ProjectileChoice();

                controller.Move(moveDirection * Time.deltaTime);
                moveDirection.y -= (gravity * Time.deltaTime);



                if (!isHidden && !isClimbing && !isGrappling) // normal movement
                {
                    if (controller.collisionFlags == CollisionFlags.Above && jumpCount != 2)
                    {
                        moveDirection.y = 0;
                        jumpCount = 2;
                    }

                    FlipPlayerSprite();

                    DropTrap();

                    DropBox();

                    if (!controller.isGrounded) // Movement in air
                        moveDirection.x = Input.GetAxis("Horizontal") * speed;

                    else // on ground
                    {
                        moveDirection = new Vector2(Input.GetAxis("Horizontal") * speed, 0.0f);
                        if (noMoreBunnyHops > 0)
                        {
                            speed = 3;
                            noMoreBunnyHops -= Time.deltaTime;
                        }
                        else
                            jumpCount = 0;

                    }

                    if (jumpCount == 0 && Input.GetButtonDown("Jump"))     // Jumping
                    {
                        if (nearClimbingSpot)
                            isClimbing = true;
                        else
                        {
                            playerAnim.Play("PlayerJumping");
                            moveDirection.y = jumpSpeed;
                            jumpCount = 1;
                            if (isChased && inSpotLightTimer > 0)
                                noMoreBunnyHops = .5f;
                        }
                        gameObject.transform.Find("SFX").Find("JumpingSound").GetComponent<AudioSource>().Play();
                    }
                }
                else if (!isHidden && !isGrappling) // climbing
                {
                    if (climbingBarrier > transform.position.y || Input.GetAxis("Vertical") < 0) // wall boundry and movement
                    {
                        moveDirection.y = Input.GetAxis("Vertical") * speed;
                        if (!gameObject.transform.Find("SFX").Find("WallClimbingSound").GetComponent<AudioSource>().isPlaying && Input.GetAxis("Vertical") != 0)
                            gameObject.transform.Find("SFX").Find("WallClimbingSound").GetComponent<AudioSource>().Play();
                    }
                    else
                        moveDirection.y = 0;
                    if (Input.GetButtonDown("Jump")) // Jump up or off of wall
                    {
                        isClimbing = false;
                        sprintDust.SetActive(false);
                        moveDirection.y = jumpSpeed;
                        jumpCount = 1;
                    }
                }

                Hide();

                KillGuard();

                //Trigger freeze effect
                if (Input.GetKeyDown(KeyCode.LeftControl) && grappleObject == null)
                {
                    gameManager.GetComponent<GameManagerController>().ShowFreezeTimeOutline();
                }
                else if ((Input.GetKeyUp(KeyCode.LeftControl) && grappleObject == null))
                {
                    gameManager.GetComponent<GameManagerController>().HideFreezeTimeOutline();
                }


                if (Input.GetKey(KeyCode.LeftControl) && grappleObject == null)
                {
                    if (Camera.main.transform.position.z >= -20)
                        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Camera.main.transform.position + new Vector3(0, 0, -5), .5f);

                    Cursor.visible = false;

                    ProjectileLines();

                    if (Input.GetMouseButtonUp(0))
                    {
                        switch (projectile)
                        {
                            case ProjectileType.Darts:
                                CreateDart(RayCasting(targets, layerMaskDarts));
                                break;
                            case ProjectileType.NoiseMaker:
                                CreateNoiseMaker();
                                break;
                            case ProjectileType.SmokeBomb:
                                CreateSmokeBomb();
                                break;
                        }
                    }

                }
                else
                {
                    transform.Find("ArcSource").gameObject.SetActive(false);
                    transform.Find("DartRenderer").gameObject.SetActive(false);
                    Cursor.visible = true;

                    if (!isGrappling)
                    {
                        GameObject temp = RayCasting("GrappleSpot", layerMaskGrappling);
                        if (temp && grapplingCount > 0)
                        {
                            grappleObject = temp;
                            gameObject.transform.Find("SFX").Find("GrapplingThrowSound").GetComponent<AudioSource>().Play();
                            grappling = Instantiate(transform.Find("Grappling").gameObject);
                            grappling.transform.position = transform.position + new Vector3(0, 1, 0);
                            grappling.SetActive(true);
                            Vector3 vector = grappleObject.transform.position - grappling.transform.position;
                            float angle = (Mathf.Atan2(vector.y, vector.x)) * Mathf.Rad2Deg;
                            angle -= 90;
                            grappling.transform.Rotate(transform.forward * angle);
                            grappling.GetComponent<Rigidbody>().velocity = grappling.transform.up * 20;
                            grapplingCount--;
                        }
                    }

                    if (dartsShot > 0)
                    {
                        dartsShot--;
                        gameObject.transform.Find("SFX").Find("DartSound").GetComponent<AudioSource>().Play();
                        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerThrowing"))
                            GetComponent<Animator>().Play("PlayerThrowing");

                    }
                }

                TimeFreeze();

                Grappling();
            }
            else
            {
                ProjectileChoice();

                if (Input.GetKey(KeyCode.LeftControl) && grappleObject == null)
                {
                    if (Camera.main.transform.position.z >= -20)
                        Camera.main.transform.position = Vector3.MoveTowards(Camera.main.transform.position, Camera.main.transform.position + new Vector3(0, 0, -5), .5f);

                    Cursor.visible = false;

                    ProjectileLines();

                    if (Input.GetMouseButtonUp(0))
                    {
                        switch (projectile)
                        {
                            case ProjectileType.Darts:
                                CreateDart(RayCasting(targets, layerMaskDarts));
                                break;
                            case ProjectileType.NoiseMaker:
                                CreateNoiseMaker();
                                break;
                            case ProjectileType.SmokeBomb:
                                CreateSmokeBomb();
                                break;
                        }
                    }

                }
                else
                {
                    transform.Find("ArcSource").gameObject.SetActive(false);
                    transform.Find("DartRenderer").gameObject.SetActive(false);
                    Cursor.visible = true;
                    if (dartsShot > 0)
                    {
                        dartsShot--;
                        gameObject.transform.Find("SFX").Find("DartSound").GetComponent<AudioSource>().Play();
                        if (!GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("PlayerThrowing"))
                            GetComponent<Animator>().Play("PlayerThrowing");

                    }
                }
            }
        }
    }

    #region Ability functions

    void TimeFreeze()
    {
        if (Input.GetKey(KeyCode.LeftControl) && grappleObject == null)
        {
            if (freezeTimeWait > 0)
                freezeTimeWait -= Time.deltaTime;
            else
                Time.timeScale = 0;
        }
        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            freezeTimeWait = .1f;
            Time.timeScale = 1;
        }
    }

    void Sprint()
    {
        GetComponent<Animator>().SetBool("Sprinting", true);
        if (Input.GetKey(KeyCode.LeftShift) && controller.velocity.magnitude > 0.1f)
        {
            if (dropObject)
                StartCoroutine("Sprinting");

            speed = 9;
        }
        else if (jumpCount != 0)
            speed = 9;
        else
        {
            GetComponent<Animator>().SetBool("Sprinting", false);
            speed = 5;
        }
    }

    void CreateDart(GameObject hit)
    {
        if (hit && dartCount > 0)
        {
            transform.Find("SFX").Find("GadgetUseSound").GetComponent<AudioSource>().Play();
            dart = Instantiate(transform.Find("Dart").gameObject);

            dart.transform.position = controller.transform.position + new Vector3(0, 1, 0);
            dart.SetActive(true);

            // rotation to aim darts
            Vector3 vector = mouse - dart.transform.position;
            float angle = (Mathf.Atan2(vector.y, vector.x)) * Mathf.Rad2Deg;
            angle -= 90;
            dart.transform.Rotate(transform.forward * angle);

            dart.GetComponent<Rigidbody>().velocity = dart.transform.up * 20;
            dartCount--;

            dartsShot++;
        }
    }

    void CreateNoiseMaker()
    {
        if (noiseMakerCount > 0)
        {
            transform.Find("SFX").Find("GadgetUseSound").GetComponent<AudioSource>().Play();
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
            mouse.z = controller.transform.position.z;

            noiseMaker = Instantiate(transform.Find("NoiseMaker").gameObject);
            noiseMaker.transform.position = controller.transform.position + new Vector3(0, 1, 0);

            noiseMaker.SetActive(true);

            noiseMaker.GetComponent<Rigidbody>().velocity = (mouse - noiseMaker.transform.position);
            noiseMakerCount--;

        }
    }

    void CreateSmokeBomb()
    {
        if (smokeBombCount > 0)
        {
            transform.Find("SFX").Find("GadgetUseSound").GetComponent<AudioSource>().Play();

            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
            mouse.z = controller.transform.position.z;

            smokeBomb = Instantiate(transform.Find("SmokeBomb").gameObject);
            smokeBomb.transform.position = controller.transform.position + new Vector3(0, 1, 0);
            smokeBomb.SetActive(true);

            smokeBomb.GetComponent<Rigidbody>().velocity = (mouse - smokeBomb.transform.position);
            smokeBombCount--;
        }
    }

    void DropBox()
    {
        if (Input.GetKeyDown(KeyCode.B) && jumpCount == 0 && boxCount > 0)
        {
            transform.Find("SFX").Find("GadgetUseSound").GetComponent<AudioSource>().Play();

            box = Instantiate(transform.Find("Box").gameObject);
            box.transform.position = transform.position -= new Vector3(0, .4f, 0);
            box.SetActive(true);
            box.GetComponent<Animator>().SetTrigger("Spawn");
            boxCount--;
        }
    }

    void DropTrap()
    {
        if (Input.GetKeyDown(KeyCode.T) && trapCount > 0)
        {
            transform.Find("SFX").Find("GadgetUseSound").GetComponent<AudioSource>().Play();

            trap = Instantiate(transform.Find("Trap").gameObject);
            if (GetComponent<SpriteRenderer>().flipX)
                trap.transform.position = transform.Find("Trap").transform.position + new Vector3(1.5f, 0, 0);
            else
                trap.transform.position = transform.Find("Trap").transform.position + new Vector3(-1.5f, 0, 0);


            trap.SetActive(true);
            trapCount--;
        }
    }

    void KillGuard()
    {
        if (nearGuardHitBox)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                FindObjectOfType<GameManagerController>().KillEffect.transform.position = guard.transform.position;
                FindObjectOfType<GameManagerController>().KillEffect.Play(true);
                GetComponent<Animator>().Play("PlayerAttacking");
                guard.transform.parent.GetComponent<GuardScript>().isPlayerInSight = false;
                guard.transform.parent.GetComponent<GuardScript>().KillGuard();


                transform.Find("SFX").Find("BackStabbingSound").GetComponent<AudioSource>().pitch = UnityEngine.Random.Range(.8f, 1.2f);
                transform.Find("SFX").Find("BackStabbingSound").GetComponent<AudioSource>().Play();
                nearGuardHitBox = false;
                guard = null;
            }
        }
    }

    void Grappling()
    {
        if (isGrappling)
        {
            if (grappleObject.transform.position.x > transform.position.x)
                GetComponent<SpriteRenderer>().flipX = true;
            else
                GetComponent<SpriteRenderer>().flipX = false;

            moveDirection = Vector3.zero;
            if (Vector3.Distance(controller.transform.position, (grappleObject.transform.position + new Vector3(0, 2, 0))) > grappleObject.GetComponent<BoxCollider>().bounds.size.x)
            {
                gravity = 0;
                controller.transform.position = Vector3.MoveTowards(controller.transform.position, (grappleObject.transform.position + new Vector3(0, 2f, 0)), 25 * Time.deltaTime);
            }
            else
            {
                gravity = 35;
                grappleObject.GetComponent<BoxCollider>().enabled = true;
                isGrappling = false;
                grappleObject = null;
                controller.detectCollisions = true;
            }
        }
    }

    void ProjectileChoice()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            projectile = ProjectileType.Darts;

        if (Input.GetKeyDown(KeyCode.Alpha2))
            projectile = ProjectileType.NoiseMaker;

        if (Input.GetKeyDown(KeyCode.Alpha3))
            projectile = ProjectileType.SmokeBomb;
    }

    void Hide()
    {
        if (Input.GetKeyDown(KeyCode.E) && nearHidingSpot && !isChased)
        {

            isHidden = !isHidden;

            if (isHidden)
            {
                gameManager.GetComponent<GameManagerController>().ShowHiddenOutline();

                spriteRenderer.sortingOrder = 0;
                transform.position = new Vector3(hidingSpotX, transform.position.y, transform.position.z + 1);
                moveDirection = Vector2.zero;
            }
            else
            {
                gameManager.GetComponent<GameManagerController>().HideHiddenOutline();

                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
                spriteRenderer.sortingOrder = 10;
                if (breakableHidingSpot)
                {
                    breakableHidingSpot.GetComponent<BoxCollider>().enabled = false;
                    Destroy(breakableHidingSpot.transform.Find("Command").gameObject);
                    breakableHidingSpot.GetComponent<Animator>().SetTrigger("Destroy");
                    nearHidingSpot = false;
                    breakableHidingSpot = null;
                }
            }
        }
    }

    #endregion

    #region HelperFunctions

    void UpdateAnimations()
    {
        if (playerHP <= 0)//player dead
        {
            GetComponent<CharacterController>().enabled = false;
            grappleObject.GetComponent<BoxCollider>().enabled = true;
            transform.Find("SFX").Find("StunnedSound").GetComponent<AudioSource>().mute = true;
            playerAnim.SetInteger("HP", playerHP);
            playerAnim.SetFloat("Speed", 0);
            playerAnim.SetBool("Grappling", false);
            playerAnim.SetBool("Sprinting", false);
            playerAnim.SetBool("isClimbing", false);
            playerAnim.SetBool("Stunned", false);
            GetComponent<SpriteRenderer>().color = new Color(.2f, .2f, .2f);
            gameManager.GetComponent<GameManagerController>().EnableDeathMenu();
            return;
        }
        else // player alive
        {
            if (inSpotlight)
                GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
            else
                GetComponent<SpriteRenderer>().color = new Color(.25f, .25f, .25f);

            if (isClimbing)
                playerAnim.SetFloat("Speed", Mathf.Abs(controller.velocity.y));
            else
                playerAnim.SetFloat("Speed", Mathf.Abs(controller.velocity.x));

            if (((controller.velocity.y < -.7f && jumpCount != 0) || controller.velocity.y < -10) && !isClimbing && !isGrappling)
                playerAnim.Play("Falling");

            playerAnim.SetBool("Stunned", isStunned);
            playerAnim.SetBool("Grappling", isGrappling);
            playerAnim.SetBool("isClimbing", isClimbing);
            playerAnim.SetInteger("HP", playerHP);
        }
    }

    void FlipPlayerSprite()
    {
        if (Input.GetAxis("Horizontal") > 0)
        {
            transform.GetComponent<SpriteRenderer>().flipX = true;
            sprintDust.GetComponent<SpriteRenderer>().flipX = false;
            sprintDust.transform.position = transform.position + new Vector3(-.7f, -.8f, 0);

        }
        else if (Input.GetAxis("Horizontal") < 0)
        {
            transform.GetComponent<SpriteRenderer>().flipX = false;
            sprintDust.GetComponent<SpriteRenderer>().flipX = true;
            sprintDust.transform.position = transform.position + new Vector3(.7f, -.8f, 0);
        }

    }

    void ProjectileLines()
    {

        transform.Find("ArcSource").gameObject.SetActive(false);
        transform.Find("DartRenderer").gameObject.SetActive(false);


        switch (projectile)
        {
            case ProjectileType.Darts:
                mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
                mouse.z = controller.transform.position.z;

                dartLineRenderer[1] = mouse;
                dartLineRenderer[0] = transform.position + new Vector3(0, 1, 0);
                transform.Find("DartRenderer").GetComponent<LineRenderer>().positionCount = 2;
                transform.Find("DartRenderer").GetComponent<LineRenderer>().SetPositions(dartLineRenderer);
                transform.Find("DartRenderer").Find("Cursor").transform.position = mouse;
                transform.Find("DartRenderer").gameObject.SetActive(true);
                break;
            case ProjectileType.NoiseMaker:
                if (noiseMakerCount > 0)
                {
                    transform.Find("ArcSource").gameObject.SetActive(true);
                    transform.Find("ArcSource").Find("Cursor").transform.position = transform.Find("ArcSource").GetComponent<LineRenderer>().GetPosition(10);
                    transform.Find("ArcSource").Find("Cursor").GetComponent<SpriteRenderer>().sprite = transform.Find("NoiseMaker").Find("GameObject").Find("Ring").GetComponent<SpriteRenderer>().sprite;
                    transform.Find("ArcSource").Find("Cursor").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1);
                }
                break;
            case ProjectileType.SmokeBomb:
                if (smokeBombCount > 0)
                {
                    transform.Find("ArcSource").gameObject.SetActive(true);
                    transform.Find("ArcSource").Find("Cursor").transform.position = transform.Find("ArcSource").GetComponent<LineRenderer>().GetPosition(10);
                    transform.Find("ArcSource").Find("Cursor").GetComponent<SpriteRenderer>().sprite = transform.Find("NoiseMaker").Find("GameObject").Find("Ring").GetComponent<SpriteRenderer>().sprite;
                    transform.Find("ArcSource").Find("Cursor").GetComponent<SpriteRenderer>().color = new Color(0, .5f, 0);
                }
                break;
        }
    }

    GameObject RayCasting(string targetTag, int LayerMask) // raycast
    {
        if (Input.GetMouseButtonDown(0))
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
            mouse.z = controller.transform.position.z;

            RaycastHit Hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), mouse - (transform.position + new Vector3(0, 1, 0)), out Hit, 1000, LayerMask))
            {
                if (Hit.transform.tag == targetTag && Vector3.Distance(mouse, Hit.point) < 1 && Vector3.Distance(transform.position, Hit.transform.position) > 3)
                {
                    return Hit.transform.gameObject;
                }

            }
        }
        return null;
    }

    GameObject RayCasting(string[] targetTag, int LayerMask) // raycast override for darts
    {
        if (Input.GetMouseButtonUp(0))
        {
            mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition + new Vector3(0, 0, Camera.main.transform.position.z * -1));
            mouse.z = controller.transform.position.z;

            RaycastHit Hit;
            if (Physics.Raycast(transform.position + new Vector3(0, 1, 0), mouse - (transform.position + new Vector3(0, 1, 0)), out Hit, 10000, LayerMask))
            {
                for (int i = 0; i < targetTag.Length; i++)
                    if (Hit.transform.tag == targetTag[i] && Vector3.Distance(mouse, Hit.point) < 1.25f)
                        return Hit.transform.gameObject;
            }
        }
        return null;
    }

    IEnumerator Sprinting()
    {
        // sets up the sound and stopping the previous one
        dropObject = false;
        // checks for if the player can jump so they can sprint on the ground and on walls to create sound. Cannot create sound while hidden.
        if (jumpCount == 0 && !isHidden)
        {
            sprint.transform.parent.gameObject.SetActive(true);
            // checking distance between player position and sprint position to check if player is moving and sprinting.
            if (Vector3.Distance(sprint.transform.position, transform.position) > 0.00001f && !anim.GetCurrentAnimatorStateInfo(0).IsName("Noise Animation"))
            {
                gameObject.transform.Find("SFX").Find("FootStepNoise").GetComponent<AudioSource>().Play();
                if (!isClimbing)
                    sprintDust.SetActive(true);
                anim.Play("Noise Animation");
            }

            yield return new WaitForSeconds(.25f);
            if (!isClimbing)
                sprintDust.SetActive(false);

            yield return new WaitForSeconds(.25f);

            sprint.transform.parent.gameObject.SetActive(false);
        }
        dropObject = true;
    }

    public void AddImpact(Vector3 direction, float force) // call this function to add an impact force:
    {
        direction = new Vector3(direction.x, 0, 0);
        impact += direction.normalized * force / mass;
    }

    public void DamagePlayerHP(short _dmg)
    {
        cameraLogic.Shake(.2f, .2f);
        transform.Find("SFX").Find("HeartbeatSound").GetComponent<AudioSource>().Play();
        playerHP -= _dmg;
        if (playerHP < 0)
            playerHP = 0;

        gameManager.GetComponent<GameManagerController>().ShowRedOutline();

    }

    public void NotNearGuard()
    {
        nearGuardHitBox = false;
        guard = null;
    }

    #endregion

    #region Getters
    public ushort GetPlayerScore()
    {
        return playerScore;
    }

    public short GetPlayerHP()
    {
        return playerHP;
    }
    #endregion

    #region Setters

    public void SetPlayerHP(short _newHealth)
    {
        playerHP = _newHealth;
    }

    public void SetPlayerScore(ushort _newScore)
    {
        playerScore += _newScore;
    }
    #endregion

    #region Collisions
    void OnCollisionEnter(Collision collision)
    {
        if (controller.collisionFlags == CollisionFlags.Below && collision.gameObject.layer == 17)
        {
            Debug.Log(collision.gameObject.layer);
        }

        if (collision.transform.tag == "Wall")
            nearClimbingSpot = true;

        if (collision.transform.tag == "FallingObject")
        {
            if (collision.transform.GetComponent<FallingObjectScript>().isFalling)
                DamagePlayerHP(4);
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.transform.tag == "Wall")
            nearClimbingSpot = false;


    }
    #endregion

    #region Triggers
    void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Collectable")//Collectables
        {
            playerScore++;
            collider.gameObject.SetActive(false);
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Simulate(0f, true, true);
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Play();
        }

        if (collider.tag == "SmokeCLoud")//Smoke Cloud
        {
            isHidden = true;
        }

        if (collider.tag == "HealthChest") //Chests
        {
            collider.GetComponent<Animator>().Play("ChestoPening");
            playerHP = 4;
            collider.GetComponent<BoxCollider>().enabled = false;
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Simulate(0f, true, true);
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Play();
        }

        if (collider.tag == "SmokeChest")
        {
            collider.GetComponent<Animator>().Play("SmokeChestOpening");
            smokeBombCount++;
            collider.GetComponent<BoxCollider>().enabled = false;
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Simulate(0f, true, true);
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Play();
        }

        if (collider.tag == "NoiseMakerChest")
        {
            collider.GetComponent<Animator>().Play("NoiseMakerChestOpening");
            noiseMakerCount++;
            collider.GetComponent<BoxCollider>().enabled = false;
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Simulate(0f, true, true);
            transform.Find("PickUpEffect").GetComponent<ParticleSystem>().Play();
        }

        if (collider.tag == "Checkpoint")//Checkpoint
        {
            collider.GetComponent<Animator>().Play("CheckpointReached");
            gameManager.GetComponent<GameManagerController>().checkpointReached = true;
            gameManager.GetComponent<GameManagerController>().respawnPoint = new Vector3(collider.transform.position.x, collider.transform.position.y, transform.position.z);
        }

        if (collider.tag == "HidingSpot")//Hiding
        {
            nearHidingSpot = true;
            hidingSpotX = collider.transform.position.x;
            collider.transform.Find("Command").gameObject.GetComponent<SpriteRenderer>().sprite = EKey;
        }

        if (collider.tag == "BreakableHidingSpot")// Breakable hiding
        {
            nearHidingSpot = true;
            hidingSpotX = collider.transform.position.x;
            collider.transform.Find("Command").gameObject.GetComponent<SpriteRenderer>().sprite = EKey;
            breakableHidingSpot = collider.gameObject;
        }

        if (collider.tag == "Wall")//Climbing
        {
            noMoreBunnyHops = 0;
            if (collider.transform.position.x > transform.position.x)
                GetComponent<SpriteRenderer>().flipX = true;
            else
                GetComponent<SpriteRenderer>().flipX = false;

            nearClimbingSpot = true;
            climbingBarrier = (collider.bounds.size.y * .5f) + (collider.transform.position.y);
            if (jumpCount == 1 && !isClimbing)
            {
                isClimbing = true;
                jumpCount = 0;
            }

        }

        if (collider.tag == "HitBox")//Kill guard
        {
            nearGuardHitBox = true;
            guard = collider.gameObject;
            guard.transform.parent.transform.Find("Command").gameObject.GetComponent<SpriteRenderer>().sprite = FKey;
            guard.transform.parent.Find("RedOutline").gameObject.SetActive(true);
        }

        if (collider.tag == "Spikes") //Spikes
        {
            DamagePlayerHP(4);
        }

        if (collider.transform.tag == "Finish")
            SceneManager.LoadScene("EndGameScene");
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag == "Wall")
            if (moveDirection.y < 0)
            {
                isClimbing = true;
                jumpCount = 0;
            }



        //Spotlight
        if (other.tag == "Spotlight")
            inSpotLightTimer = .03f;

    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Spotlight")
        {
            inSpotlight = false;
        }

        if (collider.tag == "SmokeCLoud")//Smoke Cloud
        {
            isHidden = false;
        }

        if (collider.tag == "HidingSpot")
        {
            nearHidingSpot = false;
            collider.transform.Find("Command").gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }

        if (collider.tag == "Wall")
        {
            nearClimbingSpot = false;
            isClimbing = false;
            jumpCount = 1;
        }

        if (collider.tag == "HitBox")
        {
            guard.transform.parent.Find("RedOutline").gameObject.SetActive(false);
            nearGuardHitBox = false;
            guard = null;
            collider.transform.parent.Find("Command").gameObject.GetComponent<SpriteRenderer>().sprite = null;
        }

        if (collider.tag == "BreakableHidingSpot") // Breakable hiding
        {
            nearHidingSpot = false;
            collider.transform.Find("Command").gameObject.GetComponent<SpriteRenderer>().sprite = null;
            breakableHidingSpot = null;
        }
    }
    #endregion

}