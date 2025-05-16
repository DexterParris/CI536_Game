using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public Vector3 playerPosition;
    public Vector3 playerRotation;


    public static GameManager Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this) 
        { 
            Destroy(this); 
        } 
        else 
        { 
            Instance = this; 
            DontDestroyOnLoad(gameObject);
        } 
    }

    public void storePlayerLocation(Vector3 playerPosition, Vector3 playerRotation)
    {
        this.playerPosition = playerPosition;
        this.playerRotation = playerRotation;
    }

    public Vector3 LoadPlayerPos()
    {
        return playerPosition;
    }

    public Vector3 LoadPlayerRot()
    {
        return playerRotation;
    }
}
