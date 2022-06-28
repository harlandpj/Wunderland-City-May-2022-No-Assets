using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartMsgScene0 : MonoBehaviour
{
    [SerializeField]
    private TMP_Text messageDisplay;

    // Start is called before the first frame update
    void Start()
    {

        messageDisplay.SetText("HEY ALICE!\nI'M SO GLAD YOU CAME!\nGET DOWN TO WUNDERLAND... THE VIRUS ESCAPED, JUST FOLLOW THE PATH AND\nWATCH OUT FOR WOLVES!".ToString());
        StartCoroutine("ClearDisplay");
    }

    IEnumerator ClearDisplay()
    {
        yield return new WaitForSeconds(10);
        messageDisplay.SetText("".ToString());
    }
}