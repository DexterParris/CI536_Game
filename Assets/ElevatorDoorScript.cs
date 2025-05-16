using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ElevatorDoorScript : MonoBehaviour
{
    public bool openDoor;
    public ElevatorScript elevatorScript;
    public bool levelTransition;
    public string sceneName;


    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            switch (openDoor)
            {
                case true:
                    elevatorScript.OpenDoor();
                    break;
                case false:
                    elevatorScript.CloseDoor();
                    if(levelTransition)
                    {
                        StartCoroutine(changeScene());
                    }
                    break;
            }

        }
    }


    IEnumerator changeScene()
    {
        yield return new WaitForSeconds(1f);


        SceneManager.LoadScene(sceneName);
    }
}
