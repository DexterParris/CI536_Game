using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    // Start is called before the first frame update
    private void Awake()
    {
        StartCoroutine(Kill());
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += -transform.right * 1f;
    }

    IEnumerator Kill()
    {
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }
}
