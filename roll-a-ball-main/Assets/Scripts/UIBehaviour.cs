using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    // Start is called before the first frame update
    private TMP_Text text_collectibles;
    private TMP_Text text_win;
    private GameObject start_menu;
    private GameObject game;
    private GameObject collectibles;
    private MenuManager menuManager;
    private ExperimentManager experimentManager;
    private bool gameStarted = false;
    private bool gameCompleted = false;
    
    void Start()
    {
        text_collectibles = transform.Find("text_collectibles").gameObject.GetComponent<TMP_Text>();
        text_win = transform.Find("text_win").gameObject.GetComponent <TMP_Text>();
        start_menu = transform.Find("StartMenu").gameObject;
        game = GameObject.Find("Game");
        collectibles = game.transform.Find("Collectibles").gameObject;
        
        // Try to find the MenuManager component
        menuManager = FindObjectOfType<MenuManager>();
        
        // Try to find the ExperimentManager component
        experimentManager = FindObjectOfType<ExperimentManager>();
        
        // Initially hide the win text
        if (text_win != null)
        {
            text_win.gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Check if game has started (ball behaviour is enabled)
        GameObject player = game.transform.Find("Player").gameObject;
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        gameStarted = ballBehaviour.enabled;
        
        // If game hasn't started, show default message and don't check win condition
        if (!gameStarted)
        {
            if (text_collectibles != null)
            {
                text_collectibles.text = "Press a button to start!";
            }
            return;
        }
        
        // If game completed, don't continue checking
        if (gameCompleted)
        {
            return;
        }
        
        int collectibles_count = 12;
        bool shouldCheckWinCondition = false;
        
        // If using ExperimentManager, get count from it
        if (experimentManager != null)
        {
            // For now, since ExperimentManager is in Step 3.1 (initialization only),
            // we'll just check if there are any active collectibles at all
            List<GameObject> allCollectibles = new List<GameObject>();
            
            // Add tutorial collectibles
            if (experimentManager.GetTutorialCollectibles() != null)
            {
                allCollectibles.AddRange(experimentManager.GetTutorialCollectibles());
            }
            
            // Add experiment collectibles  
            if (experimentManager.GetExperimentCollectibles() != null)
            {
                allCollectibles.AddRange(experimentManager.GetExperimentCollectibles());
            }
            
            // Count active collectibles
            foreach (GameObject collectible in allCollectibles)
            {
                if (collectible != null && collectible.activeSelf)
                {
                    collectibles_count++;
                }
            }
            
            // Only check win condition if we have some collectibles (meaning experiment/tutorial has started)
            shouldCheckWinCondition = allCollectibles.Count > 0;
            
            text_collectibles.text = $"Collectibles left: {collectibles_count}";
        }
        else
        {
            // Legacy mode: count children of collectibles parent
            foreach (Transform child in collectibles.transform)
            {
                if (child.gameObject.activeSelf)
                {
                    collectibles_count++;
                }
            }
            text_collectibles.text = "Collectibles left: " + collectibles_count;
            shouldCheckWinCondition = true; // Always check in legacy mode
        }

        // Handle win condition only if:
        // 1. Game is started
        // 2. We should check win condition (have collectibles setup)
        // 3. All collectibles are collected
        // 4. Game not already completed
        if (gameStarted && shouldCheckWinCondition && collectibles_count == 0 && !gameCompleted) {
            gameCompleted = true; // Prevent multiple triggers
            
            if (text_win != null)
            {
                text_win.gameObject.SetActive(true);
            }
            
            // Use MenuManager if available, otherwise fallback to direct menu activation
            if (menuManager != null)
            {
                menuManager.ShowStartMenu();
            }
            else
            {
                start_menu.SetActive(true);
            }
            
            // Disable ball behaviour to stop the game
            if (ballBehaviour != null)
            {
                ballBehaviour.enabled = false;
            }
            
            Debug.Log("Game completed! All collectibles collected.");
        }
    }
    
    // Method to reset game state when starting a new game
    public void ResetGameState()
    {
        gameStarted = false;
        gameCompleted = false;
        if (text_win != null)
        {
            text_win.gameObject.SetActive(false);
        }
    }
}
