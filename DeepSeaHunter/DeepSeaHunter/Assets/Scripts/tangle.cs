using UnityEngine;

public class tangle : MonoBehaviour
{
    [SerializeField] int tangleMod;

    private void OnTriggerEnter(Collider other)
    {
        CallThisTheyDoTheSameThing(other);
    }
    private void OnTriggerExit(Collider other)
    {
        CallThisTheyDoTheSameThing(other);
    }

    void CallThisTheyDoTheSameThing(Collider other)
    {
        if (other.isTrigger)
            return;
        ITangle tangle = other.GetComponent<ITangle>();
        if (tangle != null)
        {
            tangle.toggleTangled(tangleMod);
        }
    }
}
