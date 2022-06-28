using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartMsgScene2 : MonoBehaviour
{
    [SerializeField]
    private TMP_Text messageDisplay;

    // Start is called before the first frame update
    void Start()
    {

        messageDisplay.SetText("KILL THE GUARDS AND COLLECT THE QUEEN'S RABBITS TO GET THE KEY TO THE NEXT LEVEL!".ToString());
        StartCoroutine("ClearDisplay");
    }

    IEnumerator ClearDisplay()
    {
        yield return new WaitForSeconds(10);
        messageDisplay.SetText("");
    }
}
