using UnityEngine;

public class oxygen : MonoBehaviour
{
    [SerializeField] playerController player;
    [SerializeField] int o2;
    int o2Max;
    [SerializeField] int drownDmg;
    [SerializeField] float o2TickRate;
    float o2Timer;

    bool inO2Zone;

    void Start()
    {
        o2Max = o2;
        updateO2UI();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inO2Zone) trackOxygen();
    }

    public bool getOxygen() { return inO2Zone; }
    public void setOxygen(bool value) 
    {
        inO2Zone = value;
        if (inO2Zone) maxO2();
    }

    void trackOxygen()
    {
        o2Timer += Time.deltaTime;
        if (o2Timer >= o2TickRate)
        {
            o2Timer = 0;
            if (o2 >= drownDmg) o2 -= drownDmg; //reduce o2 only
            else if (o2 > 0) //reduce o2, deal remaining as dmg
            {
                int dif = drownDmg - o2;
                o2 = 0;
                player.takeDamage(dif);
            }
            else //deal dmg only
            {
                player.takeDamage(drownDmg);
            }
            updateO2UI();
        }
    }

    void updateO2UI()
    {
        GameManager.instance.playerO2Bar.fillAmount = (float)o2 / o2Max;
        GameManager.instance.playerO2UI.SetActive(!inO2Zone);
    }

    public void modifyO2(int num) //use this func for o2 consumeables/pickups
    { 
        o2 += num; 
        updateO2UI();
    }

    void maxO2() 
    { 
        o2 = o2Max;
        updateO2UI();
    }
}
