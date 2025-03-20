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

    [SerializeField] int roamPauseTime;
    [SerializeField] int roamDist;

    [SerializeField] Collider trapCollider;
    [SerializeField] Collider takeDmgCollider;
    [SerializeField] float unPuffRate;

    [SerializeField] int animTranSpeed;

    Color modelColor;
    float roamTimer;
    Vector3 startingPos;
    float unPuffTimer;
    bool isPuffed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    { 
        modelColor = model.material.color;
        startingPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        unPuffTimer += Time.deltaTime;
        if (agent.remainingDistance < 0.01f)
            roamTimer += Time.deltaTime;
        if (isPuffed && unPuffTimer >= unPuffRate)
        {
            Debug.Log("UN-Puffed");
            isPuffed = false;
            unPuffTimer = 0;
            trapCollider.enabled = isPuffed;
            takeDmgCollider.enabled = !isPuffed;
            anim.Play("Swim");
        }
        //trap doesnt need to see the player ignore FOV/In range checks
        checkRoam();
    }
    void checkRoam()
    { //since trap never stops roaming, ignore player alive check
        if (roamTimer > roamPauseTime && agent.remainingDistance < 0.01f)
        {
            roam();
        }
    }

    void roam()
    {
        roamTimer = 0;
        Vector3 ranPos = Random.insideUnitSphere * roamDist;
        ranPos += startingPos;
        NavMeshHit hit;
        NavMesh.SamplePosition(ranPos, out hit, roamDist, 1);
        agent.SetDestination(hit.position);
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
