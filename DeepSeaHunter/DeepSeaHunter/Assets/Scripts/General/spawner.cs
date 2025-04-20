using System.Collections;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class spawner : MonoBehaviour, IDamage
{
    [SerializeField] GameObject[] objectToSpawn;
    [SerializeField] int numToSpawn; //idea to make this a range (multi item chest)
    [SerializeField] int timeBetweenSpawns;
    [SerializeField] Transform[] spawnPos;
    [SerializeField] bool bossSpawner;

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

    public void spawn()
    {
        int arrayPos = Random.Range(0, spawnPos.Length);

        Instantiate(objectToSpawn[Random.Range(0, objectToSpawn.Length)], spawnPos[arrayPos].position, spawnPos[arrayPos].rotation);
        if (!bossSpawner) {
            spawnCounter++;
            spawnTimer = 0;
        }
    }

    public void takeDamage(int damage, int source)
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
