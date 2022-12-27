using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MechBoss : MonoBehaviour, ITappable
{
    [SerializeField] private float hp = 100;
    [SerializeField] private float currentTime;
    [SerializeField] private float attackCooldown;
    [SerializeField] private Animator[] turretAnimator;
    [SerializeField] private Transform[] gunTips;

    //components
    private Outline outline;
    private NavMeshAgent agent;
    private Animator animator;
    private AudioSource audioSource;
    [SerializeField] private AudioClip missileLaunchClip;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private GameObject muzzleFlash;

    private PlayerScript playerScript;
    

    private bool readyToShoot = false;
    private bool isInactive = true;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        outline = GetComponent<Outline>();
        audioSource = GetComponent<AudioSource>();

    }
    // Start is called before the first frame update
    void Start()
    {
        playerScript = GameManager.Instance.GetPlayerScript();
        missileLaunchClip = AssetManager.Instance.missileLaunchClip;
        explosionClip = AssetManager.Instance.explosionClip;
        muzzleFlash = AssetManager.Instance.muzzleFlash;
    }

    // Update is called once per frame
    void Update()
    {
        HandleAddressables();


        if (readyToShoot)
        {
            FaceTarget(GameManager.Instance.GetPlayerTransform().position);

            agent.ResetPath();
            agent.isStopped = true;
            agent.velocity = Vector3.zero;

            currentTime += Time.deltaTime;
            if (currentTime > attackCooldown)
            {
                Shoot();
            }

        }
    }

    void Shoot()
    {
        currentTime = 0;
        audioSource.clip = missileLaunchClip;
        audioSource.Play();

        for (int i = 0; i < gunTips.Length; i++)
        {
            GameObject clone = Instantiate(muzzleFlash, gunTips[i].position, Quaternion.identity);
            ParticleSystem flash = clone.GetComponent<ParticleSystem>();
            flash.Play();
        }

        
         playerScript.DamagePlayer(1);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "BossTrigger")
        {
            readyToShoot = true;
            animator.SetBool("Walk", false);
        }
        
    }

    private void FaceTarget(Vector3 destination)
    {
        Vector3 lookPos = destination - transform.position;
        lookPos.y = 0;
        Quaternion rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 10 * Time.deltaTime);
    }

    public void OnTap()
    {
        // check ammo
        if (this.isInactive || playerScript.GetCurrentWeapon().GetCurrentAmmoCount() <= 0)
        {
            // warning 
            return;
        }

        // check color
        string currentBossColor;
        if (this.gameObject.tag == "RedEnemy")
            currentBossColor = "Red";
        else if (this.gameObject.tag == "GreenEnemy")
            currentBossColor = "Green";
        else if (this.gameObject.tag == "BlueEnemy")
            currentBossColor = "Blue";
        else
            currentBossColor = "Unknown";

        // check tap 
        if (hp > 0 && playerScript.CurrentWeaponColor.ToString() == currentBossColor)
        {
            hp -= 5;
            ChangeEnemyType();
        }
        
        // check HP
        if (hp <= 0)
        {
            hp = 0;
            animator.SetTrigger("Dead");
            playerScript.AddPlayerGold(200);
            GameManager.Instance.UpdateGameState(GameManager.GameStates.BossDefeated);
        }
    }

    private void ChangeEnemyType()
    {
        if (this.gameObject.tag == "RedEnemy")
        {
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                this.gameObject.tag = "GreenEnemy";
                outline.OutlineColor = Color.green;
            }
            else if (rand == 1)
            {
                this.gameObject.tag = "BlueEnemy";
                outline.OutlineColor = Color.blue;

            }
        }
        else if (this.gameObject.tag == "GreenEnemy")
        {
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                this.gameObject.tag = "RedEnemy";
                outline.OutlineColor = Color.red;
            }
            else if (rand == 1)
            {
                this.gameObject.tag = "BlueEnemy";
                outline.OutlineColor = Color.blue;

            }
        }
        else if (this.gameObject.tag == "BlueEnemy")
        {
            int rand = Random.Range(0, 2);
            if (rand == 0)
            {
                this.gameObject.tag = "RedEnemy";
                outline.OutlineColor = Color.red;
            }
            else if (rand == 1)
            {
                this.gameObject.tag = "GreenEnemy";
                outline.OutlineColor = Color.green;

            }
        }
    }

    void HandleAddressables()
    {
        if (missileLaunchClip == null)
        {
            missileLaunchClip = AssetManager.Instance.missileLaunchClip;
        }

        if (explosionClip == null)
        {
            explosionClip = AssetManager.Instance.explosionClip;
        }

        if (muzzleFlash == null)
        {
            muzzleFlash = AssetManager.Instance.muzzleFlash;
        }
        
    }

    public void ActivateBoss()
    {
        this.isInactive = false;
    }
}
