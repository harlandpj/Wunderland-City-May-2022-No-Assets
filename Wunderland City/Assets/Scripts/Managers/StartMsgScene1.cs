using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartMsgScene1 : MonoBehaviour
{
    [SerializeField]
    private TMP_Text messageDisplay;

    GameObject player;

    [SerializeField]
    private GameObject exitBarrier;

    // Start is called before the first frame update
    void Start()
    {
        // set has rifle to false
        player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<WunderlandThirdPersonController>().SetHasRifle(false);
        // only disable force field "EXIT" if NOT start menu or Wolf Hills Scenes 0 & 1

        // find force field exit, always need to activate it ONLY when key is found
        exitBarrier.SetActive(false);
        
        messageDisplay.SetText("I AM SURE I LEFT A RIFLE HERE SOMEWHERE...\nAND THE KEY TO WUNDERLAND TOO\nYOU CAN'T GET IN WITHOUT THEM\nPERHAPS YOU CAN FIND THEM?\n YOU SHOULD LOOK HIGH AND LOW!".ToString());
        StartCoroutine("ClearDisplay");
    }

    IEnumerator ClearDisplay()
    {
        yield return new WaitForSeconds(10);
        messageDisplay.SetText("");
    }
}