using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialStudyTransitionMenu : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button startTimedStudyButton;
    [SerializeField] private Button restartTutorialButton;
    [SerializeField] private Button returnToLaunchMenuButton;
    [SerializeField] private TMP_Text instructionText;
    
    [Header("Game References")]
    [SerializeField] private GameBehaviour gameManager;
    
    [Header("Menu References")]
    [SerializeField] private GameObject transitionMenu;
    [SerializeField] private GameObject launchMenu;
    
    private void Start()
    {
        SetupButtons();
        UpdateInstructionText();
    }
    
    private void SetupButtons()
    {
        // Setup button listeners
        if (startTimedStudyButton != null)
        {
            startTimedStudyButton.onClick.RemoveAllListeners();
            startTimedStudyButton.onClick.AddListener(OnStartTimedStudy);
        }
        
        if (restartTutorialButton != null)
        {
            restartTutorialButton.onClick.RemoveAllListeners();
            restartTutorialButton.onClick.AddListener(OnRestartTutorial);
        }
        
        if (returnToLaunchMenuButton != null)
        {
            returnToLaunchMenuButton.onClick.RemoveAllListeners();
            returnToLaunchMenuButton.onClick.AddListener(OnReturnToLaunchMenu);
        }
        
        // Auto-find GameBehaviour if not assigned
        if (gameManager == null)
        {
            gameManager = FindFirstObjectByType<GameBehaviour>();
        }
        
        // Auto-find this menu GameObject if not assigned
        if (transitionMenu == null)
        {
            transitionMenu = this.gameObject;
        }
    }

    public void UpdateInstructionText(string customText = null)
    {
        if (instructionText != null)
        {
            if (!string.IsNullOrEmpty(customText))
            {
                instructionText.text = customText;
            }
            else
            {
                instructionText.text = "";
            }
        }
    }
    

    
    // Called when "Start Timed Study" button is pressed
    public void OnStartTimedStudy()
    {
        Debug.Log("Starting randomized timed study sequence...");
        
        if (gameManager != null)
        {
            // Hide this menu
            HideTransitionMenu();
            
            // Start randomized study sequence (will do both interaction modes)
            gameManager.StartRandomizedStudy();
        }
        else
        {
            Debug.LogError("GameBehaviour reference not found!");
        }
    }
    
    // Called when "Restart Tutorial" button is pressed
    public void OnRestartTutorial()
    {
        Debug.Log("Restarting tutorial sequence...");
        
        if (gameManager != null)
        {
            // Hide this menu
            HideTransitionMenu();
            
            // Restart the sequential tutorials (keyboard -> hand tracking)
            gameManager.StartSequentialTutorials();
        }
        else
        {
            Debug.LogError("GameBehaviour reference not found!");
        }
    }
    
    // Called when "Return to Launch Menu" button is pressed
    public void OnReturnToLaunchMenu()
    {
        Debug.Log("Returning to launch menu...");
        
        if (gameManager != null)
        {
            // Hide this menu
            HideTransitionMenu();
            
            // Show launch menu and reset game state
            gameManager.startOver();
        }
        else
        {
            Debug.LogError("GameBehaviour reference not found!");
        }
    }
    
    // Helper method to hide the transition menu
    private void HideTransitionMenu()
    {
        if (transitionMenu != null)
        {
            transitionMenu.SetActive(false);
        }
    }
    
    // Public method to show the transition menu (called by GameBehaviour)
    public void ShowTransitionMenu()
    {
        if (transitionMenu != null)
        {
            transitionMenu.SetActive(true);
            UpdateInstructionText();
        }
    }
    
    // Alternative method for starting study with specific interaction mode
    public void StartStudyWithInteractionChoice()
    {
        // You could implement a sub-menu here to choose interaction mode for the study
        // For now, this is a placeholder for future enhancement
        Debug.Log("Interaction choice menu could be implemented here");
    }
    
    // Method to get tutorial completion status (if needed for UI updates)
    public bool AreTutorialsCompleted()
    {
        if (gameManager != null)
        {
            // You might need to add public properties to GameBehaviour to access these
            return gameManager.currentPhase == GameBehaviour.GamePhase.TutorialStudyTransitionMenu;
        }
        return false;
    }
    

}