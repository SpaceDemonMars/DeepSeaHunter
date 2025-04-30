using UnityEngine;

public class temperature : MonoBehaviour
{
    [SerializeField] float coldSlow;
    [SerializeField] float veryColdSlow;
    [SerializeField] int freezeDmg;
    [SerializeField] float freezeRate;
    float freezeTimer;
    [SerializeField] int gearBase; //serialize field temporarily for testing
    [SerializeField] int zoneMod;
    int playerTemp;
    bool playerSlowed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        calculatePlayerTemp();
    }

    // Update is called once per frame
    void Update()
    {
        applyFreezingDmg();
    }

    void applyTemperatureEffects()
    {
        if (!playerSlowed) {
            if (playerTemp == 2) //cold
            {
                playerSlowed = true;
                GameManager.instance.playerScript.stateTangled(coldSlow);
            }
            else if (playerTemp < 2) //very cold
            {
                playerSlowed = true;
                GameManager.instance.playerScript.stateTangled(veryColdSlow);
                //add oxygen slowed breath here
            }
        }
        applyFreezingDmg();
    }

    void applyFreezingDmg()
    {
        if (playerTemp == 0) //freezing (effect stacks onto very cold)
        {
            freezeTimer += Time.deltaTime; //only update timer while freezing
            if (freezeTimer >= freezeRate)
            {
                freezeTimer = 0;
                GameManager.instance.playerScript.takeDamage(freezeDmg, (int)IDamage.sourceType.temp);
            }
        }
    }

    void calculatePlayerTemp() 
    {
        Debug.Log("Entered Func");
        if (playerSlowed) //clear current effects
        {
            Debug.Log("Attempting to clear slow");
            if (playerTemp == 2) //cold
            {
                Debug.Log("Cleared COLD slow");
                GameManager.instance.playerScript.stateUntangled(coldSlow);
            }
            else if (playerTemp < 2) //very cold
            {
                Debug.Log("Cleared VERY COLD slow");
                GameManager.instance.playerScript.stateUntangled(veryColdSlow);
                //add oxygen slowed breath here
            }
            playerSlowed = false;
        }
        updateTemperatureUI(false);
        playerTemp = gearBase + zoneMod; //recalc temp
        updateTemperatureUI(true);
        applyTemperatureEffects(); //reapply effects
    }

    void updateTemperatureUI(bool state) //bool is so i can also turn it off in here easy
    {
        if (playerTemp < 3) //temp is not warm (no warm icon, would cause index out of range)
            GameManager.instance.temperatureIcons[playerTemp].SetActive(state);
    }

    public int getGearBase() { return gearBase; }
    public void setGearBase(int gB) 
    { 
        gearBase = gB; 
        calculatePlayerTemp();
    }
    public int getZoneMod() { return zoneMod; }
    public void setZoneMod(int zM)
    {
        zoneMod = zM;
        calculatePlayerTemp();
    }
}
