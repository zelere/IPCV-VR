using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBehaviour : MonoBehaviour
{
    private GameObject collectibles;
    private GameObject player;
    private Canvas canvas;

    // Start is called before the first frame update
    void Start()
    {
        collectibles = transform.Find("Collectibles").gameObject;
        player = transform.Find("Player").gameObject;
        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void StartGame()
    {
        foreach (Transform collectible in collectibles.transform)
        {
            collectible.gameObject.SetActive(true);
        }
        player.GetComponent<BallBehaviour>().enabled = true;
        player.transform.position = new Vector3(0, 0.5f, 0);
        canvas.transform.Find("StartMenu").gameObject.SetActive(false);
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
}
    