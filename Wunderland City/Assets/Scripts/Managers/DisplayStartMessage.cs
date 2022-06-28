using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DisplayStartMessage : MonoBehaviour
{
    int sceneNumber;

    // Start is called before the first frame update
    void Start()
    {
        Scene activeScene = SceneManager.GetActiveScene();
        sceneNumber = activeScene.buildIndex;
        sceneNumber = activeScene.buildIndex;

        if (sceneNumber > SceneManager.sceneCountInBuildSettings)
        {
            // doesn't exist in build index, use default
            sceneNumber = 0;
        }

        

        if (GameManagement.Instance != null)
        {
            switch (sceneNumber)
            {
                //case 0:
                //    GameManagement.Instance.PostDisplayMessage("HEY ALICE!\nI'M SO GLAD YOU CAME!\n " +
                //"GET DOWN TO WUNDERLAND... THE VIRUS ESCAPED, JUST FOLLOW THE PATH AND\nWATCH OUT FOR WOLVES!"); break;
                //case 0:
                //    GameManagement.Instance.PostDisplayMessage(""); break;
                case 2: GameManagement.Instance.PostDisplayMessage("KILL THE GUARDS AND COLLECT THE QUEENS RABBITS TO GET THE KEY TO THE NEXT LEVEL!"); break;
                
                case 3: GameManagement.Instance.PostDisplayMessage("WELCOME TO MY HELL! THERE IS NO ESCAPE!"); break;
                case 4: GameManagement.Instance.PostDisplayMessage("Welcome Player to Scene 5"); break;
                default: break;
            }
        }
        else
        {
            Debug.Log("Game Manager not active yet!");
            StartCoroutine("CheckGameManagerActive");
        }
    }

    public void DisplayNow()
    {
        switch (sceneNumber)
        {
            case 0:
                GameManagement.Instance.PostDisplayMessage("HEY ALICE!\nI'M SO GLAD YOU CAME!\n " +
            "GET DOWN TO WUNDERLAND... THE VIRUS ESCAPED, JUST FOLLOW THE PATH AND\nWATCH OUT FOR WOLVES!"); break;
            case 1:
                GameManagement.Instance.PostDisplayMessage("I'M SURE I LEFT A RIFLE HERE SOMEWHERE...\nAND THE KEY TO WUNDERLAND TOO!\nYOU CAN'T GET IN WITHOUT THEM!\n" +
            "PERHAPS YOU CAN FIND THEM?\n YOU SHOULD LOOK HIGH AND LOW!"); break;
            case 2: GameManagement.Instance.PostDisplayMessage("KILL THE GUARDS AND COLLECT THE QUEENS RABBITS TO GET THE KEY TO THE NEXT LEVEL!"); break;

            case 3: GameManagement.Instance.PostDisplayMessage("WELCOME TO SPIDER QUEEN HELL! THERE IS NO ESCAPE!"); break;
            case 4: GameManagement.Instance.PostDisplayMessage("THE END!"); break;
            default: break;
        }
    }

    IEnumerator CheckGameManagerActive()
    {
        while (GameManagement.Instance != null)
        {
            yield return null;
        }

        DisplayNow();
        yield return null;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
