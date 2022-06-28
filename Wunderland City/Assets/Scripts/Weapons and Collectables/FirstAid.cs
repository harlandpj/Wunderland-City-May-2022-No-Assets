using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstAid : MonoBehaviour
{
    public bool pickedUp = false;

    private void OnTriggerEnter(Collider other)
    {
        // check who entered
        //if (other.gameObject.CompareTag("Player") && !pickedUp)
        //{
        //    pickedUp = true;
        //    // player has picked it up
        //    GameManagement.Instance.HealthKitsNumber = GameManagement.Instance.HealthKitsNumber + 1;
        //    GameManagement.Instance.PostMinorDisplayMessage("You Collected: Health Kit");
        //    audioSource.Play();
        //    Destroy(gameObject, 1f);
        //}
    }
}
