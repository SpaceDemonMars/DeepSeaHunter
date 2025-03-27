using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class spawner : MonoBehaviour, IDamage
{
    [SerializeField] GameObject objectToSpawn;
    [SerializeField] int numToSpawn;
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;

    [SerializeField] GameObject chestSpawnerLid;
    [SerializeField] Transform chestHalfOpenPos;
    [SerializeField] Transform chestOpenPos;
    [SerializeField] int chestOpenSpeed;

    float spawnTimer;

    int spawnCounter;

    bool startSpawning;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        spawnTimer += Time.deltaTime;

        if(startSpawning && objectToSpawn != null)
        {
            if (spawnCounter < numToSpawn && timeBetweenSpawns <= spawnTimer)
            spawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            startSpawning = true;
        }
    }

    void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);

        Instantiate(objectToSpawn, spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        spawnCounter++;
        spawnTimer = 0;
    }

    public void takeDamage(int damage)
    {
        StartCoroutine(openChest());
    }

    IEnumerator openChest()
    {
        while (chestSpawnerLid.transform.rotation.x >= chestOpenPos.rotation.x)
        {
            chestSpawnerLid.transform.Rotate(Vector3.right * Mathf.Lerp(chestSpawnerLid.transform.rotation.x, -90, chestOpenSpeed * Time.deltaTime));
            if (chestSpawnerLid.transform.rotation.x <= chestHalfOpenPos.rotation.x && !startSpawning)
                startSpawning = true;
            yield return new WaitForSeconds(.1f);
        }
    }
}
