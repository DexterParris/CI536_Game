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
    private GameObject particles;
    private Transform playerPos;
    private PlayerMovement player;
    
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
        target = player.transform;
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        UpdateParticles();
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
            if (navUpdateTimer <= 0)
            {
                agent.destination = target.position;
                navUpdateTimer = 3f;
            }
            else
            {
                navUpdateTimer -= Time.deltaTime;
            }    
        }
        
    }
    

    void Attack()
    {
        if (attackCooldown <= 0f)
        {
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
        particles = Instantiate(deathParticles, transform.position + Vector3.down * 0.5f, Quaternion.identity);
        particles.transform.parent = gameObject.transform;
        particles.transform.localPosition = Vector3.zero;
        
        Destroy(GetComponent<NavMeshAgent>());
        //replace with death animation
        rb.freezeRotation = false;
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;
        rb.useGravity = true;
        rb.AddForce(Vector3.forward*50f);
        Destroy(gameObject, 2f);
        //replace with death animation
    }

    public void DamageReciever(float damage, Transform hitPosition)
    {
        health -= damage;
        if (health <= 0)
        {
            Die(hitPosition);
        }
    }

    public void KickReciever(Transform hitPosition)
    {
        DamageReciever(10f, hitPosition);
        rb.freezeRotation = false;
    }
}
