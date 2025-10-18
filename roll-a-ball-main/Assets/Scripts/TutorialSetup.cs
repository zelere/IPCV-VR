using UnityEngine;

public class TutorialSetup : MonoBehaviour
{
    [Header("Tutorial Collectibles Setup")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private Transform arenaCenter;
    [SerializeField] private float arenaSize = 10f; // Adjust based on your arena size
    [SerializeField] private float collectibleHeight = 0.5f;
    
    private GameObject[] tutorialCollectibles = new GameObject[4];
    
    void Start()
    {
        if (collectiblePrefab != null)
        {
            CreateTutorialCollectibles();
        }
    }
    
    void CreateTutorialCollectibles()
    {
        Vector3 center = arenaCenter != null ? arenaCenter.position : Vector3.zero;
        float offset = arenaSize / 2f - 1f; // Place them slightly inside the corners
        
        // Define the 4 corner positions
        Vector3[] cornerPositions = new Vector3[]
        {
            new Vector3(center.x + offset, center.y + collectibleHeight, center.z + offset), // Top-right
            new Vector3(center.x - offset, center.y + collectibleHeight, center.z + offset), // Top-left
            new Vector3(center.x - offset, center.y + collectibleHeight, center.z - offset), // Bottom-left
            new Vector3(center.x + offset, center.y + collectibleHeight, center.z - offset)  // Bottom-right
        };
        
        for (int i = 0; i < 4; i++)
        {
            GameObject tutorialCollectible = Instantiate(collectiblePrefab, cornerPositions[i], Quaternion.identity);
            tutorialCollectible.name = $"TutorialCollectible_{i + 1}";
            tutorialCollectible.tag = "TutorialCollectible";
            
            // Replace the CubeBehaviour with TutorialCollectibleBehaviour
            if (tutorialCollectible.GetComponent<CubeBehaviour>() != null)
            {
                DestroyImmediate(tutorialCollectible.GetComponent<CubeBehaviour>());
            }
            
            if (tutorialCollectible.GetComponent<TutorialCollectibleBehaviour>() == null)
            {
                tutorialCollectible.AddComponent<TutorialCollectibleBehaviour>();
            }
            
            tutorialCollectibles[i] = tutorialCollectible;
            
            // Initially disable them - they will be enabled when tutorial starts
            tutorialCollectible.SetActive(false);
        }
        
        Debug.Log("Tutorial collectibles created at arena corners");
    }
    
    [ContextMenu("Create Tutorial Collectibles")]
    public void CreateTutorialCollectiblesEditor()
    {
        CreateTutorialCollectibles();
    }
    
    [ContextMenu("Remove Tutorial Collectibles")]
    public void RemoveTutorialCollectibles()
    {
        GameObject[] existingTutorialCollectibles = GameObject.FindGameObjectsWithTag("TutorialCollectible");
        for (int i = 0; i < existingTutorialCollectibles.Length; i++)
        {
            if (Application.isPlaying)
                Destroy(existingTutorialCollectibles[i]);
            else
                DestroyImmediate(existingTutorialCollectibles[i]);
        }
        Debug.Log("Tutorial collectibles removed");
    }
}