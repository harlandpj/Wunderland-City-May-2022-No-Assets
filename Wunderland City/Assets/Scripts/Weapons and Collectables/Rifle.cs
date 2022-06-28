using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rifle : MonoBehaviour
{
    GameObject player;

    [Header("Rifle Audio Clips")]
    [Tooltip("sound used for rifle pickup")]
    [SerializeField]
    private AudioClip rifleCollectedNoise;
    
    private AudioSource audioSource;
    private bool rifleCollected;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource= gameObject.GetComponent<AudioSource>();
        rifleCollected = false;
    }

    private void Update()
    {
        // rotate if not collected
        if (!rifleCollected)
        {
            transform.Rotate(Vector3.forward, 2f);
        }
    }

    private bool rifleCollection = false;

    private void OnTriggerEnter(Collider other)
    {
        // check who entered
        if (other.CompareTag("Player") && !rifleCollection)
        {
            rifleCollection = true;
            // player has rifle
            player.GetComponent<WunderlandThirdPersonController>().SetHasRifle(true);
            audioSource.PlayOneShot(rifleCollectedNoise);

            if (GameManagement.Instance.HasExitKey)
            {
                GameManagement.Instance.ForceField.SetActive(true);
                GameManagement.Instance.PostDisplayMessage("WELL DONE! YOU HAVE THE EXIT KEY, AND A RIFLE!\nYOU CAN GO TO THE NEXT LEVEL..\nCONTINUE YOUR JOURNEY AND BE CAREFUL!");
            }
            else
            {
                GameManagement.Instance.PostDisplayMessage("YOU NEED TO FIND THE EXIT KEY TOO TO GO TO THE NEXT LEVEL..\nAND CONTINUE YOUR JOURNEY!");
            }
            
            Destroy(gameObject, 0.8f);
        }
    }
}
