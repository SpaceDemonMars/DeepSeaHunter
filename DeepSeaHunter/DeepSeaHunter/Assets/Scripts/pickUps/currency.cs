using UnityEngine;

[CreateAssetMenu]
public class currency : Item
{
    public int fishValue = 0;
    public int scrapValue = 0;

    public override void useItem()
    {
        GameManager.instance.playerScript.inven.addFish(fishValue);
        GameManager.instance.playerScript.inven.addScrap(scrapValue);
    }
}
