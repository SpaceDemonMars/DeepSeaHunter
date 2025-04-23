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
            GameManager.instance.playerScript.tempScript.setZoneMod(zone.zoneTempMod);
            GameManager.instance.playerScript.radioScript.setInStatic(zone.hasStatic);
        }
    }
}
