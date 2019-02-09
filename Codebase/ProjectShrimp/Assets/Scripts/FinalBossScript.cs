using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FinalBossScript : MonoBehaviour {

    public float weaponMovementSpeed;

    public ushort bossHealth;
    public bool bossIsDead;

    GameObject bossWeapon;
    

    Vector3 targetAngle;
    Vector3 currentAngle;
    Vector3 originAngle;



	// Use this for initialization
	void Start () {
        bossWeapon = gameObject.transform.Find("BossWeapon").gameObject;
        targetAngle = new Vector3(-90, 0, 0);
        originAngle = new Vector3(0,0,0);
        currentAngle = originAngle;
        bossIsDead = false;
        bossHealth = 2;
    }
	
	// Update is called once per frame
	void Update () {
        if (bossHealth <= 0)
            bossIsDead = true;

        if (!bossIsDead)
        {
            if (BossVisionScript.isSpotted)
            {
                StartCoroutine(WeaponSlamming());
            }
        }
        if (bossIsDead)
        {
            StartCoroutine(LoadVictoryScene());
        }
		
	}

    private IEnumerator WeaponSlamming()
    {
        // Moving weapon to spot
        bossWeapon.transform.position = Vector3.Lerp(bossWeapon.transform.position, BossVisionScript.playerXPosition, Mathf.PingPong(Time.deltaTime * weaponMovementSpeed, 1.0f));

        yield return new WaitForSeconds(1.5f);
        // Weapon slamming down
        currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, targetAngle.x, Time.deltaTime * 10), 0, 0);
        bossWeapon.transform.eulerAngles = currentAngle;
        
        yield return new WaitForSeconds(1.5f);

        // Pulling the weapon back up 
        currentAngle = new Vector3(Mathf.LerpAngle(currentAngle.x, originAngle.x, Time.time), 0, 0);
        bossWeapon.transform.eulerAngles = currentAngle;

        yield return new WaitForSeconds(1.0f);

        // Enabling the boss line of sight
        BossVisionScript.isSpotted = false;
        gameObject.transform.Find("BossLineOfSight").Find("BossVision").gameObject.SetActive(true);
        yield return new WaitForSeconds(5.0f);

        gameObject.transform.Find("BossLineOfSight").GetComponent<CapsuleCollider>().enabled = true;
    }
    IEnumerator LoadVictoryScene()
    {
        yield return new WaitForSeconds(3.0f);
        SceneManager.LoadScene("BossStage2");
    }

    public void DamageBossHP(ushort _dmg)
    {
        bossHealth -= _dmg;
        if (bossHealth > 2)
            bossHealth = 0;
    }
}
