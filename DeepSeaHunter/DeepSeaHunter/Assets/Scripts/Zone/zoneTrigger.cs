using UnityEngine;

public class zoneTrigger : MonoBehaviour
{
    [SerializeField] zoneStats zone;

    private void OnTriggerEnter(Collider other)
    {
        playerController player = other.GetComponent<playerController>();
        if (player != null)
        {
            GameManager.instance.playerScript.o2Script.setOxygen(zone.hasOxygen);
            if (!zone.isBubbles)
            {
                GameManager.instance.playerScript.tempScript.setZoneMod(zone.zoneTempMod);
                GameManager.instance.playerScript.radioScript.setInStatic(zone.hasStatic);
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        playerController player = other.GetComponent<playerController>();
        if (player != null && zone.isBubbles)
        {
            GameManager.instance.playerScript.o2Script.setOxygen(!zone.hasOxygen);
        }
    }
}
