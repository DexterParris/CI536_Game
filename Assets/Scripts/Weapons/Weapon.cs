using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public string weaponType;
    public int bulletDamage;
    public float fireRate;
    public int maxClipAmmo;
    public int clipAmmo;
    public ParticleSystem impactParticle;
    public Transform firingPoint;
    private bool isPickedUp = false;
    
    

    private void Update()
    {
        if (!isPickedUp)
        {
            transform.Rotate(Vector3.up * 1.5f);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isPickedUp = true;
            collision.transform.GetComponent<PlayerMovement>().PickupWeapon(this);
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<SphereCollider>());
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.identity;
        }
    }

    
}