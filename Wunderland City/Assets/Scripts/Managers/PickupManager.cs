using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupManager : MonoBehaviour
{
    [Header("Points Awarded")]
    [SerializeField]
    private int pickupPoints;

    [SerializeField]
    private int healthPoints;

    [Header("Audio Clips")]
    [SerializeField]
    AudioClip healthClip;

    [SerializeField]
    AudioClip pointsClip;

    private bool hitByPlayer = false;

    private void OnTriggerEnter(Collider other)
    {
        if (!GameManagement.Instance.bGameOver)
        {
            // check who triggered this - if the player update score & health in game manager        
            if (other.gameObject.CompareTag("Player") && !hitByPlayer)
            {
                StartPickupCollected();
            }
        }
    }

    private void StartPickupCollected()
    {
        // prevent re-collisions giving more points
        hitByPlayer = true;

        // turn collider off for extra security
        gameObject.GetComponent<Collider>().enabled = false;
        StartCoroutine(PickupCollected());
    }

    IEnumerator PickupCollected()
    {
        AudioSource aSource = GetComponent<AudioSource>();

        // play correct noise
        if (gameObject.CompareTag("Pickup1"))
        {
            // play health noise
            aSource.clip = healthClip;
        }
        else
        {
            aSource.clip = pointsClip;
        }

        aSource.Play(); // play collected sound

        GameManagement.Score += pickupPoints;
        GameManagement.Health += healthPoints;

        yield return new WaitForSeconds(1f);
        Destroy(gameObject, 0.1f);
    }
}
