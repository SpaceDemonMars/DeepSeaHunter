using UnityEngine;

public interface ITangle
{
    void stateTangled(int slowFactor);
    void stateUntangled(int slowFactor);
    void toggleTangled(int tangleMod);
}
