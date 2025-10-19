using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Rendering;

public class GameBehaviour : MonoBehaviour
{
    [SerializeField] private GameObject launchMenu;
    [SerializeField] private GameObject tutorialStudyTransitionMenu;
    [SerializeField] private GameObject player;
    [SerializeField] private MonoBehaviour playerScript;

    [SerializeField] private TMP_Text handTrackingInstructionsText;
    [SerializeField] private TMP_Text keyboardInstructionsText;

    [SerializeField] private string collectibleTag = "Collectible";
    [SerializeField] private string tutorialCollectibleTag = "TutorialCollectible";


    private Vector3 playerStartPosition;
    private Quaternion playerStartRotation;
    private GameObject[] collectibles;
    private GameObject[] game_collectibles;
    private GameObject[] tutorial_collectibles;

    public enum GamePhase { LaunchMenu, KeyboardTutorial, HandTrackingTutorial, TutorialStudyTransitionMenu, Study, SecondStudy, AllCompleted }
    public GamePhase currentPhase = GamePhase.LaunchMenu;

    // Tutorial management
    private bool inTutorialPhase = false;
    private int tutorialCollectibleCount = 4; // Assuming 4 tutorial collectibles
    private bool keyboardTutorialCompleted = false;
    private bool handTrackingTutorialCompleted = false;
    
    // Study management
    private bool keyboardStudyCompleted = false;
    private bool handTrackingStudyCompleted = false;
    private bool isFirstStudy = true;
    private BallBehaviour.InteractionMode firstStudyMode;
    private BallBehaviour.InteractionMode secondStudyMode;
    
    [Header("Development/Testing")]
    [SerializeField] private bool mockHandTracking = false; // Enable to test without hand tracking sensor

    // Tracks how many collectibles have been collected
    private int currentCollectibleIndex = 0;
    private float gameStartTime; // Track when the game starts


    // Start is called before the first frame update
    void Start()
    {
        playerStartPosition = player.transform.position;
        playerStartRotation = player.transform.rotation;

        game_collectibles = GameObject.FindGameObjectsWithTag(collectibleTag);
        tutorial_collectibles = GameObject.FindGameObjectsWithTag(tutorialCollectibleTag);
        playerScript.enabled = false;
        launchMenu.SetActive(true);
        tutorialStudyTransitionMenu.SetActive(false);
        handTrackingInstructionsText.gameObject.SetActive(false);
        keyboardInstructionsText.gameObject.SetActive(false);

        // Deactivate all collectibles before the game starts
        HideAllCollectibles();
        HideAllTutorialCollectibles();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // Hide all collectibles
    private void HideAllCollectibles()
    {
        foreach (GameObject collectible in game_collectibles)
        {
            collectible.SetActive(false);
        }
    }

    // Hide all tutorial collectibles
    private void HideAllTutorialCollectibles()
    {
        foreach (GameObject collectible in tutorial_collectibles)
        {
            collectible.SetActive(false);
        }
    }

    private void StartGame()
    {
        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        HideAllCollectibles();
        HideAllTutorialCollectibles();
        collectibles = game_collectibles;
        inTutorialPhase = false;
        currentPhase = GamePhase.Study;
        
        // Reset sequence
        currentCollectibleIndex = 0;
        // Record start time
        gameStartTime = Time.time;

        // Reset UI for main game
        UIBehaviour uiBehaviour = FindFirstObjectByType<UIBehaviour>();
        if (uiBehaviour != null)
        {
            uiBehaviour.ResetForNewPhase();
        }

        // Activate the first collectible
        if (collectibles.Length > 0)
            collectibles[0].SetActive(true);

        playerScript.enabled = true;
        launchMenu.SetActive(false);
        
        Debug.Log($"Main game started with {collectibles.Length} collectibles");
    }

    private void StartTutorial()
    {

        player.transform.position = playerStartPosition;
        player.transform.rotation = playerStartRotation;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;

        HideAllCollectibles();
        HideAllTutorialCollectibles();


        collectibles = tutorial_collectibles;
        inTutorialPhase = true;

        // Reset sequence
        currentCollectibleIndex = 0;
        // Record start time
        gameStartTime = Time.time;

        // Reset UI for tutorial
        UIBehaviour uiBehaviour = FindFirstObjectByType<UIBehaviour>();
        if (uiBehaviour != null)
        {
            uiBehaviour.ResetForNewPhase();
        }

        // Activate the first collectible
        if (collectibles.Length > 0)
            collectibles[0].SetActive(true);


        playerScript.enabled = true;
        launchMenu.SetActive(false);
    }

    private void StartTutorialWithKeyboard()
    {
        // Set interaction mode to keyboard
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);

        currentPhase = GamePhase.KeyboardTutorial;
        Debug.Log("Starting keyboard tutorial");
        handTrackingInstructionsText.gameObject.SetActive(false);
        keyboardInstructionsText.gameObject.SetActive(true);
        
        // Set keyboard tutorial instructions
        if (keyboardInstructionsText != null)
        {
            keyboardInstructionsText.text = "KEYBOARD TUTORIAL\n\n" +
                                           "Use WASD or Arrow Keys to move\n" +
                                           "Collect all 4 collectibles";
        }

        StartTutorial();
    }
    
