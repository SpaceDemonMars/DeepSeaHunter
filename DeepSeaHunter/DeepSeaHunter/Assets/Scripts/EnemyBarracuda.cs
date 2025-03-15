using UnityEngine;
using UnityEngine.AI;


//This work is by Leanne, I've only moved it to fix a bug from merging -paige 
public class EnemyBarracuda : EnemyAI
{ 
    [SerializeField] int biteDmg;
    [SerializeField] float biteRate;
    [SerializeField] int biteDist;

    //I've commented this out to remove this warning (also line 55) -paige
    //warning CS0414: The field 'EnemyBarracuda.biteTimer' is assigned but its value is never used
    //int biteTimer; 

    override protected void Start() //made this override protected, so it can function as a child class -paige
    {
        base.agent = GetComponent<NavMeshAgent>();
        NavMeshHit hit;
        if (NavMesh.SamplePosition(transform.position, out hit, 5f, NavMesh.AllAreas))
        {
            transform.position = hit.position;
            base.agent.enabled = true;
        }
        GameManager.instance.updateGameGoal(1);
        
    }
    //This was the version of update included with the bite code, but i noticed it updates time and
    //tries to call shoot twice, to be safe ive commented it out, so it uses EnemyAI's update instead -paige
             
    /*void Update()
    {
        shootTimer += Time.deltaTime;
        if (shootTimer >= shootRate)
        {
            shoot();
        }
        setAnimLocomotion();
        shootTimer += Time.deltaTime;
        if (playerInRange)
        {
            playerDir = GameManager.instance.player.transform.position - transform.position;
            agent.SetDestination(GameManager.instance.player.transform.position);

            if (shootTimer >= shootRate)
                shoot();
            if (agent.remainingDistance <= agent.stoppingDistance)
                faceTarget();
        }
    }*/

    void bite()
    {
        //biteTimer = 0;
        RaycastHit hit;

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, biteDist))
        {
            Debug.Log(hit.collider.name);
            IDamage dmg = hit.collider.GetComponent<IDamage>();

            if (dmg != null)
            {
                dmg.takeDamage(biteDmg);
            }
        }
    }
}
