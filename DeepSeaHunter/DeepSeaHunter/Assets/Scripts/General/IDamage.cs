public interface IDamage
{
    enum sourceType
    {
        damage,      // 0 - generic creature 
        o2,          // 1 - oxygen loss
        temp,        // 2 - cold
        trap,        // 3 - trap
        sanity,      // 4 - sanity 
        boss,        // 5 - boss 
        sacrifice    // 6 - self-sacrifice
    }

    void takeDamage(int damage, int source = (int)sourceType.damage);
}
