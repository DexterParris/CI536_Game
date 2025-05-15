using System;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    // Enemy Variables
    [Header("Enemy Variables")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float damage = 10f;
    public GameObject deathParticles;
    public ParticleSystem impactParticle;
    public Transform bloodTransform;
    private GameObject particles;
    private Transform playerPos;
    private PlayerMovement player;
    private Animator eAnim;
    private float agentVelocity;
    private bool isDying = false;
    public GameObject gibPrefab;
    public GameObject zombieMesh;
    public Transform modelTransform;
    public SoundTrigger deathSoundTrigger;

    // Movement Variables
    [Header("Movement Variables")]
    public Rigidbody rb;
    public float moveSpeed = 5f;
    public NavMeshAgent agent;
    private float navUpdateTimer = 0f;    


    // Enemy AI Variables
    [Header("AI Variables")]
    public float chaseRange = 10f;
    public float stopRange = 2f;
    public float attackCooldown = 0f;
    public float attackCooldownTime = 1f;
    public Transform target;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        eAnim = gameObject.GetComponentInChildren<Animator>();
        target = player.transform;
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
        
        
        agent.stoppingDistance = stopRange;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParticles();
        if (!isDying)
        {
            agentVelocity = agent.velocity.magnitude/agent.speed;
            eAnim.SetFloat("walkSpeed", agentVelocity);
        }
    }

    private void FixedUpdate()
    {
        ChasePC();
    }

    void UpdateParticles()
    {
        if (particles != null)
        {
            playerPos = player.transform;
            particles.transform.LookAt(playerPos);
        }
    }
    

    void ChasePC()
    {
        if (health > 0 || agent != null)
        {
            if (Vector3.Distance(transform.position, target.position) > stopRange)
            {
                eAnim.SetBool("isWalking", true);
                if (navUpdateTimer <= 0)
                {
                    agent.destination = target.position;
                    navUpdateTimer = 2f;
                }
                else
                {
                    navUpdateTimer -= Time.deltaTime;
                }    
            }
            else
            {
                agent.destination = transform.position;
                Attack();
            }
                
        }
        
    }
    

    void Attack()
    {
        if (attackCooldown <= 0f)
        {
            eAnim.CrossFade("ZombiePunch",0.1f);
            //Debug.Log("Attacking player for " + damage + " damage!");
            attackCooldown = attackCooldownTime;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public void AttackReciever()
    {
        player.DamageReciever(damage);
    }

    void Die()
    {
        deathSoundTrigger.TriggerSounds();


        isDying = true;
        particles = Instantiate(deathParticles, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        particles.transform.parent = bloodTransform.transform;
        particles.transform.localPosition = Vector3.zero;
        
        Destroy(GetComponent<NavMeshAgent>());
        eAnim.CrossFade("ZombieDying",0.1f);
        Destroy(gameObject, 4f);
    }
    void Gib(Transform hitPosition)
    {
        deathSoundTrigger.TriggerSounds();


        isDying = true;
        Destroy(GetComponent<NavMeshAgent>());

        GameObject gibs = Instantiate(gibPrefab, modelTransform.position, transform.rotation);

        // Push the gibs backwards when the player kicks the zombie
        Rigidbody[] gibsRigidbodies = gibs.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rb in gibsRigidbodies)
        {
            rb.AddForce((transform.position - hitPosition.position).normalized * 1000f, ForceMode.Impulse);
        }

        Destroy(zombieMesh);
        Destroy(GetComponent<CapsuleCollider>());
        Destroy(gibs, 4f);
        Destroy(gameObject, 8f);
    }

    public void DamageReciever(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if (!isDying)
            {
                Die();
            }
        }
    }

    public void DamageReciever(float damage, Transform hitPosition)
    {
        health -= damage;
        if (health <= 0)
        {
            if (!isDying)
            {
                Gib(hitPosition);
            }
        }
    }

    public void KickReciever(Transform hitPosition)
    {
        DamageReciever(10f, hitPosition);
    }
}
