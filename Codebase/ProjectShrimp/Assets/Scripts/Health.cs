using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour {

    public Sprite[] HeartsArr;
    public Image Hearts;
    GameObject Player;

	// Use this for initialization
	void Start () {
        Cursor.visible = true;
        Player = FindObjectOfType<PlayerController>().gameObject;
	}
	
	// Update is called once per frame
	void Update () {
        Hearts.sprite = HeartsArr[Player.GetComponent<PlayerController>().GetPlayerHP()];
	}
}
