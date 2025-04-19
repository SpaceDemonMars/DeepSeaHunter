using UnityEngine;

public interface IDamage
{
    enum sourceType { damage, o2, temp };
    void takeDamage(int damage, int source = (int)sourceType.damage);
}
//