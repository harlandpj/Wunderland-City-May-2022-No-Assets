using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Singleton Class, can exist in multiple scenes but only one copy ever
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; } // shared by all instances

    GameObject player1;

    [Header("Enemy Prefabs (set in Inspector)")]
    [SerializeField]
    private GameObject[] enemies;

    [Header("Rabbit Spawn Points")]
    [SerializeField]
    private GameObject[] rabbitSpawnPoints;

    [Header("Collectable Items")]
    [SerializeField]
    private GameObject[] pickups;

    [Header("Max Number of Enemies & Pickups Allowed")]
    [SerializeField]
    private int totalAllowed = 100;

    [SerializeField]
    private int totalPickupsAllowed = 150;

    // setup the Singleton and retain data on loading
    private void Awake()
    {
        // check if we have been created already in another scene
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    // Start is called before the first frame update
    void Start()
    {
        player1 = GameObject.Find("Player");
        
        //InvokeRepeating("SpawnRandomEnemy", 5f, 30f);
        InvokeRepeating("SpawnRandomPickup", 30f, 60f);

       // SpawnInitialPickups();
        //SpawnInitialWanderingEnemies();
    }

    private void SpawnInitialPickups()
    {
        for (int i = 0; i < 30; i++)
        {
            SpawnRandomPickup();
        }
    }

    private void SpawnInitialWanderingEnemies()
    {
        // Spawn enemies at random positions, with random patrol points
        // just enough to make it interesting, but not overly difficult!
        for (int i = 0; i < 15; i++)
        {
            SpawnRandomEnemy();
        }
    }

    private int FindTotalEnemies()
    {
        int maxNowOnScreen = GameObject.FindGameObjectsWithTag("SpiderQueen").Length +
            GameObject.FindGameObjectsWithTag("Eyeball").Length +
            GameObject.FindGameObjectsWithTag("CardGuard").Length;
        return maxNowOnScreen;
    }

    private int FindTotalPickups()
    {
        int maxNowOnScreen = GameObject.FindGameObjectsWithTag("FirstAidKit").Length;
        return maxNowOnScreen;
    }

    private void SpawnRandomPickup()
    {
        if (SceneManager.GetActiveScene().buildIndex >=3)
        {
            // must be in Mushroom City onwards
            // spawn position - use random "free of obstacles" area for now - use spawn positions later
            float randX = UnityEngine.Random.Range(500, 530);
            float randZ = UnityEngine.Random.Range(15, 40);

            Vector3 randomPos = new Vector3(randX, 200.1f, randZ);

            // only spawn a maximum amount at any point
            if (GameManagement.Instance.HealthKitsNumber < 10)
            {
                Instantiate(pickups[0], randomPos, pickups[0].transform.rotation);
                GameManagement.Instance.PostMinorDisplayMessage("Spawned a Health Kit!");
                GameManagement.Instance.HealthKitsNumber++;
            }
        }
    }

    private void SpawnRandomEnemy()
    {
        // not taking any notice of potential obstacles in way
        // just spawning for this game

        int maxOnScreen = FindTotalEnemies(); // how many currently on screen
        int randNumber = UnityEngine.Random.Range(0, 3); // select a random enemy

        if (randNumber > 2)
        {
            // put in as never seemed to be two!
            randNumber = 2;
        }

        float randX = UnityEngine.Random.Range(10, 250);
        float randZ = UnityEngine.Random.Range(-150, 150);

        // *********** FOR TESTING - spawn at this position ***********
        randX = -12.35f; // UnityEngine.Random.Range(100, 250);
        randZ = 0; // UnityEngine.Random.Range(-100, 0);
        randNumber = 0;
        // *********** FOR TESTING - spawn at this position ***********


        Vector3 randomPos = new Vector3(randX, 0, randZ);

        // only allow a maximum number of enemies on screen at any point
        if (maxOnScreen < totalAllowed)
        {
            GameObject enemy;
            //enemy = Instantiate(enemies[randNumber], randomPos, Quaternion.identity);
            enemy = Instantiate(enemies[0], randomPos, Quaternion.identity);
            // now set random patrol positions
            // (change later to spawn zones (which must be clear of obstacles)
            // but for now, just inside castle area)

            for (int i = 0; i < 3; i++)
            {
                GameObject toCreate;

                // spawn patrol points
                float randx = UnityEngine.Random.Range(-10, 10);
                float randz = UnityEngine.Random.Range(-10,10);

                Vector3 pos = new Vector3(randx, 0f, randz);

                toCreate = Instantiate(new GameObject(), pos, Quaternion.identity);

                // now set patrol point in correct script
                switch (randNumber)
                {
                    case 0:
                        {
                            SpiderQueen myScriptReference = enemy.GetComponent<SpiderQueen>();
                            myScriptReference.SetupDynamicPatrolPoint(i, toCreate);
                            myScriptReference.SetToFirstPatrolPosition();
                            break;
                        }
                    case 1:
                        {
                            Eyeball myScriptReference = enemy.GetComponent<Eyeball>();
                            //myScriptReference.SetupDynamicPatrolPoint(i, toCreate);
                            //myScriptReference.SetToFirstPatrolPosition();
                            break;
                        }
                    case 2:
                        {
                            CardGuard myScriptReference = enemy.GetComponent<CardGuard>();
                            myScriptReference.SetupDynamicPatrolPoint(i, toCreate);
                            myScriptReference.SetToFirstPatrolPosition();
                            break;
                        }
                }
            }

            Debug.Log($"Finished Setting Patrol points in dynamic enemy: {enemy.tag.ToString()}!");
        }
    }

    // reset enemies to new player
    protected virtual void ResetEnemies()
    {
        BaseEnemy[] enemies = GameObject.FindObjectsOfType<BaseEnemy>();

        foreach (BaseEnemy enemy in enemies)
        {
            enemy.ChangePlayerReference();
        }
    }
}

