using UnityEngine;

[CreateAssetMenu]
public class consumable : Item
{
    public int hp = 0;
    public int o2 = 0;
    public int sanity = 0;

    public override void useItem()
    {
        GameManager.instance.playerScript.takeDamage(hp * -1, 0);
        GameManager.instance.o2.modifyO2(o2);
        //update sanity
    }
}
