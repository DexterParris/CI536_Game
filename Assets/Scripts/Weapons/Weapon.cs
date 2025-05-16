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
    public GameObject bulletModel;
    public SoundTrigger shootSoundTrigger;
    public SoundTrigger pickupSoundTrigger;
    public SoundTrigger reloadSoundTrigger;
    public SoundTrigger emptySoundTrigger;


    private void Update()
    {
        if (!isPickedUp)
        {
            transform.Rotate(Vector3.up * Time.deltaTime * 100);
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            isPickedUp = true;
            Destroy(GetComponent<Rigidbody>());
            Destroy(GetComponent<SphereCollider>());
            other.transform.GetComponent<PlayerMovement>().PickupWeapon(this);
            transform.localPosition = new Vector3(0, 0, 0);
            transform.localRotation = Quaternion.identity;
        }
    }
    
}