using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIBehaviour : MonoBehaviour
{
    [SerializeField] private string collectibleTag = "Collectible";
    [Header("UI Elements")]
    [SerializeField] private TMP_Text collectiblesText;
    [SerializeField] private TMP_Text victoryText;

    private int numCollectibles;
    private int originalNumCollectibles;

    void Start()
    {

        victoryText.gameObject.SetActive(false);

        numCollectibles = GameObject.FindGameObjectsWithTag(collectibleTag).Length;
        originalNumCollectibles = numCollectibles;

        UpdateCollectiblesText();

    }

    private void UpdateCollectiblesText()
    {
        if (numCollectibles > 0)
        {
            collectiblesText.text = "Collectibles left: " + numCollectibles;
        }
        else
        {
            collectiblesText.gameObject.SetActive(false);
            victoryText.gameObject.SetActive(true);

            FindAnyObjectByType<GameBehaviour>().startOver();
            numCollectibles = originalNumCollectibles;

        }
    }
    public void OnCollectiblesPicked()
    {
        numCollectibles--;
        UpdateCollectiblesText();

    }

}
