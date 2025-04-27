using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;

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
    public TMP_Text questPopupText;
    public GameObject questPopupObject;
    private List<int> savedClueIDs = new List<int>();

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

    public void Save()
    {
        generalSAVE gameSave = new()
        {
            //update save data
            pSave = playerScript.savePlayer(),
            iSave = playerScript.inven.saveInven(),
            o2 = o2.getO2(),
            inO2Zone = o2.getOxygen(),
            inStatic = radioScript.getInStatic(),
            radioOn = radioScript.getRadioOn()
        };
        Debug.Log("Success: Save (General)");

        //create save file
        SaveManager.instance.Save(gameSave);
    }

    public void Load() 
    {
        //retrieve save file
        generalSAVE gameSave = SaveManager.instance.Load();
        if (gameSave == null) return; //no save dat; do nothing

        //update data
        playerScript.loadPlayer(gameSave.pSave);
        playerScript.inven.loadInven(gameSave.iSave);
        o2.setO2(gameSave.o2);
        o2.setOxygen(gameSave.inO2Zone);
        radioScript.setInStatic(gameSave.inStatic);
        radioScript.setRadioOn(gameSave.radioOn);
        Debug.Log("Success: Load (General)");
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

    public void SaveClueFound(int clueID)
    {
        if (!savedClueIDs.Contains(clueID))
            savedClueIDs.Add(clueID);
    }

    public bool IsClueFound(int clueID)
    {
        return savedClueIDs.Contains(clueID);
    }

    public List<int> GetFoundClueIDs()
    {
        return savedClueIDs;
    }
    public void loadInventory() //updates inventory display
    {
        for (int i = 0; i < invenButtons.Length; i++)
        {
            invenButtons[i].SetActive(false);
            if (i < playerScript.inven.getInvenSize())
            {
                inventoryButtons tempScript = invenButtons[i].GetComponent<inventoryButtons>();
                if (tempScript != null)
                {
                    tempScript.item = playerScript.inven.getItem(i);
                    tempScript.itemName.text = playerScript.inven.getItem(i).itemName;
                }
                invenButtons[i].SetActive(true);
            }
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
        pauseMusic.togglePauseMusic(!radioScript.getRadioOn());
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
    public void resetLevelBoss()
    {
        //turn off boss hp UI here
        bossHP.SetActive(false);
        if (boss != null)
        {
            Destroy(boss);
            bossScript = boss.GetComponent<EnemyBoss>();
        }
        if (bossSpawner != null)
        {
            spawnerScript.spawn();
            boss = GameObject.FindWithTag("Level Boss");
        }
        //reassign hp ui //jk do it in enemy boss
    }

    public void ShowQuestPopup(string message)
    {
        questPopupText.text = message;
        questPopupObject.SetActive(true);
        StartCoroutine(HideQuestPopup());
    }

    private IEnumerator HideQuestPopup()
    {
        yield return new WaitForSeconds(3f); // Show for 3 seconds
        questPopupObject.SetActive(false);
    }

    public void setMusicVolume(float volume) { radioScript.SetRadioVol(volume); pauseMusic.SetRadioVol(volume); }
    public void setPlayerVolume(float volume) { playerScript.aud.volume = volume; }
    public void setFxVolume(float volume) { aud.volume = volume; }

    public void playSFX()
    {
        aud.PlayOneShot(uiSFX[Random.Range(0, uiSFX.Length)], uiVol);
    }
    //previous win menu
    /* public void updateGameGoal(string bossName, bool slain)
     {
         goalCountText.text = bossName;

         if (slain)
         {
             statePause();
             menuActive = menuWin;
             menuActive.SetActive(true);
         }
     }*/

    public void updateGameGoal(string bossName, bool slain)
    {
        goalCountText.text = bossName;

        if (slain)
        {
            int clueCount = JournalManager.instance.GetClueCount();

            if (clueCount >= 8) // adjust these numbers depending on how far we get
            {
                LoadingManager.LoadSceneWithTracking("GoodEndingScene");
            }
            else if (clueCount >= 4)
            {
                LoadingManager.LoadSceneWithTracking("NeutralEndingScene");
            }
            else
            {
                LoadingManager.LoadSceneWithTracking("BadEndingScene");
            }
        }
    }

    public void youLose(int source)
    {
        statePause();
        dialogHiddenUI.SetActive(false); // hide UI

        // color based on source
        loseYouDied.color = loseTextColors[source];
        loseMessage.color = loseTextColors[source];
        // flavor text and last words
        loseMessage.text = GetRandomDeathMessage(source);
        loseYouDied.text = GetRandomLastWords(source);
        // Show the lose menu
        menuActive = menuLose;
        menuActive.SetActive(true);
    }
    private readonly string[][] loseMessages = new string[][]
{
    // 0: Frostbite
    new string[] {
        "The cold reached deeper than your lungs—your story ends in silence.",
        "The warmth fled first. Then thought. Then breath."
    },
    // 1: Oxygen Deletion
    new string[] {
        "No air left to scream. The deep welcomed you quietly.",
        "The last breath was not yours to keep."
    },
    // 2: Basic Creature
    new string[] {
        "Not all deaths are grand. Some come clawing, biting, forgotten.",
        "The sea feeds its own. You were simply next."
    },
    // 3: Trap
    new string[] {
        "Curiosity cracked the seal. The trap sprung true.",
        "The past protects itself—blood for secrets."
    },
    // 4: Low Sanity
    new string[] {
        "The mind slipped beneath the waves long before the body did.",
        "You drowned in thoughts that were not your own."
    },
    // 5: Leviathan
    new string[] {
        "The abyss opened. It remembered your name.",
        "You looked into the deep... and it looked back.",
        "The abyss whispered sweet lies, and you followed."
    },
    // 6: Self-sacrifice
    new string[] {
        "Some endings cannot be fought. Only chosen.",
        "The story closes where it began—beneath the surface."
    }
};

    private readonly string[][] lastWords = new string[][]
    {
    // 0: Frostbite
    new string[] {
        "C-can’t… feel my hands…",
        "So cold… deeper than bone…",
        "Need fire… just a little more…",
        "Jewel… your suit… it wasn’t enough…"
    },
    // 1: Oxygen Deletion
    new string[] {
        "I… can’t… breathe…",
        "Too far… too deep…",
        "Nathan… I’m sorry…"
    },
    // 2: Basic Creature
    new string[] {
        "It’s just a crab…?",
        "Not like this…",
        "Damn it… should’ve run…"
    },
    // 3: Trap
    new string[] {
        "Wait—no, no—",
        "I should’ve known…",
        "What did I step into…?"
    },
    // 4: Low Sanity
    new string[] {
        "That’s not real… right?",
        "Stop whispering… please…",
        "I’m still me. I’m still me…",
        "Nathan… I saw you… you were smiling…",
        "Where’s the surface? It was right here…"
    },
    // 5: Leviathan
    new string[] {
        "It’s real…",
        "I see it… the eye…",
        "No one will believe this…",
        "Tell them… tell them it’s still down here…"
    },
    // 6: Self-sacrifice
    new string[] {
        "This is the only way…",
        "Tell them. Make them remember.",
        "If it keeps him safe… so be it."
    }
    };

    private string GetRandomDeathMessage(int source)
    {
        if (source >= 0 && source < loseMessages.Length)
        {
            return loseMessages[source][Random.Range(0, loseMessages[source].Length)];
        }
        return "The deep claimed you.";
    }

    private string GetRandomLastWords(int source)
    {
        if (source >= 0 && source < lastWords.Length)
        {
            return "\"" + lastWords[source][Random.Range(0, lastWords[source].Length)] + "\"";
        }
        return "\"...\""; // fallback if something goes wrong
    }

    /* public void youLose(int source)
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
    */


}