    // Method to toggle mock hand tracking mode (useful for testing)
    public void SetMockHandTracking(bool enabled)
    {
        mockHandTracking = enabled;
        Debug.Log($"Mock hand tracking set to: {enabled}");
    }
    
    // Method to check if currently in mock mode
    public bool IsMockHandTrackingEnabled()
    {
        return mockHandTracking;
    }

    private void StartTutorialWithHandTracking()
    {
        // Set interaction mode - use keyboard if mocking, otherwise hand tracking
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        
        if (mockHandTracking)
        {
            // Mock mode: use keyboard controls but show hand tracking instructions
            ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);
            Debug.Log("Starting MOCK hand tracking tutorial (using keyboard controls)");
        }
        else
        {
            // Real mode: use actual hand tracking
            ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.HandTracking);
            Debug.Log("Starting hand tracking tutorial");
        }

        currentPhase = GamePhase.HandTrackingTutorial;
        handTrackingInstructionsText.gameObject.SetActive(true);
        keyboardInstructionsText.gameObject.SetActive(false);
        
        // Update instruction text for mock mode
        if (mockHandTracking && handTrackingInstructionsText != null)
        {
            handTrackingInstructionsText.text = "MOCK HAND TRACKING TUTORIAL\n\n" +
                                               "(Using keyboard for testing)\n" +
                                               "Imagine moving your hand above the ball\n" +
                                               "Use WASD to simulate hand movement\n" +
                                               "Collect all 4 collectibles";
        }

        StartTutorial();
    }

    // Call this method when a collectible is collected
    public void CollectibleCollected()
    {

        if (currentCollectibleIndex < collectibles.Length)
        {
            collectibles[currentCollectibleIndex].SetActive(false);
        }

        currentCollectibleIndex++;
        
        // Check if current tutorial phase is complete
        if (inTutorialPhase && currentCollectibleIndex >= tutorialCollectibleCount)
        {
            if (currentPhase == GamePhase.KeyboardTutorial)
            {
                keyboardTutorialCompleted = true;
                Debug.Log("Keyboard tutorial completed! Starting hand tracking tutorial...");
                
                // Reset for hand tracking tutorial
                HideAllTutorialCollectibles();
                currentCollectibleIndex = 0;
                
                // Start hand tracking tutorial
                StartTutorialWithHandTracking();
            }
            else if (currentPhase == GamePhase.HandTrackingTutorial)
            {
                handTrackingTutorialCompleted = true;
                inTutorialPhase = false;
                handTrackingInstructionsText.gameObject.SetActive(false);

                
                // Both tutorials completed, show transition menu
                currentPhase = GamePhase.TutorialStudyTransitionMenu;
                playerScript.enabled = false;
                tutorialStudyTransitionMenu.SetActive(true);
                Debug.Log("Both tutorials completed! Opening transition menu.");
            }
        }
        else if (currentCollectibleIndex < collectibles.Length)
        {
            collectibles[currentCollectibleIndex].SetActive(true);
        }
        else
        {
            // All collectibles collected - game/tutorial phase complete
            if (inTutorialPhase)
            {
                Debug.Log("All tutorial collectibles collected but tutorial not marked complete - this shouldn't happen");
            }
            else
            {
                // Study finished
                float totalTime = Time.time - gameStartTime;
                string mode = (player.GetComponent<BallBehaviour>().GetInteractionMode() == BallBehaviour.InteractionMode.Keyboard) ? "Keyboard" : "Hand Tracking";
                Debug.Log($"--> Study finished! Total time: {totalTime} seconds using {mode}");
                
                // Hide instruction texts
                handTrackingInstructionsText.gameObject.SetActive(false);
                keyboardInstructionsText.gameObject.SetActive(false);
                
                // Handle study progression
                HandleStudyCompletion(totalTime);
            }
        }
    }
    
    // Handle completion of a study and progression to next study or end
    private void HandleStudyCompletion(float completionTime)
    {
        if (isFirstStudy)
        {
            // First study completed
            isFirstStudy = false;
            
            // Mark which study was completed
            if (firstStudyMode == BallBehaviour.InteractionMode.Keyboard)
            {
                keyboardStudyCompleted = true;
            }
            else
            {
                handTrackingStudyCompleted = true;
            }
            
            Debug.Log($"First study completed in {completionTime} seconds. Starting second study...");
            
            // Show countdown message using victory text
            UIBehaviour uiBehaviour = FindFirstObjectByType<UIBehaviour>();
            if (uiBehaviour != null)
            {
                uiBehaviour.ShowCountdownMessage("Part 1 completed!\nNext part starts in 5 seconds...");
            }
            
            // Start second study after a 5-second delay with countdown
            StartCoroutine(StartSecondStudyAfterDelay());
        }
        else
        {
            // Second study completed
            if (secondStudyMode == BallBehaviour.InteractionMode.Keyboard)
            {
                keyboardStudyCompleted = true;
            }
            else
            {
                handTrackingStudyCompleted = true;
            }
            
            Debug.Log($"Second study completed in {completionTime} seconds. All studies finished!");
            
            // Both studies completed
            currentPhase = GamePhase.AllCompleted;
            StartCoroutine(ShowAllStudiesCompletedAfterDelay());
        }
    }
    
    // Coroutine to start second study after a 5-second countdown
    private IEnumerator StartSecondStudyAfterDelay()
    {
        UIBehaviour uiBehaviour = FindFirstObjectByType<UIBehaviour>();
        
        // Show countdown for 5 seconds
        for (int i = 5; i > 0; i--)
        {
            if (uiBehaviour != null)
            {
                uiBehaviour.ShowCountdownMessage($"Part 1 completed!\nNext part starts in {i} seconds...");
            }
            yield return new WaitForSeconds(1f);
        }
        
        // Hide countdown message
        if (uiBehaviour != null)
        {
            uiBehaviour.HideCountdownMessage();
        }
        
        Debug.Log("Starting second study...");
        StartStudyWithMode(secondStudyMode);
    }
    
    // Coroutine to show completion message and return to menu
    private IEnumerator ShowAllStudiesCompletedAfterDelay()
    {
        yield return new WaitForSeconds(3f);
        
        Debug.Log("All studies completed! Returning to launch menu...");
        startOver();
    }

    // Method to start randomized study sequence
    public void StartRandomizedStudy()
    {
        // Randomly choose which interaction mode to start with
        bool startWithKeyboard = Random.Range(0, 2) == 0;
        
        if (startWithKeyboard)
        {
            firstStudyMode = BallBehaviour.InteractionMode.Keyboard;
            secondStudyMode = mockHandTracking ? BallBehaviour.InteractionMode.Keyboard : BallBehaviour.InteractionMode.HandTracking;
        }
        else
        {
            firstStudyMode = mockHandTracking ? BallBehaviour.InteractionMode.Keyboard : BallBehaviour.InteractionMode.HandTracking;
            secondStudyMode = BallBehaviour.InteractionMode.Keyboard;
        }
        
        Debug.Log($"Starting randomized study sequence: First={firstStudyMode}, Second={secondStudyMode}");
        
        // Start first study
        isFirstStudy = true;
        StartStudyWithMode(firstStudyMode);
    }
    
    // Method to start study with specific interaction mode
    private void StartStudyWithMode(BallBehaviour.InteractionMode mode)
    {
        Debug.Log($"StartStudyWithMode called with {mode}, isFirstStudy: {isFirstStudy}");
        
        // Set the current phase
        currentPhase = isFirstStudy ? GamePhase.Study : GamePhase.SecondStudy;
        
        // Configure ball behaviour
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(mode);
        
        // Show appropriate instructions
        if (mode == BallBehaviour.InteractionMode.Keyboard)
        {
            keyboardInstructionsText.gameObject.SetActive(true);
            handTrackingInstructionsText.gameObject.SetActive(false);
            
            if (keyboardInstructionsText != null)
            {
                string studyNumber = isFirstStudy ? "FIRST" : "SECOND";
                keyboardInstructionsText.text = $"{studyNumber} STUDY - KEYBOARD\n\n" +
                                               "Use WASD or Arrow Keys to move\n" +
                                               "Collect all collectibles as quickly as possible";
                Debug.Log($"Set keyboard instructions: {keyboardInstructionsText.text}");
                
                // Debug UI state
                Debug.Log($"Keyboard text active: {keyboardInstructionsText.gameObject.activeInHierarchy}");
                Debug.Log($"Keyboard text enabled: {keyboardInstructionsText.enabled}");
                Debug.Log($"Keyboard text position: {keyboardInstructionsText.transform.position}");
                Debug.Log($"Keyboard text color: {keyboardInstructionsText.color}");
            }
            else
            {
                Debug.LogWarning("keyboardInstructionsText is null!");
            }
        }
        else
        {
            handTrackingInstructionsText.gameObject.SetActive(true);
            keyboardInstructionsText.gameObject.SetActive(false);
            
            if (handTrackingInstructionsText != null)
            {
                string studyNumber = isFirstStudy ? "FIRST" : "SECOND";
                if (mockHandTracking)
                {
                    handTrackingInstructionsText.text = $"{studyNumber} STUDY - HAND TRACKING (MOCK)\n\n" +
                                                       "(Using keyboard for testing)\n" +
                                                       "Use WASD to simulate hand movement\n" +
                                                       "Collect all collectibles as quickly as possible";
                    handTrackingInstructionsText.text = $"{studyNumber} STUDY - HAND TRACKING\n\n" +
                                                       "Move the ball with pointing with your right hand. \n Pick up the ball by pinching with left hand.\n" +
                                                       "Collect all collectibles as quickly as possible";}
                else
                {
                    handTrackingInstructionsText.text = $"{studyNumber} STUDY - HAND TRACKING\n\n" +
                                                       "Move the ball with pointing with your right hand. \n Pick up the ball by pinching with left hand.\n" +
                                                       "Collect all collectibles as quickly as possible";
                }
                Debug.Log($"Set hand tracking instructions: {handTrackingInstructionsText.text}");
                
                // Debug UI state
                Debug.Log($"Hand tracking text active: {handTrackingInstructionsText.gameObject.activeInHierarchy}");
                Debug.Log($"Hand tracking text enabled: {handTrackingInstructionsText.enabled}");
                Debug.Log($"Hand tracking text position: {handTrackingInstructionsText.transform.position}");
                Debug.Log($"Hand tracking text color: {handTrackingInstructionsText.color}");
            }
            else
            {
                Debug.LogWarning("handTrackingInstructionsText is null!");
            }
        }
        
        Debug.Log($"Starting {(isFirstStudy ? "first" : "second")} study with {mode} mode");
        
        // Start the actual game
        StartGame();
    }

    public void StartSequentialTutorials()
    {
        // Start with keyboard tutorial first
        keyboardTutorialCompleted = false;
        handTrackingTutorialCompleted = false;
        
        // Ensure transition menu is hidden when starting tutorials
        tutorialStudyTransitionMenu.SetActive(false);
        
        StartTutorialWithKeyboard();
    }
    
    // Individual game start methods (for direct menu access)
    public void StartGameWithKeyboard()
    {
        Debug.Log("Starting single game with keyboard");
        
        // Configure ball for keyboard
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);
        
        // Set appropriate instructions
        keyboardInstructionsText.gameObject.SetActive(true);
        handTrackingInstructionsText.gameObject.SetActive(false);
        
        if (keyboardInstructionsText != null)
        {
            keyboardInstructionsText.text = "KEYBOARD GAME\n\n" +
                                           "Use WASD or Arrow Keys to move\n" +
                                           "Collect all collectibles";
        }
        
        StartGame();
    }
    
    public void StartGameWithHandTracking()
    {
        Debug.Log("Starting single game with hand tracking");
        
        // Configure ball for hand tracking (or mock)
        BallBehaviour ballBehaviour = player.GetComponent<BallBehaviour>();
        
        if (mockHandTracking)
        {
            ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);
        }
        else
        {
            ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.HandTracking);
        }
        
        // Set appropriate instructions
        handTrackingInstructionsText.gameObject.SetActive(true);
        keyboardInstructionsText.gameObject.SetActive(false);
        
        if (handTrackingInstructionsText != null)
        {
            if (mockHandTracking)
            {
                handTrackingInstructionsText.text = "HAND TRACKING GAME (MOCK)\n\n" +
                                                   "(Using keyboard for testing)\n" +
                                                   "Use WASD to simulate hand movement\n" +
                                                   "Collect all collectibles";
                handTrackingInstructionsText.text = $"HAND TRACKING\n\n" +
                                        "Move the ball with pointing with your right hand. \n Pick up the ball by pinching with left hand.\n" +
                                        "Collect all collectibles";
            }
            else
            {
                handTrackingInstructionsText.text = "HAND TRACKING GAME\n\n"  +
                                                   "Move the ball with pointing with your right hand. \n Pick up the ball by pinching with left hand.\n" +
                                                   "Collect all collectibles";
            }
        }
        
        StartGame();
    }

    // Public methods for UI to get current game state
    public bool IsInTutorialPhase()
    {
        return inTutorialPhase;
    }

    public int GetCurrentCollectibleCount()
    {
        if (inTutorialPhase)
            return tutorialCollectibleCount;
        else
            return game_collectibles.Length;
    }

    public int GetRemainingCollectibles()
    {
        if (collectibles == null) return 0;
        return collectibles.Length - currentCollectibleIndex;
    }

    public void startOver()
    {
        playerScript.enabled = false;
        launchMenu.SetActive(true);
        
        // Hide transition menu
        tutorialStudyTransitionMenu.SetActive(false);
        
        // Reset tutorial state
        keyboardTutorialCompleted = false;
        handTrackingTutorialCompleted = false;
        inTutorialPhase = false;
        
        // Reset study state
        keyboardStudyCompleted = false;
        handTrackingStudyCompleted = false;
        isFirstStudy = true;
        
        currentPhase = GamePhase.LaunchMenu;
        currentCollectibleIndex = 0;
        
        // Hide all collectibles and instruction texts
        HideAllCollectibles();
        HideAllTutorialCollectibles();
        handTrackingInstructionsText.gameObject.SetActive(false);
        keyboardInstructionsText.gameObject.SetActive(false);
        
        Debug.Log("Game reset to launch menu");
    }
}
    