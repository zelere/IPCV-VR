using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [Header("UI References")]
    public Button handTrackingButton;
    public Button keyboardButton;
    public GameObject startMenu;

    public Button startStudyButton;

    public TMP_Text instructionText;
    
    [Header("Game References")]
    public GameBehaviour gameManager;
    
    private void Start()
    {
        SetupButtons();
        ShowStartMenu();
        

    }
    

    

    void SetupButtons()
    {
        Debug.Log("Setting up buttons...");
        
        // Setup button listeners
        if (handTrackingButton != null)
        {
            handTrackingButton.onClick.AddListener(() => StartWithHandTracking());
            Debug.Log("Hand tracking button listener added successfully");
        }
        else
        {
            Debug.LogError("Hand tracking button is null! Please assign it in the inspector.");
        }
        
        if (keyboardButton != null)
        {
            keyboardButton.onClick.AddListener(() => StartWithKeyboard());
            Debug.Log("Keyboard button listener added successfully");
        }
        else
        {
            Debug.LogError("Keyboard button is null! Please assign it in the inspector.");
        }
        
        if (startStudyButton != null)
        {
            startStudyButton.onClick.AddListener(() => StartTutorial());
            Debug.Log("Start study button listener added successfully");
        }
        else
        {
            Debug.LogError("Start study button is null! Please assign it in the inspector.");
        }
        
        // Update instruction text
        if (instructionText != null)
        {
            instructionText.text = "Choose your interaction method:";
            Debug.Log("Instruction text updated");
        }
        else
        {
            Debug.LogWarning("Instruction text is null");
        }
        
        // Check if gameManager is found
        if (gameManager == null)
        {
            Debug.LogError("GameManager is null! Trying to find it automatically...");
            gameManager = FindObjectOfType<GameBehaviour>();
            if (gameManager != null)
            {
                Debug.Log("Found GameBehaviour component automatically");
            }
            else
            {
                Debug.LogError("Could not find GameBehaviour component in scene! Make sure it exists.");
            }
        }
        else
        {
            Debug.Log("GameManager reference is properly set");
        }
    }
    
    public void StartWithHandTracking()
    {
        Debug.Log("=== StartWithHandTracking button clicked! ===");
        
        // Reset UI state before starting
        ResetUIState();
        
        if (gameManager != null)
        {
            Debug.Log("GameManager found, calling StartGameWithHandTracking()");
            gameManager.StartGameWithHandTracking();
        }
        else
        {
            Debug.LogError("GameManager is null! Cannot start game.");
        }
        
        Debug.Log("Hiding start menu...");
        HideStartMenu();
    }
    
    public void StartWithKeyboard()
    {
        Debug.Log("=== StartWithKeyboard button clicked! ===");
        
        // Reset UI state before starting
        ResetUIState();
        
        if (gameManager != null)
        {
            Debug.Log("GameManager found, calling StartGameWithKeyboard()");
            gameManager.StartGameWithKeyboard();
        }
        else
        {
            Debug.LogError("GameManager is null! Cannot start game.");
        }
        
        Debug.Log("Hiding start menu...");
        HideStartMenu();
    }
    
    public void StartTutorial()
    {
        Debug.Log("=== StartTutorial button clicked! ===");
        
        // Reset UI state before starting
        ResetUIState();
        
        if (gameManager != null)
        {
            Debug.Log("GameManager found, calling StartSequentialTutorials()");
            gameManager.StartSequentialTutorials();
        }
        else
        {
            Debug.LogError("GameManager is null! Cannot start tutorial.");
        }
        
        Debug.Log("Hiding start menu...");
        HideStartMenu();
    }
    
    public void ShowStartMenu()
    {
        if (startMenu != null)
        {
            startMenu.SetActive(true);
            Debug.Log("Start menu shown");
        }
        else
        {
            Debug.LogError("Start menu GameObject is null!");
        }
    }
    
    public void HideStartMenu()
    {
        if (startMenu != null)
        {
            startMenu.SetActive(false);
            Debug.Log("Start menu hidden");
        }
        else
        {
            Debug.LogError("Start menu GameObject is null!");
        }
    }
    
    // This method can be called to return to the start menu (e.g., when game ends)
    public void ReturnToMenu()
    {
        ShowStartMenu();
    }
    
    // Reset UI state when starting a new game
    void ResetUIState()
    {
        UIBehaviour uiBehaviour = FindObjectOfType<UIBehaviour>();
        if (uiBehaviour != null)
        {
            //uiBehaviour.ResetGameState();
            Debug.Log("UI state reset");
        }
    }
}