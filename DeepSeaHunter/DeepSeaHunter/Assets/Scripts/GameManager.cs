using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public playerController playerScript;

    public GameObject boss;
    public EnemyBoss bossScript;

    public GameObject bossSpawner;
    public spawner spawnerScript;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;
    [SerializeField] GameObject goalCountLabel;
    [SerializeField] TMP_Text goalCountText;
    [SerializeField] GameObject tutorialText;

    public GameObject playerSpawnPos;
    public GameObject playerDamageScreen;
    public GameObject playerHealScreen;
    public GameObject playerSlowScreen;
    public Image playerHPBar;

    public GameObject bossHP;
    public TMP_Text bossBarText;
    public Image bossHPBar;
    public Image bossArmorBar;

    public Image harpoonChargeBar;
    public Image harpoonReloadBar;
    public Image knifeReloadBar;

    public GameObject weaponUpgradePopup;
    public TMP_Text weaponUpgradeText;
    public GameObject checkpointPopup;

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
        if (Input.GetButtonDown("Cancel"))
        {
            if (!isPaused)
            {
                statePause();
                menuActive = menuPause;
                menuActive.SetActive(true);
            }
            else if (isPaused && menuActive == menuPause)
            {
                stateUnpause();
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
