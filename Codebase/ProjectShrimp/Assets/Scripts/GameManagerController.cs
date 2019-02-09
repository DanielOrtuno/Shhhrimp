using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManagerController : MonoBehaviour
{

    GameObject Player;
    public AudioSource SFX;
    Slider Sfx;
    public AudioSource Music;
    Slider music;
    public bool checkpointReached = false;
    public Vector3 respawnPoint;

    GameObject deathMenu;
    GameObject pauseMenu;
    GameObject optionsMenu;
    GameObject playMenu;
    GameObject HUD;
    GameObject dartUI;
    GameObject noisemakerUI;
    GameObject smokebombUI;
    GameObject playerIcon;

    bool isOptionsOpen = false;
    bool isGamePaused = false;
    bool isPlayMenuOpen = false;

    GameObject screenBackground;
    GameObject screenOutline;
    GameObject audioTab;
    GameObject videoTab;
    GameObject controlsTab;
    Slider masterVol;
    public ParticleSystem KillEffect;
    public ParticleSystem NoiseMakerEffect;
    public ParticleSystem SmokeBombEffect;
    public BoxCollider Smoke;

    public Sprite playerIconIdle;
    public Sprite playerIconHidden;
    public Sprite playerIconChase;

    public Sprite redOutline;
    public Sprite blueOutline;
    public Sprite blackOutline;

    //[SerializeField]
    //GameObject hudCanvas;


    // Use this for initialization
    public void Start()
    {
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByBuildIndex(0))
        {
            Player = FindObjectOfType<PlayerController>().gameObject;
            respawnPoint = Player.transform.position;
            SFX = Player.transform.Find("SFX").Find("DartSound").GetComponent<AudioSource>();

            // menus not in Main Menu
            deathMenu = GameObject.Find("Death").transform.gameObject;
            pauseMenu = GameObject.Find("PauseMenu").gameObject;
            HUD = GameObject.Find("Canvas");

            deathMenu.SetActive(false);
            pauseMenu.SetActive(false);

            // Background music
            Music = GameObject.Find("BackgroundMusic").GetComponent<AudioSource>();

            // From HUD
            dartUI = HUD.transform.Find("DartUI").gameObject;
            noisemakerUI = HUD.transform.Find("NoisemakerUI").gameObject;
            smokebombUI = HUD.transform.Find("SmokeUI").gameObject;
            screenBackground = HUD.transform.Find("ScreenBackground").gameObject;
            screenOutline = HUD.transform.Find("ScreenOutline").gameObject;
            playerIcon = HUD.transform.Find("WinScreen").Find("HUD").Find("PlayerIcon").gameObject;

            // Particle systems
            KillEffect = GameObject.Find("KillEffect").GetComponent<ParticleSystem>();
            NoiseMakerEffect = GameObject.Find("Impact_Hit").GetComponent<ParticleSystem>();
            SmokeBombEffect = GameObject.Find("SmokeBombEffect").GetComponent<ParticleSystem>();
            Smoke = SmokeBombEffect.transform.Find("SmokeCollider").GetComponent<BoxCollider>();
        }
        // Menus
        optionsMenu = GameObject.Find("OptionsMenu").gameObject;
        playMenu = GameObject.Find("PlayMenu").gameObject;

        optionsMenu.SetActive(false);
        playMenu.SetActive(false);

        // From options menu
        videoTab = optionsMenu.transform.Find("Panel").Find("VideoTab").Find("VideoSection").gameObject;
        audioTab = optionsMenu.transform.Find("Panel").Find("AudioTab").Find("AudioSection").gameObject;
        controlsTab = optionsMenu.transform.Find("Panel").Find("ControlsTab").Find("ControlsSection").gameObject;
        masterVol = audioTab.transform.Find("MasterVol").Find("Slider").GetComponent<Slider>();
        music = audioTab.transform.Find("Music").Find("Slider").GetComponent<Slider>();
        Sfx = audioTab.transform.Find("SFX").Find("Slider").GetComponent<Slider>();

    }

    // Update is called once per frame
    public void Update()
    {
        if (Player)
        {
            if (Player.GetComponent<PlayerController>().GetPlayerHP() != 0)
                if (Input.GetKey(KeyCode.LeftControl))
                {
                    GameObject[] Lights = GameObject.FindGameObjectsWithTag("BreakableLight");
                    int objectCount = Lights.Length;

                    for (int i = 0; i < objectCount; i++)
                    {
                        Lights[i].transform.Find("GadgetPopUp").gameObject.SetActive(true);
                    }

                    GameObject[] FallingObjects = GameObject.FindGameObjectsWithTag("FallingObjectRope");
                    int objcount = FallingObjects.Length;

                    for (int i = 0; i < objcount; i++)
                    {
                        FallingObjects[i].transform.Find("GadgetPopUp").gameObject.SetActive(true);
                    }

                    GameObject[] Drones = GameObject.FindGameObjectsWithTag("DroneGuard");
                    int droneCount = Drones.Length;

                    for (int i = 0; i < droneCount; i++)
                    {
                        Drones[i].transform.Find("GadgetPopUp").gameObject.SetActive(true);
                    }

                }
                else if (Input.GetKeyUp(KeyCode.LeftControl))
                {
                    GameObject[] Lights = GameObject.FindGameObjectsWithTag("BreakableLight");
                    int objectCount = Lights.Length;

                    for (int i = 0; i < objectCount; i++)
                    {
                        Lights[i].transform.Find("GadgetPopUp").gameObject.SetActive(false);
                    }

                    GameObject[] FallingObjects = GameObject.FindGameObjectsWithTag("FallingObjectRope");
                    int objcount = FallingObjects.Length;

                    for (int i = 0; i < objcount; i++)
                    {
                        FallingObjects[i].transform.Find("GadgetPopUp").gameObject.SetActive(false);
                    }

                    GameObject[] Drones = GameObject.FindGameObjectsWithTag("DroneGuard");
                    int droneCount = Drones.Length;

                    for (int i = 0; i < droneCount; i++)
                    {
                        Drones[i].transform.Find("GadgetPopUp").gameObject.SetActive(false);
                    }
                }
        }

        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Main Menu"))
        {
            if (Player.GetComponent<PlayerController>().GetPlayerHP() == 0)
                EnableDeathMenu();

            switch (Player.GetComponent<PlayerController>().projectile)
            {
                case PlayerController.ProjectileType.Darts:
                    dartUI.SetActive(true);
                    noisemakerUI.SetActive(false);
                    smokebombUI.SetActive(false);
                    break;
                case PlayerController.ProjectileType.NoiseMaker:
                    dartUI.SetActive(false);
                    noisemakerUI.SetActive(true);
                    smokebombUI.SetActive(false);
                    break;
                case PlayerController.ProjectileType.SmokeBomb:
                    dartUI.SetActive(false);
                    noisemakerUI.SetActive(false);
                    smokebombUI.SetActive(true);
                    break;
            }

            if (Player.GetComponent<PlayerController>().isHidden)
            {
                playerIcon.GetComponent<Image>().sprite = playerIconHidden;
            }
            else if (Player.GetComponent<PlayerController>().isChased)
            {
                playerIcon.GetComponent<Image>().sprite = playerIconChase;
            }
            else
            {
                playerIcon.GetComponent<Image>().sprite = playerIconIdle;
            }


        }


        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isGamePaused)
            {
                Resume();
                optionsMenu.gameObject.SetActive(false);
            }
            else
            {
                Pause();
            }
        }

    }

    public void CloseOptions()
    {
        isOptionsOpen = false;
        optionsMenu.gameObject.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Main Menu");
    }

    public void Credits()
    {
        SceneManager.LoadScene("Credits");
    }

    public void PlayMenu()
    {

        if (isPlayMenuOpen)
        {
            playMenu.gameObject.SetActive(false);
            isPlayMenuOpen = false;
        }
        else
        {
            playMenu.gameObject.SetActive(true);
            optionsMenu.gameObject.SetActive(false);
            isPlayMenuOpen = true;

        }
    }

    public void LoadGame()
    {
        SceneManager.LoadScene("Boat_Level");
        Time.timeScale = 1;
    }

    public void ChangeToAudio()
    {
        if (isOptionsOpen)
        {
            audioTab.gameObject.SetActive(true);
            videoTab.gameObject.SetActive(false);
            controlsTab.gameObject.SetActive(false);
        }
    }

    public void MasterVolume()
    {

        AudioListener.volume = masterVol.value;
    }


    public void ChangeToVideo()
    {
        if (isOptionsOpen)
        {
            audioTab.gameObject.SetActive(false);
            videoTab.gameObject.SetActive(true);
            controlsTab.gameObject.SetActive(false);
        }
    }

    public void ChangeToControls()
    {
        if (isOptionsOpen)
        {
            audioTab.gameObject.SetActive(false);
            videoTab.gameObject.SetActive(false);
            controlsTab.gameObject.SetActive(true);
        }
    }

    public void LoadOptions()
    {
        Sfx.value = SFX.volume;
        music.value = Music.volume;
        masterVol.value = AudioListener.volume;
        if (isOptionsOpen)
        {
            optionsMenu.gameObject.SetActive(false);
            isOptionsOpen = false;
        }
        else
        {
            optionsMenu.gameObject.SetActive(true);
            SFX.volume = Sfx.value;
            Music.volume = music.value;
            AudioListener.volume = masterVol.value;
            playMenu.gameObject.SetActive(false);
            isOptionsOpen = true;

        }
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void Pause()
    {
        // Player.GetComponent<PlayerController>().timeFrozen = true;
        pauseMenu.gameObject.SetActive(true);
        Time.timeScale = 0f;
        isGamePaused = true;
        //Cursor.visible = true;
    }

    public void Resume()
    {
        //Player.GetComponent<PlayerController>().timeFrozen = false;
        Time.timeScale = 1f;
        optionsMenu.SetActive(false);
        pauseMenu.SetActive(false);
        //hudCanvas.SetActive(true);
        isGamePaused = false;
        // Cursor.visible = false;
    }

    public void EnableDeathMenu()
    {
        deathMenu.SetActive(true);
    }

    public void Respawn()
    {
        Resume();
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        //Player.GetComponent<PlayerController>().Load();
        Player.transform.position = respawnPoint;
        Player.GetComponent<PlayerController>().Start();
        Player.GetComponent<PlayerController>().isStunned = false;
        Player.gameObject.SetActive(true);
        deathMenu.gameObject.SetActive(false);
        Player.GetComponent<CharacterController>().enabled = true;
        GameObject.Find("Canvas").transform.Find("WinScreen").gameObject.SetActive(false);
    }

    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ShowRedOutline()
    {
        screenBackground.SetActive(false);
        screenBackground.SetActive(true);
        screenBackground.GetComponent<Animator>().Play("ScreenRedGlow");


        screenOutline.SetActive(false);
        screenOutline.SetActive(true);
        screenOutline.GetComponent<Image>().sprite = redOutline;
        screenOutline.GetComponent<Animator>().SetBool("Connected", true);
        screenOutline.GetComponent<Animator>().Play("ScreenOutlineOn");
    }

    public void ShowFreezeTimeOutline()
    {
        screenOutline.GetComponent<Image>().sprite = blueOutline;
        screenOutline.GetComponent<Animator>().SetBool("Connected", false);
        screenOutline.GetComponent<Animator>().Play("ScreenOutlineOn");
    }

    public void HideFreezeTimeOutline()
    {
        screenOutline.GetComponent<Animator>().SetBool("Connected", false);
        screenOutline.GetComponent<Animator>().Play("ScreenOutlineOff");
    }


    public void ShowHiddenOutline()
    {
        screenOutline.GetComponent<Image>().sprite = blackOutline;
        screenOutline.GetComponent<Animator>().SetBool("Connected", false);
        screenOutline.GetComponent<Animator>().Play("ScreenOutlineOn");
    }

    public void HideHiddenOutline()
    {
        screenOutline.GetComponent<Animator>().SetBool("Connected", false);
        screenOutline.GetComponent<Animator>().Play("ScreenOutlineOff");
    }

}
