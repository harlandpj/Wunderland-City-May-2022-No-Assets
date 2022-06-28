using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

// SINGLETON class - only EVER one instance of it across all scenes
//
public class GameManagement : MonoBehaviour
{
    private static GameManagement _instance;

    public static GameManagement Instance
    {
        get
        {
            if (_instance == null)
            {
                GameObject go = new GameObject("GameManagement");
                go.AddComponent<GameManagement>();

                GameManagement gameScript = go.GetComponent<GameManagement>();
                gameScript.SetupVarsDynamic();
            }
            return _instance;
        }
    } 
    
    public Vector3 SpawnLocation = new Vector3(0,0,0);

    // player stats
    public static string PlayerName;
    public static int HighScore;
    public static int Score;
    public static int enemiesLeft;

    [Header("Player Stats")]
    [SerializeField]
    private static int maxHealth = 100;
    private static int m_Health;

    public static int Health
    {
        get => m_Health;
        set
        {
            if (value >= 0)
            {
                if (value <= maxHealth)
                {
                    m_Health = value; // increase health
                }
                else
                {
                    m_Health = maxHealth;
                }
            }
            else
            {
                if (value <= 0)
                {
                    m_Health = 0;
                }
                else
                {
                    m_Health = value;
                }
            }
        }
    }

    [Header("Maximum Lives")]
    [SerializeField]
    private static readonly int maxLives = 3;  // maximum lives

    private static int m_Lives;
    public static int Lives
    {
        get => m_Lives;
        set
        {
            if (value >= 0)
            {
                if (value <= maxLives)
                {
                    m_Lives = value;
                }
                else
                {
                    m_Lives = maxLives;
                }
            }
            else
            {
                if (value <= 0)
                {
                    m_Lives = 0;
                }
                else
                {
                    m_Lives = value;
                }
            }
        }
    }

    [SerializeField]
    public int RifleClipsNumber;
    [SerializeField]
    public int HealthKitsNumber;
    [SerializeField]
    public bool HasExitKey;

    public GameObject ForceField;

    [SerializeField]
    private TMP_Text messageDisplay; // 'general' status display message box
    [SerializeField]
    private TMP_Text minorMessageDisplay; // minor/pickups message box display

    // audio
    public static AudioSource audioSource;

    [Header("Game Manager Sounds")]
    [SerializeField]
    private  AudioClip lifeLost;
    [SerializeField]
    private  AudioClip gameOver;
    [SerializeField]
    private  AudioClip pickupCollected;


    public void SetupVarsDynamic()
    {
        // setup the required fields given this was created dynamically this time
        GameObject theMessageDisplay = GameObject.FindGameObjectWithTag("MessageDisplay"); 
        messageDisplay = theMessageDisplay.GetComponent<TMP_Text>();

        GameObject theMinorDisplay = GameObject.FindGameObjectWithTag("MinorMessageDisplay");
        minorMessageDisplay = theMessageDisplay.GetComponent<TMP_Text>();

        if (SceneManager.GetActiveScene().buildIndex > 1)
        {
            // find force field exit, always need to activate it ONLY when key is found
            ForceField = GameObject.FindGameObjectWithTag("ExitToNextScene");
            ForceField.SetActive(false);
            audioSource = gameObject.GetComponent<AudioSource>();
        }

        // now set audio clips
        lifeLost = Resources.Load<AudioClip>("Audio/SoundFX/lose_a_life_23687");
        gameOver = Resources.Load<AudioClip>("Audio/SoundFX/game_over_003_23670");
    }

    public void PostDisplayMessage(string message)
    {
        messageDisplay.SetText(message.ToString());
        StopCoroutine("RemoveDisplayedMessage"); 
        StartCoroutine("RemoveDisplayedMessage");
    }

    public void PostMinorDisplayMessage(string message)
    {
        minorMessageDisplay.SetText(message.ToString());
        StopCoroutine("RemoveMinorDisplayedMessage");
        StartCoroutine("RemoveMinorDisplayedMessage");
    }

    IEnumerator RemoveDisplayedMessage() 
    {
        yield return new WaitForSeconds(10f);
        messageDisplay.SetText("".ToString());
    }

    IEnumerator RemoveMinorDisplayedMessage()
    {
        yield return new WaitForSeconds(3f);
        if (minorMessageDisplay != null)
        {
            minorMessageDisplay.SetText("".ToString());
        }
    }

    [SerializeField]
    private int HealthKitsValue = 70;

    private void UseHealthKit()
    {
        Health += HealthKitsValue;
        PostMinorDisplayMessage("HEALTH LOW: USED AUTO HEALTH KIT");
        HealthKitsNumber--;
        audioSource.PlayOneShot(pickupCollected);
    }

    private void UseRifleClip()
    {
        RifleClipsNumber--;
        PostMinorDisplayMessage("RIFLE: AUTO-REFILLED!");
        audioSource.PlayOneShot(pickupCollected);
    }

    public void ReducePlayerHealth(int amountDamage)
    {
        if (!GameManagement.Instance.bGameOver)
        {
            if (Health < 50 && Health >30)
            {
                PostMinorDisplayMessage("Health Low!");
            }

            if (Health - amountDamage <= 30)
            {
                if (HealthKitsNumber > 0)
                {
                    UseHealthKit(); // increase player health
                }
            }

            if (Health - amountDamage <= 0) 
            {
                PlayLifeLost();

                // life lost
                if (Lives == 0) 
                { 
                    Health = 0; 
                }
            }
            else
            {
                Health -= amountDamage;
            }
            
            Debug.Log("Health is: " + Health);
            CheckGameOver();
        }
    }

