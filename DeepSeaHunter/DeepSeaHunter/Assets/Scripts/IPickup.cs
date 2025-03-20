using NUnit.Framework.Interfaces;
using UnityEngine;

public interface IPickup
{
    public void getMeleeStats(meleeStats mweapon);
    public void getRangedStats(rangedStats rweapon);

}
