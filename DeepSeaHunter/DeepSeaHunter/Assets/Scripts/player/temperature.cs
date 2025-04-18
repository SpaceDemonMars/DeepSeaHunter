using UnityEngine;

public class temperature : MonoBehaviour
{
    [SerializeField] playerController player;
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
                player.stateTangled(coldSlow);
            }
            else if (playerTemp < 2) //very cold
            {
                playerSlowed = true;
                player.stateTangled(veryColdSlow);
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
                player.takeDamage(freezeDmg);
            }
        }
    }

    void calculatePlayerTemp() 
    {
        if (playerSlowed) //clear current effects
        {
            if (playerTemp == 2) //cold
            {
                player.stateUntangled(coldSlow);
            }
            else if (playerTemp < 2) //very cold
            {
                player.stateUntangled(veryColdSlow);
                //add oxygen slowed breath here
            }
            playerSlowed = false;
        }
        playerTemp = gearBase + zoneMod; //recalc temp
        applyTemperatureEffects(); //reapply effects
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
