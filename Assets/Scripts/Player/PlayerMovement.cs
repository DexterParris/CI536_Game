using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // Player Variables
    [Header("Player Variables")]
    public float health = 100f;
    public float maxHealth = 100f;
    public Rigidbody rb;

    // Camera Variables
    Transform cameraTrans;
    public float cameraSensitivity = 1f;

    // Movement Variables
    [Header("Movement Variables")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;

    // Weapon Variables
    [Header("Weapon Variables")]
    public Weapon weapon;
    public int maxAmmo = 12;
    public int ammo = 0;
    public Transform weaponSlot;




    // Start is called before the first frame update
    void Start()
    {
        // Set initial values
        ammo = maxAmmo;
        cameraTrans = Camera.main.transform;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Shoot();
        Reload();
        Jump();
        UpdateUI();
    }


    //-------------------- Cyclical Functions --------------------
    void Move()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        
        // Calculate movement
        Vector3 move = transform.right * x + transform.forward * z;
        
        transform.position += move * moveSpeed * Time.deltaTime;
    }

    void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.position, Vector3.down, out hit, 1.1f))
            {
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }



    void Shoot()
    {
        if(weapon == null)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0) && ammo > 0)
        {

            RaycastHit hit;
            if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out hit, 1000))
            {
                if (hit.transform.CompareTag("Enemy"))
                {
                    hit.transform.GetComponent<Enemy>().DamageReciever(weapon.bulletDamage);
                }
                else
                {
                    //Instantiate spark effect facing the player
                    ParticleSystem impactParticle = Instantiate(weapon.impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                }
            }

        }
    }

    void Reload()
    {
        if(Input.GetKeyDown(KeyCode.R))
        {
            if(weapon.clipAmmo == weapon.maxClipAmmo)
            {
                print("Your clip is already full");
                return;
            }
            else
            {
                // Fill the weapons ammo up to the max ammo for the weapon and take it from the players ammo
                if (ammo > weapon.maxClipAmmo)
                {
                    weapon.clipAmmo = weapon.maxClipAmmo;
                    ammo -= weapon.maxClipAmmo;
                }
                else if (ammo <= 0)
                {
                    print("You don't have enough ammo to reload");
                    return;
                }
                else
                {
                    weapon.clipAmmo = ammo;
                    ammo = 0;
                }
            }
        }
    }

    void UpdateUI()
    {
        // Update the UI with the players health and ammo
    }

    



    //-------------------- Pickup Functions --------------------
    public void PickupAmmo(int ammoAmount)
    {
        ammo += ammoAmount;
        if (ammo > maxAmmo)
        {
            ammo = maxAmmo;
        }
    }

    public void PickupHealth(float healthAmount)
    {
        health += healthAmount;
        if (health > maxHealth)
        {
            health = maxHealth;
        }
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        weapon = newWeapon;
        weapon.transform.SetParent(weaponSlot);
    }


}
