using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartMenu : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    // start pressed
    public void OnStartButton()
    {
        Debug.Log("Start!");
        SceneManager.LoadScene(1);
    }

    // creduts pressed
    public void OnCreditsButton()
    {
        Debug.Log("Credits!");
        SceneManager.LoadScene(5);
    }

    // creduts pressed
    public void OnQuitButton()
    {
        Debug.Log("Quit!");
        Application.Quit();
    }

}
