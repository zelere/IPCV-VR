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
    public TMP_Text instructionText;
    
    [Header("Game References")]
    public GameBehaviour gameManager;
    
    private void Start()
    {
        SetupButtons();
        ShowStartMenu();
        
        // If buttons are not assigned, try to find them or create them
        if (handTrackingButton == null || keyboardButton == null)
        {
            Debug.LogWarning("Buttons not assigned in MenuManager. Looking for existing buttons or consider using CreateUIElementsProgrammatically()");
            TryFindExistingButtons();
        }
    }
    
    void TryFindExistingButtons()
    {
        // Try to find buttons by name
        if (handTrackingButton == null)
        {
            GameObject handTrackingObj = GameObject.Find("HandTrackingButton");
            if (handTrackingObj != null)
                handTrackingButton = handTrackingObj.GetComponent<Button>();
        }
        
        if (keyboardButton == null)
        {
            GameObject keyboardObj = GameObject.Find("KeyboardButton");
            if (keyboardObj != null)
                keyboardButton = keyboardObj.GetComponent<Button>();
        }
        
        // Try to find start menu if not assigned
        if (startMenu == null)
        {
            GameObject startMenuObj = GameObject.Find("StartMenu");
            if (startMenuObj != null)
                startMenu = startMenuObj;
        }
        
        // Try to find game manager if not assigned
        if (gameManager == null)
        {
            gameManager = FindObjectOfType<GameBehaviour>();
        }
    }
    
    void TryFindInstructionText()
    {
        // Try to find instruction text by name or type
        if (instructionText == null)
        {
            GameObject instructionObj = GameObject.Find("InstructionText");
            if (instructionObj != null)
            {
                instructionText = instructionObj.GetComponent<TMP_Text>();
                if (instructionText != null)
                {
                    Debug.Log("Found InstructionText automatically");
                    instructionText.text = "Choose tutorial type:";
                }
            }
            else
            {
                // Try to find any TMP_Text in the start menu
                if (startMenu != null)
                {
                    TMP_Text[] texts = startMenu.GetComponentsInChildren<TMP_Text>();
                    if (texts.Length > 0)
                    {
                        instructionText = texts[0]; // Use the first text component found
                        Debug.Log("Found instruction text in start menu automatically");
                        instructionText.text = "Choose tutorial type:";
                    }
                }
            }
        }
    }
    
    // Call this method if you want to create UI elements programmatically
    public void CreateUIElementsProgrammatically()
    {
        // Find or create Canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<UnityEngine.UI.CanvasScaler>();
            canvasObj.AddComponent<UnityEngine.UI.GraphicRaycaster>();
        }
        
        // Create or find StartMenu
        if (startMenu == null)
        {
            startMenu = new GameObject("StartMenu");
            startMenu.transform.SetParent(canvas.transform, false);
            
            RectTransform rect = startMenu.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(0.5f, 0.5f);
            rect.anchorMax = new Vector2(0.5f, 0.5f);
            rect.anchoredPosition = Vector2.zero;
        }
        
        // Create instruction text
        if (instructionText == null)
        {
            GameObject textObj = new GameObject("InstructionText");
            textObj.transform.SetParent(startMenu.transform, false);
            
            instructionText = textObj.AddComponent<TMP_Text>();
            instructionText.text = "Choose tutorial type:";
            instructionText.fontSize = 24;
            instructionText.color = Color.white;
            instructionText.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0, 100);
            textRect.sizeDelta = new Vector2(300, 50);
        }
    
        SetupButtons();
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
        
        // Update instruction text
        if (instructionText != null)
        {
            instructionText.text = "Choose tutorial type:";
            Debug.Log("Instruction text updated");
        }
        else
        {
            Debug.LogWarning("Instruction text is null - trying to find it automatically");
            TryFindInstructionText();
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
        Debug.Log("=== StartWithHandTracking tutorial clicked! ===");
        
        // Reset UI state before starting
        ResetUIState();
        
        if (gameManager != null)
        {
            Debug.Log("GameManager found, calling StartTutorialWithHandTracking()");
            gameManager.StartTutorialWithHandTracking();
        }
        else
        {
            Debug.LogError("GameManager is null! Cannot start tutorial.");
        }
        
        Debug.Log("Hiding start menu...");
        HideStartMenu();
    }
    
    public void StartWithKeyboard()
    {
        Debug.Log("=== StartWithKeyboard tutorial clicked! ===");
        
        // Reset UI state before starting
        ResetUIState();
        
        if (gameManager != null)
        {
            Debug.Log("GameManager found, calling StartTutorialWithKeyboard()");
            gameManager.StartTutorialWithKeyboard();
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