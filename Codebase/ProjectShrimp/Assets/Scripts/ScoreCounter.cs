using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreCounter : MonoBehaviour {

    public PlayerController player;
    public Text scoreText;
    public Text dartText;
    public Text noisemakerText;
    public Text smokebombText;
    void Start()
    {
        player = FindObjectOfType<PlayerController>();
    }
    void Update () {
        scoreText.text = player.GetPlayerScore().ToString();
        dartText.text = player.dartCount.ToString();
        noisemakerText.text = player.noiseMakerCount.ToString();
        smokebombText.text = player.smokeBombCount.ToString();
    }
}
