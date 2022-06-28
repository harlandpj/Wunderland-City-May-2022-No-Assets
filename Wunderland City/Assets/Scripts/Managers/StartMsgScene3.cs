using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class StartMsgScene3 : MonoBehaviour
{
    [SerializeField]
    private TMP_Text messageDisplay;

    // Start is called before the first frame update
    void Start()
    {
        // actually now scene 4 as have a main menu which is scene 0
        messageDisplay.SetText("THAT IS ALL FOLKS... HOPE IT WAS FUN... I RAN OUT OF DEV TIME FOR THIS GAME JAM! MAYBE NEXT TIME!".ToString());
        StartCoroutine("ClearDisplay");
    }

    IEnumerator ClearDisplay()
    {
        yield return new WaitForSeconds(10);
        messageDisplay.SetText("");
    }
}
