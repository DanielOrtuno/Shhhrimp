using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpotterGuardScript : MonoBehaviour
{

    Animator animator;
    Animator spriteAnimator;
    bool hidden;
    public bool playAnimation;
    GuardGroupScript guardGroupScript;

    public bool isChilling;

    // Use this for initialization
    void Start()
    {

        isChilling = true;
        animator = GetComponent<Animator>();
        playAnimation = true;
        spriteAnimator = transform.parent.Find("SpotterGuardSprite").GetComponent<Animator>();

        guardGroupScript = transform.parent.parent.GetComponent<GuardGroupScript>();
    }

    // Update is called once per frame
    void Update()
    {
        if (playAnimation && isChilling)
        {
            StartCoroutine(DetectorEnum());
        }

    }

    IEnumerator DetectorEnum()
    {
        playAnimation = false;
        spriteAnimator.Play("SpotterGuardDetecting");
        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("SpotterDetector"))
            animator.Play("SpotterDetector");

        yield return new WaitForSeconds(2f);
        spriteAnimator.Play("SpotterGuardNoDetecting");

        yield return new WaitForSeconds(3f);
        playAnimation = true;


    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
        {
            if (other.GetComponent<CharacterController>().velocity.magnitude > 1)
            {
                guardGroupScript.AlertGroup();
                guardGroupScript.playerGhost.transform.position = GameObject.Find("Player").transform.position;
                guardGroupScript.playerGhost.SetActive(true);
            }
        }
    }

    public IEnumerator StunnedEnum()
    {
        StopCoroutine(DetectorEnum());
        isChilling = false;
        playAnimation = false;
        yield return new WaitForSeconds(3.1f);
        playAnimation = true;
        isChilling = true;
    }
}
