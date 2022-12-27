using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{

    #region Singleton
    public static EnemySpawner Instance { get; private set; }
    #endregion
    [Header("Enemy")]
    public List<GameObject> enemyPrefab;
    private List<Enemy> enemyList = new List<Enemy>();

    [Space(10)]
    [SerializeField] private Transform[] phase1SpawnPoint;
    private List<Checkpoint> availableCheckpoints = new List<Checkpoint>();
    private List<Checkpoint> usedCheckpoints = new List<Checkpoint>();

    [SerializeField] private GameObject boss;
    [SerializeField] private Transform bossCheckpoint;

    [Header("Spawner Settings")]
    [SerializeField] private float currentTime;
    [SerializeField] private float timeToSpawn;
    [SerializeField] private float minTimeToSpawn;
    [SerializeField] private float maxTimeToSpawn;
    [SerializeField] private int enemyCount;
    [SerializeField] private int bossEnemyCount;

    private bool bossFightStarted = false;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
        }

    }

    private void Start()
    {
        currentTime = 0;
        timeToSpawn = Random.Range(minTimeToSpawn, maxTimeToSpawn);

        enemyPrefab = AssetManager.Instance.enemies;
        
    }

    private void Update()
    {
        currentTime += Time.deltaTime;
        if ((currentTime >= timeToSpawn) && enemyCount > 0 && availableCheckpoints.Count > 0)
        {
            SpawnEnemy();

            currentTime = 0;
            timeToSpawn = Random.Range(minTimeToSpawn, maxTimeToSpawn);
            enemyCount--;
        }

        if (enemyCount <= 0 && !bossFightStarted)
        {
            enemyCount = bossEnemyCount;
            bossFightStarted = true;
            StartBossFight();
        }

    }

    private void SpawnEnemy()
    {
        int randomEnemy = Random.Range(0, 3);
        //Debug.Log(randomEnemy);

        int randomSpawn;
        randomSpawn = Random.Range(0, phase1SpawnPoint.Length);
        GameObject clone = Instantiate(enemyPrefab[randomEnemy], phase1SpawnPoint[randomSpawn].position, Quaternion.identity);
        Enemy enemy = clone.GetComponent<Enemy>();
        enemyList.Add(clone.GetComponent<Enemy>());

        int random;
        random = Random.Range(0, availableCheckpoints.Count);
        enemy.agent.SetDestination(availableCheckpoints[random].transform.position);
        enemy.checkpoint = availableCheckpoints[random];
        MakeCheckpointUsed(availableCheckpoints[random]);

    }

    private void StartBossFight()
    {
        NavMeshAgent bossAgent = boss.GetComponent<NavMeshAgent>();
        bossAgent.SetDestination(bossCheckpoint.position);

        MechBoss mechBoss = boss.GetComponent<MechBoss>();
        mechBoss.ActivateBoss();

        Animator animator = boss.GetComponent<Animator>();
        animator.applyRootMotion = false;
        animator.SetBool("Walk", true);
    }

    public void AddCheckpoint(Checkpoint checkpoint)
    {
        availableCheckpoints.Add(checkpoint);
    }

    public void MakeCheckpointAvailable(Checkpoint checkpoint)
    {
        availableCheckpoints.Add(checkpoint);
        usedCheckpoints.Remove(checkpoint);
    }

    public void MakeCheckpointUsed(Checkpoint checkpoint)
    {
        usedCheckpoints.Add(checkpoint);
        availableCheckpoints.Remove(checkpoint);
    }
}


