using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour, ITappable
{
    public enum EnemyColor
    {
        Unknown,
        Red,
        Green,
        Blue
    }

    [Header("Enemy Stats")]
    [SerializeField] private float hp;
    [SerializeField] private float damage;
    [SerializeField] private float currentTime;
    [SerializeField] private float minTimeToShoot;
    [SerializeField] private float maxTimeToShoot;
    [SerializeField] private float timeToShoot;
    [SerializeField] private float chanceToHit;
    [SerializeField] private float despawnDelay;
    [SerializeField] private EnemyColor enemyColor = EnemyColor.Unknown;


    [Header("Enemy Components")]
    public Transform point;
    public NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;

    [Header("Effects")]
    [SerializeField] private Transform guntip;
    private GameObject muzzleFlash;

    [Header("Debug Checkpoint")]
    public Checkpoint checkpoint;


    //stuff
    private bool readyToShoot = false;
    private bool isDead = false;
    private Transform playerTransform;
    private PlayerScript playerScript;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();

        animator.applyRootMotion = false;

        playerScript = GameManager.Instance.GetPlayerScript();
        playerTransform = GameManager.Instance.GetPlayerTransform();
        timeToShoot = Random.Range(minTimeToShoot, maxTimeToShoot);
        
    }

    private void Start()
    {
        audioSource.clip = AssetManager.Instance.gunshotClip;
        muzzleFlash = AssetManager.Instance.muzzleFlash;
    }

    // Update is called once per frame
    void Update()
    {
        if (readyToShoot)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            FaceTarget(playerTransform.position);
            currentTime += Time.deltaTime;

            if (currentTime > timeToShoot && !isDead)
            {
                Shoot();
            }

        }

        if (isDead)
        {
            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            currentTime += Time.deltaTime;
            if (currentTime > despawnDelay)
            {
                Destroy(this.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Checkpoint")
        {
            readyToShoot = true;
            animator.SetBool("ReadyToShoot", true);
            
        }
        
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10 * Time.deltaTime);
    }

    private void Shoot()
    {
        audioSource.Play();
        currentTime = 0;
        timeToShoot = Random.Range(minTimeToShoot, maxTimeToShoot);
        animator.SetTrigger("Shoot");

        GameObject clone = Instantiate(muzzleFlash, guntip.position, Quaternion.identity);
        ParticleSystem flash = clone.GetComponent<ParticleSystem>();
        flash.Play();
        

        //chance to spawn bullet to player
        int hit = Random.Range(0, 10);
        if (hit < 3)
        {
            playerScript.DamagePlayer(1);
        }
    }

    public void Death()
    {
        isDead = true;
        readyToShoot = false;
        currentTime = 0;

        int deathType = Random.Range(0, 2);
        animator.SetInteger("DeathType", deathType);
        animator.SetTrigger("Dead");

        //clear checkpoint
        EnemySpawner.Instance.MakeCheckpointAvailable(checkpoint);
        checkpoint = null;

        // add player gold
        playerScript.AddPlayerGold(50);
    }

    public void OnTap()
    {
        if (playerScript.GetCurrentWeapon().GetCurrentAmmoCount() <= 0)
        {
            // warning
            return;
        }

        if (playerScript.CurrentWeaponColor.ToString() == enemyColor.ToString())
        {
            Death();
            this.GetComponent<CapsuleCollider>().enabled = false;
        }
    }

}
