using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponType;
    public int bulletDamage;
    public float fireRate;
    public int maxClipAmmo;
    public int clipAmmo;
    public ParticleSystem impactParticle;


    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.GetComponent<PlayerMovement>().PickupWeapon(this);
        }
    }
}