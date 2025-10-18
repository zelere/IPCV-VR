using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class GameBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour playerScript;


    [SerializeField] private string collectibleTag = "Collectible";

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private GameObject[] collectibles;


    // Tracks how many collectibles have been collected
    private int currentCollectibleIndex = 0;
    private float gameStartTime; // Track when the game starts


    // Start is called before the first frame update
    void Start()
    {
        playerStartPosition = player.transform.position;
        playerStartRotation = player.transform.rotation;

        collectibles = GameObject.FindGameObjectsWithTag(collectibleTag);

        playerScript.enabled = false;
        startMenu.SetActive(true);
        // Deactivate all collectibles before the game starts
        HideAllCollectibles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Hide all collectibles
    private void HideAllCollectibles()
    {
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(false);
        }
    }

    private void StartGame()
    {
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        // Reset sequence
        currentCollectibleIndex = 0;
        // Record start time
        gameStartTime = Time.time;


        // Activate the first collectible
        if (collectibles.Length > 0)
            collectibles[0].SetActive(true);


        playerScript.enabled = true;
        startMenu.SetActive(false);
    }

    // Call this method when a collectible is collected
    public void CollectibleCollected()
    {
        if (currentCollectibleIndex < collectibles.Length)
        {
            collectibles[currentCollectibleIndex].SetActive(false);
        }

        currentCollectibleIndex++;

        if (currentCollectibleIndex < collectibles.Length)
        {
            collectibles[currentCollectibleIndex].SetActive(true);
        }
        else
        {
            // Game finished: compute total time
            float totalTime = Time.time - gameStartTime;
            Debug.Log("--> Game finished! Total time: " + totalTime + " seconds");
        }
    }

    public void StartGameWithHandTracking()
    {
        // Set interaction mode to hand tracking
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.HandTracking);
        
        // Start the game
        StartGame();
        
        Debug.Log("Game started with Hand Tracking interaction");
    }

    public void StartGameWithKeyboard()
    {
        // Set interaction mode to keyboard
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);
        
        // Start the game
        StartGame();
        
        Debug.Log("Game started with Keyboard interaction");
    }

    public void startOver()
    {
        playerScript.enabled = false;
        startMenu.SetActive(true);

    }
}
    