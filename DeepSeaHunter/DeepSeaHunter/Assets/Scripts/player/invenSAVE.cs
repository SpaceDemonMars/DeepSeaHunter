using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class invenSAVE : ScriptableObject
{
    public int fish;
    public int scrap;
    public List<Item> items;
    public List<int> qty;
}
