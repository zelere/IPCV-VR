using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class GameBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject tutorialCompleteMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour playerScript;
    [SerializeField] private TutorialMenuManager tutorialMenuManager;


    [SerializeField] private string collectibleTag = "Collectible";
    [SerializeField] private string tutorialCollectibleTag = "TutorialCollectible";

    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private GameObject[] collectibles;
    private GameObject[] tutorialCollectibles;
    
    // Game phases
    public enum GamePhase { Menu, Tutorial, Study }
    public GamePhase currentPhase = GamePhase.Menu;
    
    // Control mode for automatic selection
    private BallBehaviour.InteractionMode selectedControlMode;

    // Awake is called before Start to ensure proper initialization
    void Awake()
    {
        // Force tutorial complete menu to be hidden immediately
        if (tutorialCompleteMenu != null)
            tutorialCompleteMenu.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerStartPosition = player.transform.position;
        playerStartRotation = player.transform.rotation;

        collectibles = GameObject.FindGameObjectsWithTag(collectibleTag);
        tutorialCollectibles = GameObject.FindGameObjectsWithTag(tutorialCollectibleTag);

        if (tutorialMenuManager == null)
            tutorialMenuManager = FindFirstObjectByType<TutorialMenuManager>();

        // Ensure proper initial state
        playerScript.enabled = false;
        startMenu.SetActive(true);
        
        // Always ensure tutorial complete menu starts hidden
        if (tutorialCompleteMenu != null)
            tutorialCompleteMenu.SetActive(false);
        else
        {
            Debug.LogWarning("TutorialCompleteMenu reference is not assigned in GameBehaviour! Please assign it in the Inspector.");
            // Try to find it automatically
            TryFindTutorialCompleteMenu();
        }
        
        // Debug collectibles setup
        Debug.Log($"Found {collectibles.Length} study collectibles (should be 12)");
        Debug.Log($"Found {tutorialCollectibles.Length} tutorial collectibles (should be 4)");
        
        if (tutorialCollectibles.Length != 4)
        {
            Debug.LogWarning($"Expected 4 tutorial collectibles but found {tutorialCollectibles.Length}. Make sure GameObjects are tagged with '{tutorialCollectibleTag}'");
            
            if (tutorialCollectibles.Length < 4)
            {
                Debug.Log("Attempting to create missing tutorial collectibles automatically...");
                CreateMissingTutorialCollectibles();
            }
        }
            
        // Hide study collectibles initially
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(false);
        }
        
        // Hide tutorial collectibles initially
        foreach (GameObject tutorialCollectible in tutorialCollectibles)
        {
            tutorialCollectible.SetActive(false);
        }
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

    public void StartTutorial()
    {
        currentPhase = GamePhase.Tutorial;
        
        // Reset player position
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Show tutorial collectibles (4 corners)
        foreach (GameObject tutorialCollectible in tutorialCollectibles)
        {
            tutorialCollectible.SetActive(true);
        }
        
        // Hide study collectibles
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(false);
        }

        // For tutorial, let's use a default interaction mode or allow manual selection
        // You can modify this logic based on your study requirements
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard); // Default for tutorial
        
        // Ensure player script is enabled
        playerScript.enabled = true;
        startMenu.SetActive(false);
        
        Debug.Log("Tutorial phase started (default) - Player control enabled");
    }
    
    public void StartTutorialWithHandTracking()
    {
        currentPhase = GamePhase.Tutorial;
        
        // Reset player position
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Show tutorial collectibles (4 corners)
        foreach (GameObject tutorialCollectible in tutorialCollectibles)
        {
            tutorialCollectible.SetActive(true);
        }
        
        // Hide study collectibles
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(false);
        }

        // Set interaction mode to hand tracking for tutorial
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.HandTracking);
        
        // Ensure player script is enabled
        playerScript.enabled = true;
        startMenu.SetActive(false);
        
        Debug.Log("Hand Tracking Tutorial phase started - Player control enabled");
    }
    
    public void StartTutorialWithKeyboard()
    {
        currentPhase = GamePhase.Tutorial;
        
        // Reset player position
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Show tutorial collectibles (4 corners)
        foreach (GameObject tutorialCollectible in tutorialCollectibles)
        {
            tutorialCollectible.SetActive(true);
        }
        
        // Hide study collectibles
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(false);
        }

        // Set interaction mode to keyboard for tutorial
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);
        
        // Ensure player script is enabled
        playerScript.enabled = true;
        startMenu.SetActive(false);
        
        Debug.Log("Keyboard Tutorial phase started - Player control enabled");
    }
    
    public void OnTutorialComplete()
    {
        // Automatically select control mode for the study
        // This is where you can implement your automatic selection logic
        // For now, I'll randomly select between the two modes
        int randomMode = Random.Range(0, 2);
        selectedControlMode = randomMode == 0 ? BallBehaviour.InteractionMode.Keyboard : BallBehaviour.InteractionMode.HandTracking;
        
        // Disable player control temporarily while showing completion menu
        playerScript.enabled = false;
        Debug.Log("Tutorial completed - Player control disabled temporarily");
        
        // Use TutorialMenuManager to show the menu
        if (tutorialMenuManager != null)
        {
            string modeName = selectedControlMode == BallBehaviour.InteractionMode.Keyboard ? "Keyboard" : "Hand Tracking";
            tutorialMenuManager.UpdateSelectedModeText(modeName);
            tutorialMenuManager.ShowTutorialCompleteMenu();
        }
        else
        {
            // Fallback if TutorialMenuManager is not available
            if (tutorialCompleteMenu != null)
            {
                tutorialCompleteMenu.SetActive(true);
            }
            Debug.LogWarning("TutorialMenuManager reference is not assigned in GameBehaviour!");
        }
        
        Debug.Log($"Tutorial completed. Selected control mode for study: {selectedControlMode}");
    }
    
    public void StartStudy()
    {
        currentPhase = GamePhase.Study;
        
        // Reset player position
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        // Hide tutorial collectibles
        foreach (GameObject tutorialCollectible in tutorialCollectibles)
        {
            tutorialCollectible.SetActive(false);
        }
        
        // Show study collectibles (12 positions)
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(true);
        }

        // Apply the automatically selected control mode
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(selectedControlMode);
        
        // Re-enable player control for study phase
        playerScript.enabled = true;
        Debug.Log($"Study phase started with {selectedControlMode} interaction mode - Player control enabled");
        
        // Use TutorialMenuManager to hide the menu
        if (tutorialMenuManager != null)
        {
            tutorialMenuManager.HideTutorialCompleteMenu();
        }
        else if (tutorialCompleteMenu != null)
        {
            tutorialCompleteMenu.SetActive(false);
        }
        
        Debug.Log($"Study phase started with {selectedControlMode} interaction mode");
    }

    public void startOver()
    {
        currentPhase = GamePhase.Menu;
        
        // Reset player state
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        
        // Disable player control when in menu
        playerScript.enabled = false;
        startMenu.SetActive(true);
        Debug.Log("Returned to menu - Player control disabled");
        
        // Use TutorialMenuManager to hide the menu
        if (tutorialMenuManager != null)
        {
            tutorialMenuManager.HideTutorialCompleteMenu();
        }
        else if (tutorialCompleteMenu != null)
        {
            tutorialCompleteMenu.SetActive(false);
        }
        
        // Hide all collectibles
        foreach (GameObject collectible in collectibles)
        {
            collectible.SetActive(false);
        }
        
        foreach (GameObject tutorialCollectible in tutorialCollectibles)
        {
            tutorialCollectible.SetActive(false);
        }
    }
    
    public bool IsTutorialPhase()
    {
        return currentPhase == GamePhase.Tutorial;
    }
    
    public bool IsStudyPhase()
    {
        return currentPhase == GamePhase.Study;
    }
    
    private void TryFindTutorialCompleteMenu()
    {
        if (tutorialCompleteMenu == null)
        {
            // Try to find by name
            GameObject menuObj = GameObject.Find("TutorialCompleteMenu");
            if (menuObj == null)
                menuObj = GameObject.Find("Tutorial Complete Menu");
            if (menuObj == null)
                menuObj = GameObject.Find("CompleteMenu");
                
            if (menuObj != null)
            {
                tutorialCompleteMenu = menuObj;
                tutorialCompleteMenu.SetActive(false);
                Debug.Log("Found TutorialCompleteMenu automatically: " + menuObj.name);
            }
            else
            {
                Debug.LogError("Could not find TutorialCompleteMenu GameObject. Please create it or assign the reference in the Inspector.");
                Debug.LogError("Expected GameObject names: 'TutorialCompleteMenu', 'Tutorial Complete Menu', or 'CompleteMenu'");
            }
        }
    }
    
    private void CreateMissingTutorialCollectibles()
    {
        // Find existing collectible to use as template
        GameObject template = null;
        if (collectibles.Length > 0)
        {
            template = collectibles[0];
        }
        else if (tutorialCollectibles.Length > 0)
        {
            template = tutorialCollectibles[0];
        }
        
        if (template == null)
        {
            Debug.LogError("No collectible template found! Cannot create tutorial collectibles automatically.");
            return;
        }
        
        // Define positions for 4 tutorial collectibles (corners of the arena)
        Vector3[] positions = new Vector3[]
        {
            new Vector3(-8f, 0.5f, -8f),  // Bottom-left
            new Vector3(8f, 0.5f, -8f),   // Bottom-right
            new Vector3(-8f, 0.5f, 8f),   // Top-left
            new Vector3(8f, 0.5f, 8f)     // Top-right
        };
        
        int created = 0;
        for (int i = tutorialCollectibles.Length; i < 4; i++)
        {
            // Create new tutorial collectible
            GameObject newTutorialCollectible = Instantiate(template);
            newTutorialCollectible.name = $"TutorialCollectible_{i + 1}";
            newTutorialCollectible.tag = tutorialCollectibleTag;
            newTutorialCollectible.transform.position = positions[i];
            
            // Replace the script with TutorialCollectibleBehaviour
            var oldBehaviour = newTutorialCollectible.GetComponent<CubeBehaviour>();
            if (oldBehaviour != null)
            {
                DestroyImmediate(oldBehaviour);
            }
            
            if (newTutorialCollectible.GetComponent<TutorialCollectibleBehaviour>() == null)
            {
                newTutorialCollectible.AddComponent<TutorialCollectibleBehaviour>();
            }
            
            newTutorialCollectible.SetActive(false);
            created++;
            
            Debug.Log($"Created tutorial collectible: {newTutorialCollectible.name} at position {positions[i]}");
        }
        
        if (created > 0)
        {
            Debug.Log($"Successfully created {created} missing tutorial collectibles!");
            // Refresh the tutorial collectibles array
            tutorialCollectibles = GameObject.FindGameObjectsWithTag(tutorialCollectibleTag);
            Debug.Log($"Now have {tutorialCollectibles.Length} tutorial collectibles total");
        }
    }
}
    