using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    //[SerializeField] private string collectibleTag = "Collectible";
    [Header("UI Elements")]
    [SerializeField] private TMP_Text collectiblesText;
    [SerializeField] private TMP_Text victoryText;

    private GameBehaviour gameBehaviour;

    void Start()
    {
        // Find GameBehaviour reference
        gameBehaviour = FindFirstObjectByType<GameBehaviour>();

        victoryText.gameObject.SetActive(false);

        UpdateCollectiblesText();

    }

    private void UpdateCollectiblesText()
    {
        // Get actual remaining collectibles from GameBehaviour
        int actualRemaining = 0;
        if (gameBehaviour != null)
        {
            actualRemaining = gameBehaviour.GetRemainingCollectibles();
            Debug.Log($"UI: Retrieved remaining collectibles from GameBehaviour: {actualRemaining}");
        }
        else
        {
            Debug.LogWarning($"UI: GameBehaviour reference not found");
            return;
        }
        
        if (actualRemaining > 0)
        {
            collectiblesText.text = "Collectibles left: " + actualRemaining;
            Debug.Log($"UI: Updated collectibles text: {collectiblesText.text}");
        }
        else
        {
            collectiblesText.gameObject.SetActive(false);
            victoryText.gameObject.SetActive(true);

            // Don't automatically restart during tutorial - let GameBehaviour handle it
            if (gameBehaviour != null && !gameBehaviour.IsInTutorialPhase())
            {
                Debug.Log("UI: Main game completed, restarting...");
                FindAnyObjectByType<GameBehaviour>().startOver();
            }
            else if (gameBehaviour != null && gameBehaviour.IsInTutorialPhase())
            {
                Debug.Log("UI: Tutorial phase completed, letting GameBehaviour handle transition");
            }
        }
    }
    
    public void OnCollectiblesPicked()
    {
        // Simply update UI based on actual game state from GameBehaviour
        UpdateCollectiblesText();
    }
    
    // Method to reset UI for different game phases
    public void ResetForNewPhase()
    {
        if (gameBehaviour != null)
        {
            Debug.Log($"UI: Reset for new phase - Tutorial: {gameBehaviour.IsInTutorialPhase()}");
            
            // Show collectibles text and hide victory text
            collectiblesText.gameObject.SetActive(true);
            victoryText.gameObject.SetActive(false);
            
            UpdateCollectiblesText();
        }
        else
        {
            Debug.LogWarning("UI: GameBehaviour reference not found in ResetForNewPhase");
        }
    }
    
    // Method to show countdown message using victory text
    public void ShowCountdownMessage(string message)
    {
        if (victoryText != null)
        {
            // Hide collectibles text and show victory text with countdown
            collectiblesText.gameObject.SetActive(false);
            victoryText.gameObject.SetActive(true);
            victoryText.text = message;
            Debug.Log($"UI: Showing countdown message: {message}");
        }
        else
        {
            Debug.LogWarning("UI: Victory text not assigned!");
        }
    }
    
    // Method to hide countdown message
    public void HideCountdownMessage()
    {
        if (victoryText != null)
        {
            victoryText.gameObject.SetActive(false);
            // Show collectibles text again
            collectiblesText.gameObject.SetActive(true);
            Debug.Log("UI: Hiding countdown message");
        }
    }

}
