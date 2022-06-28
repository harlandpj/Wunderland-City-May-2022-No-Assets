using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToScene3 : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (GameManagement.Instance.HasExitKey)
        {
            // switch to specified scene (as determined in build settings scene order)
            SceneManager.LoadScene(4);
        }
        else
        {
            GameManagement.Instance.PostDisplayMessage("YOU DON'T HAVE THE EXIT KEY.... YET!\nYOU HAVE TO KILL 20 ENEMIES TO RECEIVE IT!");
        }
        
    }
}