    private void PlayLifeLost()
    {
        Lives--;
        audioSource.PlayOneShot(lifeLost, 1f);
        Health = 100;
    }

    // master game over checked elsewhere too
    public bool bGameOver = false;

    public void CheckGameOver()
    {
        if (!bGameOver)
        {
            // check lives left
            if (Lives <= 0)
            {
                bGameOver = true;
                SaveUserData();

                // for final game over panel if done
                //SceneManager.LoadScene(6, LoadSceneMode.Additive);
                //SceneManager.LoadScene(6);

                PostDisplayMessage("GAME OVER!\nTHANKS FOR PLAYING!\nRETURNING TO MAIN MENU in 10 Seconds!");
                audioSource.clip = gameOver;
                audioSource.PlayDelayed(2f);
                StartCoroutine("BackToMainMenu");
            }
        }
    }

    IEnumerator BackToMainMenu()
    {
        yield return new WaitForSeconds(10f);
        SceneManager.LoadScene(0);
    }
        
    public void IncreasePlayerHealth(int amountDamage)
    {
        // remember, when adding health it should be a positive amount
        Health += amountDamage;
    }

  
    // setup the static object and retain data on loading
    private void Awake()
    {
        // check if we have been created already in another scene
        _instance = this;
        DontDestroyOnLoad(gameObject);
        
        LoadUserData();

        Lives = maxLives;
        Health = maxHealth;
        HasExitKey = false;
     
        // only disable force field "EXIT" if NOT start menu or Wolf Hills Scenes 0 & 1
        if (SceneManager.GetActiveScene().buildIndex >1)
        {
            // find force field exit, always need to activate it ONLY when key is found
            ForceField = GameObject.FindGameObjectWithTag("ExitToNextScene");
            ForceField.SetActive(false);
            audioSource = gameObject.GetComponent<AudioSource>();
        }
        else audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void Start()
    {
        // load again
        LoadUserData();
    }

    public void RemoveAnEnemy()
    {
        enemiesLeft = FindTotalEnemies();
        enemiesLeft -=1; // remove one as we just killed one

        PostMinorDisplayMessage("Enemy Killed!");

        if (enemiesLeft == 0)
        {
            if (rabbitsLeft == 0)
            {
                PostDisplayMessage("WELL DONE, YOU GOT EVERY ENEMY\nAND COLLECTED ALL THE RABBITS!\nTHE GAME IS OVER!\nRETURNING TO THE MAIN MENU IN 10S!");
                bGameOver = true;
                SaveUserData();
                StopAllCoroutines();
                audioSource.clip = gameOver;
                audioSource.PlayDelayed(2f);
                StartCoroutine("BackToMainMenu");
            }
            else
            {
                PostDisplayMessage("WELL DONE, YOU GOT EVERY ENEMY!\nBUT YOU STILL HAVE TO FIND " + rabbitsLeft +" RABBIT(S)!");
            }
        }
    }

    public int rabbitsLeft;
    public AudioClip success;

    public void RemoveARabbit()
    {
        rabbitsLeft = FindTotalRabbits();
        rabbitsLeft -= 1; // remove one as we killed one, and needs to check for any spawned ones

        if (rabbitsLeft == 0)
        {
            PostDisplayMessage("WELL DONE, YOU GOT EVERY SINGLE FLUFFY BUNNY RABBIT!");
            audioSource.PlayOneShot(success);
        }
        else
        {
            PostMinorDisplayMessage("YOU GOT A RABBIT!");
        }
    }

    public int FindTotalRabbits()
    {
        int totalRabbits = GameObject.FindGameObjectsWithTag("Rabbit").Length;
        return totalRabbits;
    }

    public int FindTotalEnemies()
    {
        int totalEnemies = GameObject.FindGameObjectsWithTag("SpiderQueen").Length +
            GameObject.FindGameObjectsWithTag("CardGuard").Length;
        return totalEnemies;
    }

    [System.Serializable]
    class SaveData
    {
        public string PlayName; // players name
        public int Score; // players high score
    }

    public void SaveUserData()
    {
        SaveData data = new SaveData();

        if (Score > HighScore)
        {
            // new high score
            data.Score = Score;

            if (PlayerName != null)
            {
                if (PlayerName.Length != 0)
                {
                    data.PlayName = PlayerName;
                }
            }
            else
            {
                data.PlayName = "No Name";
            }
        }
        else
        {
            data.Score = HighScore;
            data.PlayName = PlayerName;
        }

        // convert to JSON format and save to file
        string json = JsonUtility.ToJson(data);
        File.WriteAllText(Application.persistentDataPath + "/savefile.json", json);
        Debug.Log($"Saving Data to: {Application.persistentDataPath} in savefile.json");
    }

    public void LoadUserData()
    {
        string path = Application.persistentDataPath + "/savefile.json";

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            PlayerName = data.PlayName;
            HighScore = data.Score;
            Debug.Log($"Loading Data from: {Application.persistentDataPath} in savefile.json");
        }
    }
}
