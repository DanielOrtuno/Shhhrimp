using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveBurner : MonoBehaviour {
    static public bool stoveIsOn;
    bool playAnim;
    Animator anim;
    Animator bossAnim;

	// Use this for initialization
	void Start () {
        playAnim = true;
        stoveIsOn = false;
        anim = gameObject.GetComponent<Animator>();
        bossAnim = GameObject.Find("Boss").transform.Find("BossLineOfSight").transform.Find("BossBody").GetComponent<Animator>();
	}
	
	// Update is called once per frame
    private void FixedUpdate()
    {
        if(playAnim)
        {
            playAnim = false;
            StartCoroutine(StoveBurning());
        }

    }

    IEnumerator StoveBurning()
    {
        yield return new WaitForSeconds(5.0f);
        anim.SetBool("stoveIsOff", false);
        anim.SetBool("stoveIsOn", true);
        stoveIsOn = true;      
        yield return new WaitForSeconds(8.0f);
        anim.SetBool("stoveIsOff", true);
        anim.SetBool("stoveIsOn", false);
        stoveIsOn = false;       
        playAnim = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FinalBoss" && stoveIsOn == true)
        {
            other.transform.parent.GetComponent<FinalBossStage2>().DamageBossHP(1);
            bossAnim.SetTrigger("TakeDamage");
            StopAllCoroutines();
            anim.SetBool("stoveIsOff", true);
            playAnim = false;
        }
           
    }

}
