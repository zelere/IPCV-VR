using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private string collectibleTag = "Collectible";
    [SerializeField] private string tutorialCollectibleTag = "TutorialCollectible";
    [Header("UI Elements")]
    [SerializeField] private TMP_Text collectiblesText;
    [SerializeField] private TMP_Text victoryText;
    [SerializeField] private TMP_Text tutorialText;

    private int numCollectibles;
    private int originalNumCollectibles;
    private int numTutorialCollectibles;
    private int originalNumTutorialCollectibles;
    
    private GameBehaviour gameBehaviour;

    void Start()
    {
        gameBehaviour = FindAnyObjectByType<GameBehaviour>();
        
        victoryText.gameObject.SetActive(false);
        
        if (tutorialText != null)
            tutorialText.gameObject.SetActive(false);

        numCollectibles = GameObject.FindGameObjectsWithTag(collectibleTag).Length;
        originalNumCollectibles = numCollectibles;
        
        numTutorialCollectibles = GameObject.FindGameObjectsWithTag(tutorialCollectibleTag).Length;
        originalNumTutorialCollectibles = numTutorialCollectibles;

        UpdateCollectiblesText();
    }

    private void UpdateCollectiblesText()
    {
        if (gameBehaviour != null && gameBehaviour.IsTutorialPhase())
        {
            // Tutorial phase
            if (numTutorialCollectibles > 0)
            {
                collectiblesText.text = "Tutorial - Collectibles left: " + numTutorialCollectibles;
                if (tutorialText != null)
                {
                    tutorialText.gameObject.SetActive(true);
                    tutorialText.text = "Collect all cubes to complete the tutorial";
                }
            }
            else
            {
                collectiblesText.gameObject.SetActive(false);
                if (tutorialText != null)
                {
                    tutorialText.text = "Tutorial completed! Get ready for the study.";
                }
                
                gameBehaviour.OnTutorialComplete();
                numTutorialCollectibles = originalNumTutorialCollectibles;
            }
        }
        else
        {
            // Study phase
            if (numCollectibles > 0)
            {
                collectiblesText.text = "Collectibles left: " + numCollectibles;
                if (tutorialText != null)
                    tutorialText.gameObject.SetActive(false);
            }
            else
            {
                collectiblesText.gameObject.SetActive(false);
                victoryText.gameObject.SetActive(true);
                if (tutorialText != null)
                    tutorialText.gameObject.SetActive(false);

                gameBehaviour.startOver();
                numCollectibles = originalNumCollectibles;
            }
        }
    }
    public void OnCollectiblesPicked()
    {
        if (gameBehaviour != null && gameBehaviour.IsTutorialPhase())
        {
            numTutorialCollectibles--;
        }
        else
        {
            numCollectibles--;
        }
        UpdateCollectiblesText();
    }
    
    public void ResetTutorialCollectibles()
    {
        numTutorialCollectibles = originalNumTutorialCollectibles;
        UpdateCollectiblesText();
    }
    
    public void ResetStudyCollectibles()
    {
        numCollectibles = originalNumCollectibles;
        UpdateCollectiblesText();
    }
}
