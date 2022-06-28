using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

public class HUD : MonoBehaviour
{
    // HUD display
    [Header("Player Stats")]
    [SerializeField]
    private TMP_Text PlayerScore; // players score
    [SerializeField]
    private TMP_Text PlayerHiScore; // players score
    [SerializeField]
    private TMP_Text PlayerLives; // players lives
    [SerializeField]
    private TMP_Text PlayerHealth; // players health
    [SerializeField]
    private TMP_Text HiPlayerName; // high score players name
    [SerializeField]
    private TMP_Text EnemiesRemaining; // enemies left to kill this level
    [SerializeField]
    private TMP_Text InfoDisplay; // 'general' status display message box
    [SerializeField]
    private TMP_Text MinorMessageDisplay; // minor message (e.g. pickups collection) display
    [SerializeField]
    private TMP_Text RifleClipsNumber; // number of rifle clips held
    [SerializeField]
    private TMP_Text HealthKitsNumber; // number of health kits held
    [SerializeField]
    private TMP_Text NumRabbitsLeft; // number of rabbits left
    
    private GameObject RabbitsLabel;

    // Start is called before the first frame update (singleton, so only called once it seems)
    void Start()
    {
        InvokeRepeating("UpdatePlayerStats", 0, 1);
        MinorMessageDisplay.SetText("".ToString());
        
        string m_Scene = SceneManager.GetActiveScene().name;
        
        if (RabbitsLabel == null)
        {
            RabbitsLabel = GameObject.FindGameObjectWithTag("RabbitsLabel"); 
        }

        if (m_Scene != "Mushroom City - Scene 2")
        {
            
            RabbitsLabel.SetActive(false);
        }
        else
        {
            RabbitsLabel.SetActive(true);
        }
    }

    public void OnGameQuit()
    {
        // quit button action
        GameManagement.Instance.SaveUserData();

#if UNITY_EDITOR
        EditorApplication.ExitPlaymode();
#else
        Application.Quit();
#endif
    }

    private void UpdatePlayerStats()
    {
        // updates player stats, done every second - may use events later!
        Debug.Log("Updating Player Stats");

        PlayerScore.SetText(GameManagement.Score.ToString());
        PlayerHiScore.SetText(GameManagement.HighScore.ToString());
        PlayerLives.SetText(GameManagement.Lives.ToString());
        PlayerHealth.SetText(GameManagement.Health.ToString());
        HiPlayerName.SetText(GameManagement.PlayerName.ToString());

        EnemiesRemaining.SetText(GameManagement.Instance.FindTotalEnemies().ToString());
        RifleClipsNumber.SetText(GameManagement.Instance.RifleClipsNumber.ToString());
        HealthKitsNumber.SetText(GameManagement.Instance.HealthKitsNumber.ToString());

        // only update this field in the Rabbit Scene!
        string m_Scene = SceneManager.GetActiveScene().name;
        if (m_Scene == "Mushroom City - Scene 2")
        {
            RabbitsLabel.SetActive(true);
            NumRabbitsLeft.SetText(GameManagement.Instance.FindTotalRabbits().ToString());
        }
    }
}