using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public Vector3 targetPosition;
    
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(GetComponent<TrailRenderer>(), 1.99f);
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPosition, 100f * Time.deltaTime);

    }


}
