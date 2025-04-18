using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

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
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    [Header("UI")]
    public GameObject checkpointPopup;
    [SerializeField] GameObject goalCountLabel;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject tutorialText;
    [SerializeField] GameObject dialogHiddenUI;

    [Header("Player")]
    public GameObject player;
    public playerController playerScript;
    public GameObject playerSpawnPos;

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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
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
    }

    // Update is called once per frame
    void Update()
    {
        openTabSettings();
        openTabInventory();
        openTabEquipment();
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
            if (!isPaused)
            {
                statePause();
                menuActive = menuTab;
                menuActiveTab = menuTabInventory;
                menuTabText.text = "Inventory";
                menuActive.SetActive(true);
                menuActiveTab.SetActive(true);
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
            else if (isPaused && menuActive == menuTab) //other tab menu
            {
                menuActiveTab.SetActive(false);
                menuActiveTab = menuTabEquipment;
                menuTabText.text = "Equipment";
                menuActiveTab.SetActive(true);
            }
        }
    }

    public void statePause()
    {
        isPaused = !isPaused;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Time.timeScale = 0;
    }

    public void stateUnpause()
    {
        isPaused = !isPaused;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        Time.timeScale = 1;
        menuActive.SetActive(false);
        menuActive = null;
        menuActiveTab.SetActive(false);
        menuActiveTab = null;
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

    public void youLose()
    {
        statePause();
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    //

    public void resetLevelBoss()
    {
        //turn off boss hp UI here
        bossHP.SetActive(false);
        if (boss !=  null) 
            Destroy(boss);
        spawnerScript.spawn();
        boss = GameObject.FindWithTag("Level Boss");
        bossScript = boss.GetComponent<EnemyBoss>();
        //reassign hp ui //jk do it in enemy boss
    }
}
