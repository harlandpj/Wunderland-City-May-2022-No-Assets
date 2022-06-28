using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RifleClip : MonoBehaviour
{
    GameObject player;

    [Header("Rifle Kit Sound")]
    [Tooltip("sound used for rifle kit pickup")]
    [SerializeField]
    private AudioClip rifleClipPickupNoise;

    private AudioSource audioSource;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // check who entered
        if (other.CompareTag("Player"))
        {
            // player has picked it up
            player.GetComponent<WunderlandThirdPersonController>().SetRifleClipCollected();
            GameManagement.Instance.RifleClipsNumber++;
            GameManagement.Instance.PostMinorDisplayMessage("You Collected: Rifle Clip");
            audioSource.Play();
            Destroy(gameObject, 1f);
        }
    }
}
