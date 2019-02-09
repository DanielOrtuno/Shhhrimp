using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossStage2 : MonoBehaviour {

    public float weaponMovementSpeed;
    public ushort bossHealth;
    public bool bossIsDead;

    GameObject bossWeapon;

    Vector3 currentPos;
    Vector3 targetPos;
    Vector3 originPos;


	// Use this for initialization
	void Start () {
        bossWeapon = gameObject.transform.Find("BossWeapon").gameObject;
        bossHealth = 4;
        bossIsDead = false;
        targetPos = Vector3.down * 7;
    }
	
	// Update is called once per frame
	void Update () {
        if (bossHealth == 0)
            bossIsDead = true;

        if (!bossIsDead)
        {
            if (BossVisionScript2.isSpotted)
            {
                StartCoroutine(FistSlamming());
            }
        }
        else if (bossIsDead)
        {
            gameObject.GetComponent<Rigidbody>().useGravity = true;
            StartCoroutine(LoadVictoryScene());
        }
	}

   

    IEnumerator FistSlamming()
    {
        // Moving hand to spot
        originPos = Vector3.Lerp(bossWeapon.transform.position, BossVisionScript2.playerXPosition, Mathf.PingPong(Time.deltaTime * weaponMovementSpeed, 1.0f));
        bossWeapon.transform.position = originPos;

        yield return new WaitForSeconds(.8f);
        // Hand slamming down
        currentPos = Vector3.Lerp(bossWeapon.transform.position, originPos + targetPos, Mathf.PingPong(Time.deltaTime * weaponMovementSpeed, 1.0f));
        bossWeapon.transform.position = currentPos;
        yield return new WaitForSeconds(1.5f);
        // Check to see if hand hitted stove
        
        // Lifting the hand back up 
        originPos = Vector3.Lerp(currentPos, currentPos + Vector3.up * 7, Mathf.PingPong(Time.deltaTime * weaponMovementSpeed, 1.0f));
        bossWeapon.transform.position = originPos;

        yield return new WaitForSeconds(1.0f);

        // Enabling the boss line of sight
        BossVisionScript2.isSpotted = false;        
        gameObject.transform.Find("BossLineOfSight").Find("BossVision").gameObject.SetActive(true);
        yield return new WaitForSeconds(4.0f);

        gameObject.transform.Find("BossLineOfSight").GetComponent<CapsuleCollider>().enabled = true;
    }
    IEnumerator LoadVictoryScene()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("EndGameScene");
    }

    public void DamageBossHP(ushort _dmg)
    {
        bossHealth -= _dmg;
        if (bossHealth > 4)
            bossHealth = 0;
    }


}
