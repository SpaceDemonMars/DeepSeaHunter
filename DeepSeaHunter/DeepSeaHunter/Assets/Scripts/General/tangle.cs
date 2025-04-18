using UnityEngine;

public class tangle : MonoBehaviour
{
    [SerializeField] float tangleMod;

    private void OnTriggerEnter(Collider other)
    {
        if (other.isTrigger)
            return;
        ITangle tangle = other.GetComponent<ITangle>();
        if (tangle != null)
        {
            tangle.stateTangled(tangleMod);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.isTrigger)
            return;
        ITangle tangle = other.GetComponent<ITangle>();
        if (tangle != null)
        {
            tangle.stateUntangled(tangleMod);
        }
    }
}
