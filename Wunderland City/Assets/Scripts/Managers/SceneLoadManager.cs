using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// class used to manage switching scenes, and perform any setup required depending on which
// scene the player is in (from light scene to dark and back again maybe)
public class SceneLoadManager : MonoBehaviour
{
    public static SceneLoadManager Instance { get; private set; } // shared by all instances

    [SerializeField]
    [Header("Skyboxes List (in Scene Order)")]
    [Tooltip("Put Skybox to be used in the order of the first scene to the last!")]
    Material[] skyBoxes;

    [SerializeField]
    [Header("Scene Lighting Type - (main light, enter in Scene order)")]
    [Tooltip("Enter 0,1,2 or 3 for dawn, day, dusk, night respectively!")]
    int[] lightUsedThisScene; // a lighting type e.g. 0 -dawn, 1- day, 2- dusk, 3- night

    private int numberSkyboxes; //  should match number of scenes

    private GameObject player; // player
    private Transform playerTransform; // player transform for positioning

    public static int sceneNumber; // current scene number

    // setup the static object and retain data on loading
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
        // set skybox up
        GenerateSkybox();

        // setup player
        player = GameObject.FindGameObjectWithTag("Player");
        playerTransform = player.transform;
    }

    private void GenerateSkybox()
    {
        // sets skybox for this scene by finding active scene in build index
        // if greater than number of scenes (then it hasn't been added) then sets at a default
        // and then finding the correct lighting setup for it

        Scene activeScene = SceneManager.GetActiveScene();
        sceneNumber = activeScene.buildIndex;

        if (sceneNumber > SceneManager.sceneCountInBuildSettings)
        {
            // doesn't exist in build index, use default
            sceneNumber = 0;
        }

        if (skyBoxes.Length > 0)
        {
            // sets skybox and changes main light to correct light angle/intensity etc
            UnityEngine.RenderSettings.skybox = skyBoxes[sceneNumber];
          //  SetLightIntensity(sceneNumber, lightUsedThisScene[sceneNumber]);
        }
    }

    public static void SetLightIntensity(int sceneNumber, int lightingType)
    {
        // set main light rotation and intensity to simulate different times of day as
        // our scenes will progress from light to dark and back again
        //
        // main directional light angle
        //
        // dawn -40 degrees, intensity 0.9
        // day 50 degrees, intensity 1
        // dusk -73 degrees, intensity 0.6
        // night -88 deg, intensity 0.1


        Quaternion newRotation = new Quaternion();
        float newIntensity = new float();

        // standard rotation of a directional light at time of writing is (50,-30,0)
        switch (lightingType)
        {
            case 0:
                {
                    // dawn
                    newRotation = Quaternion.Euler(new Vector3(-40f, -30f, 0f));
                    newIntensity = 0.9f;
                    break;
                }

            case 1:
                {
                    // day
                    newRotation = Quaternion.Euler(new Vector3(-40f, -30f, 0f));
                    newIntensity = 0.9f;
                    break;
                }


            case 2:
                {
                    // dusk
                    newRotation = Quaternion.Euler(new Vector3(073f, -30f, 0f));
                    newIntensity = 0.6f;
                    break;
                }

            case 3:
                {
                    // night
                    newRotation = Quaternion.Euler(new Vector3(-88f, -30f, 0f));
                    newIntensity = 0.1f;
                    break;
                }

            default:
                {
                    // day is default
                    newRotation = Quaternion.Euler(new Vector3(-40f, -30f, 0f));
                    newIntensity = 0.9f;
                    break;
                }
        }

        GameObject mainLight = GameObject.FindGameObjectWithTag("MainLight");

        // find main light in active scene and reset it
        mainLight.GetComponent<Light>().intensity = newIntensity;
        mainLight.GetComponent<Transform>().rotation = newRotation;
    }

    public void SwitchToScene(int sceneNumber)
    {
        SceneManager.LoadScene(sceneNumber);
    }
}
