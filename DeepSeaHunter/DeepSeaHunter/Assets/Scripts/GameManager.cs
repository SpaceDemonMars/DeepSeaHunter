using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public GameObject player;
    public playerController playerScript;

    [SerializeField] GameObject menuActive;
    [SerializeField] GameObject menuPause;
    [SerializeField] GameObject menuWin;
    [SerializeField] GameObject menuLose;

    public bool isPaused;
    public int goalCount;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        instance = this;
        player = GameObject.FindWithTag("Player");
        playerScript = player.GetComponent<playerController>();
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

    public void updateGameGoal(int amount)
    {
        goalCount += amount;

        if (goalCount <= 0)
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
}
