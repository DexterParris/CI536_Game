using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    public string sceneName;
    public GameObject player;
    public bool entrance;

    // Start is called before the first frame update

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !entrance)
        {
            StartCoroutine(LoadNextScene(sceneName));
        }
    }

    private void Start()
    {
        if (entrance)
        {
            if (PlayerPrefs.HasKey("PlayerPosX"))
            {
                player.transform.parent = transform;

                player.transform.localPosition = new Vector3(
                    PlayerPrefs.GetFloat("PlayerPosX"),
                    PlayerPrefs.GetFloat("PlayerPosY"),
                    PlayerPrefs.GetFloat("PlayerPosZ")
                );
                player.transform.localRotation = new Quaternion(
                    PlayerPrefs.GetFloat("PlayerRotX"),
                    PlayerPrefs.GetFloat("PlayerRotY"),
                    PlayerPrefs.GetFloat("PlayerRotZ"),
                    PlayerPrefs.GetFloat("PlayerRotW")
                );
                player.transform.parent = null;


                PlayerPrefs.DeleteKey("PlayerPosX");
                PlayerPrefs.DeleteKey("PlayerPosY");
                PlayerPrefs.DeleteKey("PlayerPosZ");
                PlayerPrefs.DeleteKey("PlayerRotX");
                PlayerPrefs.DeleteKey("PlayerRotY");
                PlayerPrefs.DeleteKey("PlayerRotZ");
                PlayerPrefs.DeleteKey("PlayerRotW");


            }
        }
        
    }

    IEnumerator LoadNextScene(string sceneName)
    {

        print("Loading scene: " + sceneName);
        yield return new WaitForSeconds(1f);
        player.transform.parent = transform;
        
        PlayerPrefs.SetFloat("PlayerPosX", player.transform.localPosition.x);
        PlayerPrefs.SetFloat("PlayerPosY", player.transform.localPosition.y);
        PlayerPrefs.SetFloat("PlayerPosZ", player.transform.localPosition.z);
        PlayerPrefs.SetFloat("PlayerRotX", player.transform.localRotation.x);
        PlayerPrefs.SetFloat("PlayerRotY", player.transform.localRotation.y);
        PlayerPrefs.SetFloat("PlayerRotZ", player.transform.localRotation.z);
        PlayerPrefs.SetFloat("PlayerRotW", player.transform.localRotation.w);

        player.transform.parent = null;
        SceneManager.LoadScene(sceneName);


    }
}
