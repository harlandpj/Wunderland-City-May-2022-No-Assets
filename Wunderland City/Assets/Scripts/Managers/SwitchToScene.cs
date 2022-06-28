using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToScene : MonoBehaviour
{
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        // switch to specified scene (as determined in build settings scene order)
        SceneManager.LoadScene(2); // Was originally scene 1 in build index but forget start menu!
    }
}
