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
    public float attackRate = 1f;
    public float attackRange = 2f;
    public GameObject deathParticles;
    public ParticleSystem impactParticle;
    public Transform bloodTransform;
    private GameObject particles;
    private Transform playerPos;
    private PlayerMovement player;
    private Animator eAnim;
    private float agentVelocity;
    private bool isDying = false;
    
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
        player = GameObject.FindWithTag("Player").GetComponentInChildren<PlayerMovement>();
        eAnim = gameObject.GetComponentInChildren<Animator>();
        target = player.transform;
        agent = GetComponent<NavMeshAgent>();
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParticles();
        if (health > 0)
        {
            agentVelocity = agent.velocity.magnitude/agent.speed;
        }
        eAnim.SetFloat("walkSpeed", agentVelocity);
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
            player.DamageReciever(damage);
            attackCooldown = attackCooldownTime;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    void Die(Transform hitPosition)
    {
        isDying = true;
        particles = Instantiate(deathParticles, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        particles.transform.parent = bloodTransform.transform;
        particles.transform.localPosition = Vector3.zero;
        
        Destroy(GetComponent<NavMeshAgent>());
        eAnim.CrossFade("ZombieDying",0.1f);
        Destroy(gameObject, 4f);
    }

    public void DamageReciever(float damage, Transform hitPosition)
    {
        health -= damage;
        if (health <= 0)
        {
            if (!isDying)
            {
                Die(hitPosition);
            }
        }
    }

    public void KickReciever(Transform hitPosition)
    {
        DamageReciever(10f, hitPosition);
        rb.freezeRotation = false;
    }
}
