using System.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    // Player Variables
    [Header("Player Variables")]
    public float health = 100f;
    public float maxHealth = 100f;
    public CharacterController characterController;
    bool isDying = false;

    // Camera Variables
    public Transform cameraTrans;
    public float cameraSensitivity = 1f;

    // Movement Variables
    [Header("Movement Variables")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    private float currentMoveSpeed = 5f;
    private float cameraPitch = 0f;
    private float yRot = 0f;
    private bool jumpQueued = false;
    private float jumpQueueTime = 0.2f;
    private float jumpQueueTimer = 0f;
    [SerializeField] private float bHopMaxBoost = 5f;
    [SerializeField] private float bHopBoost = 0f;
    [SerializeField] private float bHopTimer = 0f;
    public Transform lastCheckpoint;
    

    // UI
    [Header("UI Variables")]
    public UnityEngine.UI.Slider healthBar;
    public UnityEngine.UI.Slider ammoBar;
    public CanvasGroup deathScreen;

    // Gravity
    private float gravity = -9.81f;
    private Vector3 velocity;

    // Weapon Variables
    [Header("Weapon Variables")]
    public Weapon weapon;
    public int maxAmmo = 12;
    public int ammo = 0;
    public Transform weaponSlot;
    public Transform legObject;
    private Animator weaponAnim;
    private Animator legAnim;

    void Start()
    {
        ammo = maxAmmo;
        weaponAnim = weaponSlot.GetComponent<Animator>();
        legAnim = legObject.GetComponent<Animator>();
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        lastCheckpoint = transform;
    }

    void Update()
    {
        HandleJumpInput();
        Shoot();
        Kick();
        Reload();
        UpdateUI();
        Move();
        Look();

    }

    void LateUpdate()
    {
    }

    //-------------------- Movement --------------------
    void Move()
    {
        bHopTimer -= Time.deltaTime;
        bHopTimer = Mathf.Clamp(bHopTimer, 0, 2f);
        if (Input.GetKey(KeyCode.LeftShift))
            currentMoveSpeed = moveSpeed * 1.3f + bHopBoost;
        else
            currentMoveSpeed = moveSpeed + bHopBoost;

        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        // Do gravity stuff
        if (characterController.isGrounded)
        {
            if (jumpQueued)
            {
                velocity.y = jumpForce;
                jumpQueued = false;
                bHopBoost += 1f;
                bHopBoost = Mathf.Clamp(bHopBoost, 0, bHopMaxBoost);
                bHopTimer = 1f;
            }
            else
            {
                if (bHopTimer <= 0) bHopBoost = 0;
            }
        }
        else
        {
            velocity.y += gravity * Time.deltaTime;

        }

        Vector3 fullMove = (move * currentMoveSpeed) + new Vector3(0, velocity.y, 0);
        characterController.Move(fullMove * Time.deltaTime);

        // Reset jump queue if it expires
        if (jumpQueued)
        {
            jumpQueueTimer -= Time.deltaTime;
            if (jumpQueueTimer <= 0f)
                jumpQueued = false;
        }
    }

    void HandleJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpQueued = true;
            jumpQueueTimer = jumpQueueTime;
        }
    }

    //-------------------- Camera --------------------
    void Look()
    {
        yRot = Input.GetAxis("Mouse X") * Time.deltaTime * cameraSensitivity;
        float xRot = Input.GetAxis("Mouse Y") * Time.deltaTime * cameraSensitivity;

        cameraPitch -= xRot;
        cameraPitch = Mathf.Clamp(cameraPitch, -90f, 90f);

        float targetRoll = 0f;
        if (Input.GetKey(KeyCode.A)) targetRoll = 2f;
        else if (Input.GetKey(KeyCode.D)) targetRoll = -2f;

        float currentRoll = cameraTrans.localRotation.eulerAngles.z;
        if (currentRoll > 180f) currentRoll -= 360f;
        float smoothRoll = Mathf.Lerp(currentRoll, targetRoll, Time.deltaTime * 10f);

        cameraTrans.localRotation = Quaternion.Euler(cameraPitch, 0f, smoothRoll);
        transform.Rotate(Vector3.up * yRot);
    }

    //-------------------- Combat --------------------
    void Shoot()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && ammo > 0)
        {
            if (weapon == null) return;
            if (weapon.clipAmmo > 0)
            {
                weapon.clipAmmo--;
                PlayShootAnim();

                GameObject bullet = Instantiate(weapon.bulletModel, weapon.firingPoint.position,
                    quaternion.identity);

                RaycastHit hit;
                if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out hit, 1000))
                {
                    if (hit.transform.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<Enemy>()?.DamageReciever(weapon.bulletDamage);
                        Instantiate(hit.transform.GetComponent<Enemy>().impactParticle, hit.point,
                            Quaternion.LookRotation(hit.normal));
                    }
                    else if (hit.transform.CompareTag(("EnemyHead")))
                    {
                        hit.transform.GetComponent<Enemy>()?.DamageReciever(weapon.bulletDamage * 10);
                        Instantiate(hit.transform.GetComponent<Enemy>().impactParticle, hit.point,
                            Quaternion.LookRotation(hit.normal));
                    }
                    else
                    {
                        Instantiate(weapon.impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
                    }
                    bullet.GetComponent<Bullet>().targetPosition = hit.point;
                }
                else
                {
                    bullet.GetComponent<Bullet>().targetPosition = cameraTrans.position + cameraTrans.forward * 50f;
                }

            }
            else
            {
                PlayShootBlankAnim();
            }
        }
        else if (Input.GetKeyDown(KeyCode.Mouse0) && ammo <= 0)
        {
            PlayShootBlankAnim();
        }
    }

    void Reload()
    {
        if (!Input.GetKeyDown(KeyCode.R) || weapon == null) return;

        if (weapon.clipAmmo == weapon.maxClipAmmo) return;

        int needed = weapon.maxClipAmmo - weapon.clipAmmo;

        if (ammo <= 0) return;

        if (ammo >= needed)
        {
            weapon.clipAmmo = weapon.maxClipAmmo;
            ammo -= needed;
        }
        else
        {
            weapon.clipAmmo += ammo;
            ammo = 0;
        }

        PlayReloadAnim();
    }

    void Kick()
    {
        if (Input.GetKeyDown(KeyCode.Q))
            PlayKickAnim();
    }

    public void KickDamage()
    {
        if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out RaycastHit hit, 2f))
        {
            if (hit.transform.CompareTag("Enemy"))
            {
                hit.transform.GetComponent<Enemy>()?.KickReciever(hit.transform);
                Instantiate(hit.transform.GetComponent<Enemy>().impactParticle, hit.point, Quaternion.LookRotation(hit.normal));
            }


            Rigidbody rb = hit.transform.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 dir = (hit.transform.position - transform.position).normalized;
                rb.AddForce(dir * 5000f * Time.deltaTime, ForceMode.Impulse);
            }
        }
    }

    void Respawn()
    {
        Time.timeScale = 1f;
        transform.position = lastCheckpoint.position;
        transform.rotation = lastCheckpoint.rotation;

        health = maxHealth;
        ammo = maxAmmo;

        if(weapon != null) weapon.clipAmmo = weapon.maxClipAmmo;
        deathScreen.alpha = 0;

        isDying = false;
    }

    IEnumerator Die()
    {
        print("You Died!");

        // Slow down the game
        Time.timeScale = 0.1f;

        while (deathScreen.alpha < 1)
        {
            deathScreen.alpha += 0.005f;
            yield return new WaitForSeconds(0.0005f);
        }

        yield return new WaitForSeconds(0.2f);
        Respawn();
    }

    

    public void DamageReciever(float damage)
    {
        health -= damage;
        if (health <= 0)
        {
            if(!isDying)
            {
                isDying = true;
                StartCoroutine(Die());

            }
        }
    }


    //-------------------- UI & Pickups --------------------
    void UpdateUI() 
    { 
        healthBar.value = health / maxHealth;

        if (weapon == null)
        {
            ammoBar.value = 0;
        }
        else
        {
            if (weapon.clipAmmo == 0)
                ammoBar.value = 0;
            else
                ammoBar.value = (float)weapon.clipAmmo / (float)weapon.maxClipAmmo;
        }

    }


    void PlayReloadAnim()
    {
        weaponAnim.CrossFade("Reload", 0.1f);
        if (weapon.reloadSoundTrigger != null)
            weapon.reloadSoundTrigger.TriggerSounds();
    }
    void PlayShootAnim()
    {
        weaponAnim.CrossFade("Shoot", 0.1f);
        if (weapon.shootSoundTrigger != null)
            weapon.shootSoundTrigger.TriggerSounds();
    }
    void PlayShootBlankAnim()
    {
        weaponAnim.CrossFade("ShootBlank", 0.1f);
        if (weapon.emptySoundTrigger != null)
            weapon.emptySoundTrigger.TriggerSounds();
    }
    void PlayPickupAnim()
    {
        weaponAnim.CrossFade("PickUp", 0.1f);
        if (weapon.pickupSoundTrigger != null)
            weapon.pickupSoundTrigger.TriggerSounds();
    }
    void PlayKickAnim() => legAnim.CrossFade("Kick", 0.1f);

    public void PickupAmmo(int amount)
    {
        ammo = Mathf.Min(ammo + amount, maxAmmo);
    }

    public void PickupHealth(float amount)
    {
        health = Mathf.Min(health + amount, maxHealth);
    }

    public void PickupWeapon(Weapon newWeapon)
    {
        if (weapon) Destroy(weapon);
        weapon = newWeapon;
        weapon.transform.SetParent(weaponSlot);
        PlayPickupAnim();
    }
}
