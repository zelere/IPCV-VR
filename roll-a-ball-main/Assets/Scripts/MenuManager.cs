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
            instructionText.text = "Choose your interaction method:";
            instructionText.fontSize = 24;
            instructionText.color = Color.white;
            instructionText.alignment = TextAlignmentOptions.Center;
            
            RectTransform textRect = textObj.GetComponent<RectTransform>();
            textRect.anchorMin = new Vector2(0.5f, 0.5f);
            textRect.anchorMax = new Vector2(0.5f, 0.5f);
            textRect.anchoredPosition = new Vector2(0, 100);
            textRect.sizeDelta = new Vector2(300, 50);
        }
        
        // Create Hand Tracking Button
        if (handTrackingButton == null)
        {
            handTrackingButton = CreateButton("HandTrackingButton", "Hand Tracking", new Vector2(0, 50));
        }
        
        // Create Keyboard Button
        if (keyboardButton == null)
        {
            keyboardButton = CreateButton("KeyboardButton", "Keyboard Controls", new Vector2(0, 0));
        }
        
        SetupButtons();
    }
    
    Button CreateButton(string name, string text, Vector2 position)
    {
        GameObject buttonObj = new GameObject(name);
        buttonObj.transform.SetParent(startMenu.transform, false);
        
        Button button = buttonObj.AddComponent<Button>();
        Image buttonImage = buttonObj.AddComponent<Image>();
        buttonImage.color = new Color(0.2f, 0.3f, 0.8f, 0.8f);
        
        // Create button text
        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(buttonObj.transform, false);
        
        TMP_Text buttonText = textObj.AddComponent<TMP_Text>();
        buttonText.text = text;
        buttonText.fontSize = 18;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Setup RectTransforms
        RectTransform buttonRect = buttonObj.GetComponent<RectTransform>();
        buttonRect.anchorMin = new Vector2(0.5f, 0.5f);
        buttonRect.anchorMax = new Vector2(0.5f, 0.5f);
        buttonRect.anchoredPosition = position;
        buttonRect.sizeDelta = new Vector2(200, 40);
        
        RectTransform textRect = textObj.GetComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        return button;
    }
    
    void SetupButtons()
    {
        // Setup button listeners
        if (handTrackingButton != null)
        {
            handTrackingButton.onClick.AddListener(() => StartWithHandTracking());
        }
        
        if (keyboardButton != null)
        {
            keyboardButton.onClick.AddListener(() => StartWithKeyboard());
        }
        
        // Update instruction text
        if (instructionText != null)
        {
            instructionText.text = "Choose your interaction method:";
        }
    }
    
    public void StartWithHandTracking()
    {
        Debug.Log("Starting game with Hand Tracking");
        
        if (gameManager != null)
        {
            gameManager.StartGameWithHandTracking();
        }
        
        HideStartMenu();
    }
    
    public void StartWithKeyboard()
    {
        Debug.Log("Starting game with Keyboard controls");
        
        if (gameManager != null)
        {
            gameManager.StartGameWithKeyboard();
        }
        
        HideStartMenu();
    }
    
    public void ShowStartMenu()
    {
        if (startMenu != null)
        {
            startMenu.SetActive(true);
        }
    }
    
    public void HideStartMenu()
    {
        if (startMenu != null)
        {
            startMenu.SetActive(false);
        }
    }
    
    // This method can be called to return to the start menu (e.g., when game ends)
    public void ReturnToMenu()
    {
        ShowStartMenu();
    }
}