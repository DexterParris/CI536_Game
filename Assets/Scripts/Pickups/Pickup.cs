using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public string pickupType;
    public int pickupAmount;
    private bool isPickedUp = false;


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
            PlayerMovement playerMovement = other.transform.GetComponent<PlayerMovement>();
            if (pickupType == "Health")
            {
                playerMovement.PickupHealth(pickupAmount);
            }
            else if (pickupType == "Ammo")
            {
                playerMovement.PickupAmmo(pickupAmount);
            }

            Destroy(gameObject);

        }
    }
}
