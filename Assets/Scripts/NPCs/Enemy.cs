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
    ParticleSystem deathParticles;

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
        
    }

    void Move()
    {

    }


        void Die()
    {
        deathParticles = Instantiate(deathParticles, transform.position, Quaternion.identity);
        Destroy(gameObject);
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
