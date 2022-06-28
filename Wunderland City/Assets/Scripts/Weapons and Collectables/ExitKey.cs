using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitKey : MonoBehaviour
{
    GameObject player;

    [Header("Exit Key Audio Clips")]
    [Tooltip("sound used for key pickup")]
    [SerializeField]
    private AudioClip keyCollectedNoise;

    private AudioSource audioSource;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        // rotate if not collected
        transform.Rotate(Vector3.up, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // check who entered
        if (other.CompareTag("Player"))
        {
            // player has key
            GameManagement.Instance.HasExitKey = true;

            if (player.GetComponent<WunderlandThirdPersonController>().hasRifle)
            {
                GameManagement.Instance.ForceField.SetActive(true);
                GameManagement.Instance.PostDisplayMessage("WELL DONE! YOU HAVE THE EXIT KEY, AND A RIFLE!\nYOU CAN GO TO THE NEXT LEVEL..\nCONTINUE YOUR JOURNEY AND BE CAREFUL!");
            }
            else
            {
                GameManagement.Instance.PostDisplayMessage("WELL DONE! YOU HAVE THE EXIT KEY..\nDON'T FORGET YOU NEED A RIFLE AS WELL TO ACTIVATE THE EXIT PORTAL\nAND CONTINUE YOUR JOURNEY!");
            }
            
            audioSource.PlayOneShot(keyCollectedNoise);
            Destroy(gameObject, 1f);
        }
    }
}
