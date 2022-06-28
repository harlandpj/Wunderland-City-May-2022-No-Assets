using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDefaultPlayerPosition : MonoBehaviour
{
    [SerializeField]
    private Vector3 defaultPosition; // default position for player

    // setup the default player position
    private void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = new Vector3(defaultPosition.x, defaultPosition.y, defaultPosition.z);
        //player.transform.rotation = Quaternion.identity;
    }
}
