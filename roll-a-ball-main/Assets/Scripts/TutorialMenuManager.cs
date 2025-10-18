using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TutorialMenuManager : MonoBehaviour
{
    [Header("Menu References")]
    [SerializeField] private GameObject startMenu;
    [SerializeField] private GameObject tutorialCompleteMenu;
    [SerializeField] private GameBehaviour gameBehaviour;
    
    [Header("Start Menu Buttons")]
    [SerializeField] private Button startTutorialButton;
    [SerializeField] private Button startWithHandTrackingButton;
    [SerializeField] private Button startWithKeyboardButton;
    
    [Header("Tutorial Complete Menu")]
    [SerializeField] private Button startStudyButton;
    [SerializeField] private Button backToMenuButton;
    [SerializeField] private TMP_Text selectedModeText;
    
    void Awake()
    {
        // Ensure tutorial complete menu is hidden initially
        if (tutorialCompleteMenu != null)
            tutorialCompleteMenu.SetActive(false);
    }

    void Start()
    {
        if (gameBehaviour == null)
            gameBehaviour = FindFirstObjectByType<GameBehaviour>();
            
        SetupButtons();
        
        // Double-check that tutorial complete menu is hidden
        if (tutorialCompleteMenu != null)
            tutorialCompleteMenu.SetActive(false);
    }
    
    void SetupButtons()
    {       // Start menu buttons
        if (startTutorialButton != null)
            startTutorialButton.onClick.AddListener(() => gameBehaviour.StartTutorial());
            
        if (startWithHandTrackingButton != null)
            startWithHandTrackingButton.onClick.AddListener(() => gameBehaviour.StartGameWithHandTracking());
            
        if (startWithKeyboardButton != null)
            startWithKeyboardButton.onClick.AddListener(() => gameBehaviour.StartGameWithKeyboard());
        
        // Tutorial complete menu buttons
        if (startStudyButton != null)
            startStudyButton.onClick.AddListener(() => gameBehaviour.StartStudy());
            
        if (backToMenuButton != null)
            backToMenuButton.onClick.AddListener(() => gameBehaviour.startOver());
    }
    
    public void UpdateSelectedModeText(string modeName)
    {
        if (selectedModeText != null)
        {
            selectedModeText.text = $"\nControl mode selected: {modeName} \nClick 'Start Study' when ready!\n";
        }
    }
    
    public void ShowTutorialCompleteMenu()
    {
        if (tutorialCompleteMenu != null)
        {
            tutorialCompleteMenu.SetActive(true);
            Debug.Log("Tutorial complete menu shown");
        }
        else
        {
            Debug.LogError("TutorialCompleteMenu reference is not assigned!");
        }
    }
    
    public void HideTutorialCompleteMenu()
    {
        if (tutorialCompleteMenu != null)
        {
            tutorialCompleteMenu.SetActive(false);
            Debug.Log("Tutorial complete menu hidden");
        }
    }
}