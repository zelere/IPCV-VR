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

    // Start is called before the first frame update
    void Start()
    {
        playerStartPosition = player.transform.position;
        playerStartRotation = player.transform.rotation;

        collectibles = GameObject.FindGameObjectsWithTag(collectibleTag);

        playerScript.enabled = false;
        startMenu.SetActive(true);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartGame()
    {
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;


        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(true);
        }

        playerScript.enabled = true;
        startMenu.SetActive(false);
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
    