using Unity.VisualScripting;
using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class Trap : MonoBehaviour, IDamage
{
    enum trapType { puffer, jelly }
    [SerializeField] trapType type;
    [SerializeField] Renderer model;
    [SerializeField] Animator anim;

    [SerializeField] NavMeshAgent agent;

    [SerializeField] Collider trapCollider;
    [SerializeField] Collider takeDmgCollider;
    [SerializeField] float unPuffRate;

    [SerializeField] int animTranSpeed;

    Color modelColor;
    float unPuffTimer;
    bool isPuffed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        modelColor = model.material.color;
    }

    // Update is called once per frame
    void Update()
    {
        unPuffTimer += Time.deltaTime;
        if (isPuffed && unPuffTimer >= unPuffRate)
        {
            Debug.Log("UN-Puffed");
            isPuffed = false;
            unPuffTimer = 0;
            trapCollider.enabled = isPuffed;
            takeDmgCollider.enabled = !isPuffed;
            anim.Play("Swim");
        }
        //add roaming movement
    }
    public void takeDamage(int damage)
    {
        StartCoroutine(flashWhite());
        if (type == trapType.puffer)
        {
            Debug.Log("Puffed");
            isPuffed = true;
            unPuffTimer = 0;
            trapCollider.enabled = isPuffed;
            takeDmgCollider.enabled = !isPuffed;
            anim.Play("BlowUp");
        }
    }

    IEnumerator flashWhite()
    {
        model.material.color = Color.white;
        yield return new WaitForSeconds(.1f);
        model.material.color = modelColor;
    }
}
