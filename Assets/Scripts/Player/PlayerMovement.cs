using Unity.Mathematics;
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
    private float currentMoveSpeed = 5f;
    private float cameraPitch = 0f;
    private bool jumpQueued = false;

    // Weapon Variables
    [Header("Weapon Variables")]
    public Weapon weapon;
    public int maxAmmo = 12;
    public int ammo = 0;
    public Transform weaponSlot;
    public Transform legObject;
    private Animator weaponAnim;
    private Animator legAnim;

    

    // Start is called before the first frame update
    void Start()
    {
        // Set initial values
        ammo = maxAmmo;
        cameraTrans = Camera.main.transform;
        weaponAnim = weaponSlot.GetComponent<Animator>();
        legAnim = legObject.GetComponent<Animator>();

        //lock the cursor
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;

    }

    // Update is called once per frame
    void Update()
    {
        Jump();
        Shoot();
        Kick();
        Reload();
        UpdateUI();
    }

    private void LateUpdate()
    {
        Look();
        Move();
    }

    private void FixedUpdate()
    {
    }


    //-------------------- Cyclical Functions --------------------
    void Move()
    {
        if(Input.GetKey(KeyCode.LeftShift))
        {
            currentMoveSpeed = moveSpeed * 1.7f;
        }
        else
        {
            currentMoveSpeed = moveSpeed;
        }

        // Calculate movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Cap the movement speed
        Vector3 velocity = move * currentMoveSpeed;
        rb.velocity = new Vector3(velocity.x, rb.velocity.y, velocity.z);

        // Roll the camera 
        Vector3 localVelocity = cameraTrans.InverseTransformDirection(rb.velocity);
        float roll = localVelocity.x * -0.4f;
        cameraTrans.localRotation *= Quaternion.Euler(0f, 0f, roll);



    }

    void Look()
    {
        float yRot = Input.GetAxis("Mouse X") * cameraSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * cameraSensitivity;

        cameraPitch -= xRot;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        cameraTrans.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
        transform.Rotate(Vector3.up * yRot);
        
    }

    void Jump()
    {

        if (Input.GetKeyDown(KeyCode.Space))
        {
            RaycastHit jumpCheck;
            Physics.Raycast(transform.position + Vector3.up * 0.02f, Vector3.down, out jumpCheck, 10f);

            if (jumpCheck.distance < 0.45f)
            {
                jumpQueued = true;
            }
        }

        RaycastHit groundCheck;
        if (Physics.Raycast(transform.position + Vector3.up * 0.2f, Vector3.down, out groundCheck, 0.3f) && jumpQueued)
        {
            if (groundCheck.distance -0.2f < 0.01f)
            {
                rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
                rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
                jumpQueued = false;
                return;
            }
        }
    }



    void Shoot()
    {

        if (Input.GetKeyDown(KeyCode.Mouse0) && ammo > 0)
        {
            if(weapon == null)
            {
                return;
            }

            if (weapon.clipAmmo > 0)
            {
                weapon.clipAmmo -= 1;
                PlayShootAnim();
                RaycastHit hit;
                if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out hit, 1000))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        Enemy enemyScript = hit.transform.GetComponent<Enemy>();
                        enemyScript.DamageReciever(weapon.bulletDamage);
                        
                        ParticleSystem enemyImpactParticle = Instantiate(enemyScript.impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    else
                    {
                        //Instantiate spark effect from the surface hit
                        ParticleSystem impactParticle = Instantiate(weapon.impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                }

                GameObject bullet = Instantiate(weapon.bulletModel, weapon.firingPoint.position, Quaternion.LookRotation(hit.point - weapon.firingPoint.position));
                bullet.transform.localRotation = Quaternion.Euler(0, 90, 0);

            }
            else
            {
                PlayShootBlankAnim();
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
                
            }
            else
            {
                int ammoNeeded = weapon.maxClipAmmo - weapon.clipAmmo;
                // Fill the weapons ammo up to the max ammo for the weapon and take it from the players ammo
                if (ammo > weapon.maxClipAmmo)
                {
                    weapon.clipAmmo = weapon.maxClipAmmo;
                    ammo -= ammoNeeded;
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
                PlayReloadAnim();

            }
        }
    }

    void Kick()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            // Play kick animation
            PlayKickAnim();
        }
        
    }

    public void KickDamage()
    {
        //raycast from the camera if it hits an enemy or a door then trigger a kicked effect
        RaycastHit hit;
        if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out hit, 2f))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                Enemy enemyScript = hit.transform.GetComponent<Enemy>();
                enemyScript.KickReciever();
                ParticleSystem enemyImpactParticle = Instantiate(enemyScript.impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
            }
            else if (hit.transform.CompareTag("Door"))
            {
                // Trigger door kick animation
                PlayPickupAnim();
            }
        }
    }

    void UpdateUI()
    {
        // Update the UI with the players health and ammo
    }

    
    //-------------------- Animation Functions ----------------
    
    void PlayReloadAnim()
    {
        weaponAnim.Play("Reload");
    }
    
    void PlayShootAnim()
    {
        weaponAnim.Play("Shoot");

    }

    void PlayShootBlankAnim()
    {
        weaponAnim.Play("ShootBlank");
    }

    void PlayPickupAnim()
    {
        weaponAnim.Play("PickUp");
    }

    void PlayKickAnim()
    {
        legAnim.Play("Kick");
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
        if (weapon) Destroy(weapon);
        weapon = newWeapon;
        weapon.transform.SetParent(weaponSlot);
        PlayPickupAnim();
    }


}
