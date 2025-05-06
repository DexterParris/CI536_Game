using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy Variables
    [Header("Enemy Variables")]
    public float health = 100f;
    public float maxHealth = 100f;
    public float damage = 10f;
    public float attackRate = 1f;
    public float attackRange = 2f;
    public ParticleSystem deathParticles;
    public ParticleSystem impactParticle;

    // Movement Variables
    [Header("Movement Variables")]
    public Rigidbody rb;
    public float moveSpeed = 5f;


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

    }

    // Update is called once per frame
    void Update()
    {
        ChasePC();
        Move();
    }

    void ChasePC()
    {
        if (target != null)
        {
            float distance = Vector3.Distance(transform.position, target.position);
            if (distance < chaseRange && distance > stopRange)
            {
                Move();
            }
            else if (distance <= stopRange)
            {
                // Attack the player
                Attack();
            }
        }
    }

    void Move()
    {
        if (target != null)
        {
            Vector3 direction = target.position - transform.position;
            direction.Normalize();
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    void Attack()
    {
        if (attackCooldown <= 0f)
        {
            // Implement attack logic here
            Debug.Log("Attacking player for " + damage + " damage!");
            attackCooldown = attackCooldownTime;
        }
        else
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    void Die()
    {
        deathParticles = Instantiate(deathParticles, transform.position + Vector3.down * 0.5f, Quaternion.identity);

        //replace with death animation
        rb.freezeRotation = false;
        rb.AddForce(Vector3.forward*50f);
        //Destroy(gameObject);
        //replace with death animation
    }

    public void DamageReciever(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Die();
        }
    }
}
