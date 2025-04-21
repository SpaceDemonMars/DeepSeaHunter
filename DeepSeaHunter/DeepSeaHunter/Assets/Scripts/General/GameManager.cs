using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Data;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Menus")]
    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuTab;
    [SerializeField] GameObject menuActiveTab;
    [SerializeField] TMP_Text menuTabText;
    [SerializeField] GameObject menuTabSettings;
    [SerializeField] GameObject menuTabInventory;
    [SerializeField] GameObject menuTabEquipment;
    [SerializeField] GameObject menuTabJournal;

    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    //inventory
    [SerializeField] GameObject[] invenButtons;
    public GameObject itemInfo;
    public TMP_Text itemInfoName;
    public TMP_Text itemInfoDesc;
    public TMP_Text itemInfoQty;
    public TMP_Text fishText;
    public TMP_Text scrapText;
    //lose
    public Image loseScreen;
    [SerializeField] TMP_Text loseYouDied;
    [SerializeField] TMP_Text loseMessage;
    [SerializeField] Color loseTextColorDamage;
    [SerializeField] Color loseTextColorO2;
    [SerializeField] Color loseTextColorTemp;
    Color[] loseTextColors;

    [Header("UI")]
    public GameObject checkpointPopup;
    [SerializeField] GameObject goalCountLabel;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject tutorialText;

    [Header("Dialogue")]
    public GameObject dialogHiddenUI;
    public TMP_Text interactPrompt;
    public DialogueManager dialogueManager;

    [Header("Menu SFX")]
    [SerializeField] AudioSource aud;
    [SerializeField] AudioClip[] uiSFX;
    [SerializeField] float uiVol;
    public GameObject pausePlayer;
    public PauseMusic pauseMusic;

    [Header("Player")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;
    public GameObject radio;
    public Radio radioScript;
    public oxygen o2;

    [Header("Player-UI")]
    //screens
    public GameObject playerDamageScreen;
    public GameObject playerHealScreen;
    public GameObject playerSlowScreen;
    //bars
    public Image playerHPBar;
    public Image playerO2Bar;
    public GameObject playerO2UI;
    public GameObject[] temperatureIcons;
    public TMP_Text damageText;
    //weapons
    public Image harpoonChargeBar;
    public Image harpoonReloadBar;
    public Image knifeReloadBar;
    //popups
    public GameObject weaponUpgradePopup;
    public TMP_Text weaponUpgradeText;

    [Header("Boss")]
    public GameObject boss;
    public EnemyBoss bossScript;
    public GameObject bossSpawner;
    public spawner spawnerScript;

    [Header("Boss-UI")]
    public GameObject bossHP;
    public TMP_Text bossBarText;
    public Image bossHPBar;
    public Image bossArmorBar;

    public bool isPaused;
    bool isTutorialLevel;
    //public int goalCount;

    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
        radio = GameObject.FindWithTag("Radio");
        pausePlayer = GameObject.FindWithTag("PauseMusic");
        o2 = player.GetComponent<oxygen>();
        radioScript = radio.GetComponent<Radio>();
        pauseMusic = pausePlayer.GetComponent<PauseMusic>();
        bossSpawner = GameObject.FindWithTag("Boss Spawner");
        if (bossSpawner != null)
        {
            spawnerScript = bossSpawner.GetComponent<spawner>();
            resetLevelBoss(); //spawns boss
        }
        playerSpawnPos = GameObject.FindWithTag("Player Spawn Pos");
        isTutorialLevel = (GameObject.FindWithTag("Tutorial") != null) ? true : false;
        goalCountLabel.SetActive(!isTutorialLevel);
        tutorialText.SetActive(isTutorialLevel);

        loseTextColors = new Color[3];
        loseTextColors[0] = loseTextColorDamage;
        loseTextColors[1] = loseTextColorO2;
        loseTextColors[2] = loseTextColorTemp;
    }

    // Update is called once per frame
    void Update()
    {
        openTabSettings();
        openTabInventory();
        openTabEquipment();
        openTabJournal();
    }

    public void openTabSettings(bool buttonClick = false)
    {
        if (Input.GetButtonDown("Cancel") || buttonClick == true)
        {
            if (!isPaused)
            {
                statePause();
                menuActive = menuTab;
                menuActiveTab = menuTabSettings;
                menuTabText.text = "Settings";
                menuActive.SetActive(true);
                menuActiveTab.SetActive(true);
            }
            else if (isPaused && menuActive == menuTab && 
                menuActiveTab == menuTabSettings && buttonClick == false)
            {
                stateUnpause();
            }
            else if (isPaused && menuActive == menuTab) //other tab menu
            {
                menuActiveTab.SetActive(false);
                menuActiveTab = menuTabSettings;
                menuTabText.text = "Settings";
                menuActiveTab.SetActive(true);
            }
        }
    }
    public void openTabInventory(bool buttonClick = false)
    {
        if (Input.GetButtonDown("Inventory") || buttonClick == true)
        {
            loadInventory();
            if (!isPaused)
            {
                statePause();
                menuActive = menuTab;
                menuActiveTab = menuTabInventory;
                menuTabText.text = "Inventory";
                menuActive.SetActive(true);
                menuActiveTab.SetActive(true);
            }
            else if (isPaused && menuActive == menuTab &&
                menuActiveTab == menuTabInventory && buttonClick == false)
            {
                stateUnpause();
            }
            else if (isPaused && menuActive == menuTab) //other tab menu
            {
                menuActiveTab.SetActive(false);
                menuActiveTab = menuTabInventory;
                menuTabText.text = "Inventory";
                menuActiveTab.SetActive(true);
            }
        }
    }
    public void openTabEquipment(bool buttonClick = false)
    {
        if (Input.GetButtonDown("Equipment") || buttonClick == true)
        {
            if (!isPaused) //no menu
            {
                statePause();
                menuActive = menuTab;
                menuActiveTab = menuTabEquipment;
                menuTabText.text = "Equipment";
                menuActive.SetActive(true);
                menuActiveTab.SetActive(true);
            }
            else if (isPaused && menuActive == menuTab &&
                menuActiveTab == menuTabEquipment && buttonClick == false)
            {
                stateUnpause();
            }
            else if (isPaused && menuActive == menuTab) //other tab menu
            {
                menuActiveTab.SetActive(false);
                menuActiveTab = menuTabEquipment;
                menuTabText.text = "Equipment";
                menuActiveTab.SetActive(true);
            }
        }
    }

    public void openTabJournal(bool buttonClick = false)
    {
        if (Input.GetButtonDown("Journal") || buttonClick == true)
        {
            if (!isPaused) //no menu
            {
                statePause();
                menuActive = menuTab;
                menuActiveTab = menuTabJournal;
                menuTabText.text = "Journal";
                menuActive.SetActive(true);
                menuActiveTab.SetActive(true);
            }
            else if (isPaused && menuActive == menuTab &&
                menuActiveTab == menuTabJournal && buttonClick == false)
            {
                stateUnpause();
            }
            else if (isPaused && menuActive == menuTab) //other tab menu
            {
                menuActiveTab.SetActive(false);
                menuActiveTab = menuTabJournal;
                menuTabText.text = "Journal";
                menuActiveTab.SetActive(true);
            }
        }
    }
    public void loadInventory()
    {
        for (int i = 0; i < invenButtons.Length; i++)
        {
            if(i < playerScript.inven.getInvenSize())
            {
                inventoryButtons tempScript = invenButtons[i].GetComponent<inventoryButtons>();
                if (tempScript != null)
                {
                    tempScript.item = playerScript.inven.getItem(i);
                    tempScript.setText();
                }
                invenButtons[i].SetActive(true);
            }
            else
                invenButtons[i].SetActive(false);
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        pauseMusic.togglePauseMusic(isPaused);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        radioScript.aud.Pause();
        Time.timeScale = 0;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        pauseMusic.togglePauseMusic(!radioScript.IsRadioOn());
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        radioScript.aud.UnPause();
        Time.timeScale = 1;
        menuActive.SetActive(false);
        menuActive = null;
        if (menuActiveTab != null) {
            menuActiveTab.SetActive(false);
            menuActiveTab = null;
        }
    }

    public void updateGameGoal(string bossName, bool slain)
    {
        goalCountText.text = bossName;

        if (slain)
        {
            statePause();
            menuActive = menuWin;
            menuActive.SetActive(true);
        }
    }

    public void youLose(int source)
    {
        statePause();
        dialogHiddenUI.SetActive(false); //hide UI
        //set death info
        loseYouDied.color = loseTextColors[source];
        loseMessage.color = loseTextColors[source];
        //once we decide how we're implementing the death messages
        //make an array for each source
        //create array of arrays so we can index by [source][message]
        //call line below vv (or similar line)
        //loseMessage = loseMessages[source][Random.Range(0, loseMessages[source].Length) - 1];
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    //

    public void resetLevelBoss()
    {
        //turn off boss hp UI here
        bossHP.SetActive(false);
        if (boss != null)
        {
            Destroy(boss);
            bossScript = boss.GetComponent<EnemyBoss>();
        }
        if (bossSpawner != null) {
            spawnerScript.spawn();
            boss = GameObject.FindWithTag("Level Boss");
        }
        //reassign hp ui //jk do it in enemy boss
    }

    public void setMusicVolume(float volume) { radioScript.SetRadioVol(volume); pauseMusic.SetRadioVol(volume); }
    public void setPlayerVolume(float volume) { playerScript.aud.volume = volume; }
    public void setFxVolume(float volume) { aud.volume = volume; }

    public void playSFX()
    {
        aud.PlayOneShot(uiSFX[Random.Range(0, uiSFX.Length)], uiVol);
    }
}
