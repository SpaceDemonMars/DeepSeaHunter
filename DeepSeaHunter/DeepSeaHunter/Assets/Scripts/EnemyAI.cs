using System.Collections;
using UnityEngine;

public class EnemyAI : MonoBehaviour, IDamage
{
    [SerializeField] int HP;
    [SerializeField] Renderer model;

    Color modelColor;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.instance.updateGameGoal(1);
        modelColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void takeDamage(int damage)
    {
        HP -= damage;
        StartCoroutine(flashRed());

        if (HP <= 0)
        {
            GameManager .instance.updateGameGoal(-1);
            Destroy(gameObject);
        }
    }

    IEnumerator flashRed()
    {
        model.material.color = Color.red;
        yield return new WaitForSeconds(.1f);
        model.material.color = modelColor;
    }
}
