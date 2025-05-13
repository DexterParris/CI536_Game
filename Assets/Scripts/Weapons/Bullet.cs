using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int currentFrame = 0;
    public Vector3 targetPosition;
    
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if (currentFrame < 10)
        {
            currentFrame += 1;
        }
        else if (currentFrame == 10)
        {
            currentFrame += 1;
            transform.position = targetPosition;
            print(transform.position);
        }
        
    }


}
