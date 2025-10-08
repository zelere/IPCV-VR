using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ExperimentManager : MonoBehaviour
{
    [Header("Experiment Configuration")]
    [SerializeField] private int numberOfCollectibles = 12;
    [SerializeField] private bool useRandomPositions = true;
    [SerializeField] private bool useManualPositions = false;
    
    [Header("Manual Position Setup (if useManualPositions = true)")]
    [SerializeField] private Vector3[] manualCollectiblePositions = new Vector3[12];
    
    [Header("Random Position Configuration")]
    [SerializeField] private float arenaRadius = 4.0f;
    [SerializeField] private float minDistanceBetweenCollectibles = 1.5f;
    [SerializeField] private float collectibleHeight = 0.5f;
    
    [Header("Collectible References")]
    [SerializeField] private GameObject collectiblePrefab;
    [SerializeField] private Transform collectiblesParent;
    
    [Header("Manual Collectible References (Alternative)")]
    [SerializeField] private Transform[] manualCollectibleReferences = new Transform[12];
    
    [Header("Tutorial Configuration")]
    [SerializeField] private Vector3[] tutorialPositions = new Vector3[4];
    [SerializeField] private bool usePredefinedTutorialPositions = true;
    
    [Header("Experiment State")]
    [SerializeField] private int currentCollectibleIndex = 0;
    [SerializeField] private bool isInTutorialPhase = true;
    [SerializeField] private bool experimentActive = false;
    
    [Header("UI References")]
    [SerializeField] private TMP_Text progressText;
    [SerializeField] private TMP_Text phaseText;
    [SerializeField] private GameObject startExperimentButton;
    
    // Private variables
    private List<GameObject> experimentCollectibles = new List<GameObject>();
    private List<GameObject> tutorialCollectibles = new List<GameObject>();
    private Vector3[] finalCollectiblePositions;
    private int tutorialCollectiblesCollected = 0;
    
    // Events
    public System.Action<int> OnCollectibleCollected;
    public System.Action OnTutorialCompleted;
    public System.Action OnExperimentCompleted;
    
    void Start()
    {
        InitializeSequenceVariables();
        SetupTutorialPhase();
    }
    
    /// <summary>
    /// Step 3.1 - Initialize sequence variables for the experiment
    /// </summary>
    void InitializeSequenceVariables()
    {
        Debug.Log("Initializing experiment sequence variables...");
        
        // Initialize positions array
        finalCollectiblePositions = new Vector3[numberOfCollectibles];
        
        if (useManualPositions && manualCollectiblePositions.Length >= numberOfCollectibles)
        {
            // Use manually set positions
            for (int i = 0; i < numberOfCollectibles; i++)
            {
                finalCollectiblePositions[i] = manualCollectiblePositions[i];
            }
            Debug.Log("Using manual collectible positions");
        }
        else if (useRandomPositions)
        {
            // Generate random positions
            GenerateRandomCollectiblePositions();
            Debug.Log("Generated random collectible positions");
        }
        else
        {
            // Use default grid positions as fallback
            GenerateGridCollectiblePositions();
            Debug.Log("Using default grid collectible positions");
        }
        
        // Initialize tutorial positions
        InitializeTutorialPositions();
        
        // Setup collectible references
        SetupCollectibleReferences();
        
        Debug.Log($"Experiment initialized with {numberOfCollectibles} collectibles");
    }
    
    /// <summary>
    /// Generate random positions for collectibles within the arena
    /// </summary>
    void GenerateRandomCollectiblePositions()
    {
        List<Vector3> generatedPositions = new List<Vector3>();
        int maxAttempts = 100;
        
        for (int i = 0; i < numberOfCollectibles; i++)
        {
            Vector3 newPosition = Vector3.zero;
            bool validPosition = false;
            int attempts = 0;
            
            while (!validPosition && attempts < maxAttempts)
            {
                // Generate random position within arena
                float angle = Random.Range(0f, 2f * Mathf.PI);
                float distance = Random.Range(1f, arenaRadius);
                
                newPosition = new Vector3(
                    Mathf.Cos(angle) * distance,
                    collectibleHeight,
                    Mathf.Sin(angle) * distance
                );
                
                // Check minimum distance from other collectibles
                validPosition = true;
                foreach (Vector3 existingPos in generatedPositions)
                {
                    if (Vector3.Distance(newPosition, existingPos) < minDistanceBetweenCollectibles)
                    {
                        validPosition = false;
                        break;
                    }
                }
                
                attempts++;
            }
            
            if (validPosition)
            {
                generatedPositions.Add(newPosition);
                finalCollectiblePositions[i] = newPosition;
            }
            else
            {
                // Fallback to grid position if can't find valid random position
                finalCollectiblePositions[i] = GetGridPosition(i);
                Debug.LogWarning($"Could not find valid random position for collectible {i}, using grid position");
            }
        }
    }
    
    /// <summary>
    /// Generate grid-based positions as fallback
    /// </summary>
    void GenerateGridCollectiblePositions()
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(numberOfCollectibles));
        float spacing = (arenaRadius * 2f) / (gridSize + 1);
        
        for (int i = 0; i < numberOfCollectibles; i++)
        {
            int row = i / gridSize;
            int col = i % gridSize;
            
            Vector3 position = new Vector3(
                -arenaRadius + (col + 1) * spacing,
                collectibleHeight,
                -arenaRadius + (row + 1) * spacing
            );
            
            finalCollectiblePositions[i] = position;
        }
    }
    
    /// <summary>
    /// Get grid position for a specific index
    /// </summary>
    Vector3 GetGridPosition(int index)
    {
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(numberOfCollectibles));
        float spacing = (arenaRadius * 2f) / (gridSize + 1);
        
        int row = index / gridSize;
        int col = index % gridSize;
        
        return new Vector3(
            -arenaRadius + (col + 1) * spacing,
            collectibleHeight,
            -arenaRadius + (row + 1) * spacing
        );
    }
    
    /// <summary>
    /// Initialize tutorial positions (4 corners of arena)
    /// </summary>
    void InitializeTutorialPositions()
    {
        if (usePredefinedTutorialPositions)
        {
            float cornerDistance = arenaRadius * 0.7f; // Slightly inside the arena edges
            tutorialPositions[0] = new Vector3(cornerDistance, collectibleHeight, cornerDistance);      // Top-right
            tutorialPositions[1] = new Vector3(-cornerDistance, collectibleHeight, cornerDistance);     // Top-left
            tutorialPositions[2] = new Vector3(-cornerDistance, collectibleHeight, -cornerDistance);    // Bottom-left
            tutorialPositions[3] = new Vector3(cornerDistance, collectibleHeight, -cornerDistance);     // Bottom-right
        }
        // If not using predefined, the tutorialPositions array should be set manually in inspector
    }
    
    /// <summary>
    /// Setup references to collectible objects
    /// </summary>
    void SetupCollectibleReferences()
    {
        // Clear existing lists
        experimentCollectibles.Clear();
        tutorialCollectibles.Clear();
        
        // Option 1: Use manually assigned references
        if (manualCollectibleReferences.Length >= numberOfCollectibles && manualCollectibleReferences[0] != null)
        {
            for (int i = 0; i < numberOfCollectibles; i++)
            {
                if (manualCollectibleReferences[i] != null)
                {
                    GameObject collectible = manualCollectibleReferences[i].gameObject;
                    experimentCollectibles.Add(collectible);
                }
            }
            Debug.Log("Using manually assigned collectible references");
        }
        // Option 2: Loop over children of collectibles parent
        else if (collectiblesParent != null)
        {
            for (int i = 0; i < collectiblesParent.childCount && i < numberOfCollectibles; i++)
            {
                GameObject collectible = collectiblesParent.GetChild(i).gameObject;
                experimentCollectibles.Add(collectible);
            }
            Debug.Log($"Found {experimentCollectibles.Count} collectibles from parent object");
        }
        // Option 3: Create collectibles from prefab
        else if (collectiblePrefab != null)
        {
            CreateCollectiblesFromPrefab();
            Debug.Log("Created collectibles from prefab");
        }
        else
        {
            Debug.LogError("No collectible references found! Please assign either manualCollectibleReferences, collectiblesParent, or collectiblePrefab");
        }
        
        // Create tutorial collectibles
        CreateTutorialCollectibles();
        
        // Position all collectibles and deactivate them
        PositionAndDeactivateCollectibles();
    }
    
    /// <summary>
    /// Create collectibles from prefab if no existing ones are found
    /// </summary>
    void CreateCollectiblesFromPrefab()
    {
        // Create parent if it doesn't exist
        if (collectiblesParent == null)
        {
            GameObject parentObj = new GameObject("Collectibles");
            collectiblesParent = parentObj.transform;
        }
        
        for (int i = 0; i < numberOfCollectibles; i++)
        {
            GameObject newCollectible = Instantiate(collectiblePrefab, collectiblesParent);
            newCollectible.name = $"Collectible_{i:00}";
            experimentCollectibles.Add(newCollectible);
        }
    }
    
    /// <summary>
    /// Create tutorial collectibles
    /// </summary>
    void CreateTutorialCollectibles()
    {
        if (collectiblePrefab == null) return;
        
        // Create tutorial parent if it doesn't exist
        Transform tutorialParent = transform.Find("TutorialCollectibles");
        if (tutorialParent == null)
        {
            GameObject tutorialParentObj = new GameObject("TutorialCollectibles");
            tutorialParentObj.transform.SetParent(transform);
            tutorialParent = tutorialParentObj.transform;
        }
        
        for (int i = 0; i < 4; i++)
        {
            GameObject tutorialCollectible = Instantiate(collectiblePrefab, tutorialParent);
            tutorialCollectible.name = $"TutorialCollectible_{i}";
            tutorialCollectibles.Add(tutorialCollectible);
        }
    }
    
    /// <summary>
    /// Position all collectibles at their designated positions and deactivate them
    /// </summary>
    void PositionAndDeactivateCollectibles()
    {
        // Position and deactivate experiment collectibles
        for (int i = 0; i < experimentCollectibles.Count; i++)
        {
            if (i < finalCollectiblePositions.Length)
            {
                experimentCollectibles[i].transform.position = finalCollectiblePositions[i];
            }
            experimentCollectibles[i].SetActive(false);
        }
        
        // Position and deactivate tutorial collectibles
        for (int i = 0; i < tutorialCollectibles.Count; i++)
        {
            if (i < tutorialPositions.Length)
            {
                tutorialCollectibles[i].transform.position = tutorialPositions[i];
            }
            tutorialCollectibles[i].SetActive(false);
        }
        
        Debug.Log("All collectibles positioned and deactivated");
    }
    
    /// <summary>
    /// Setup the tutorial phase
    /// </summary>
    void SetupTutorialPhase()
    {
        isInTutorialPhase = true;
        tutorialCollectiblesCollected = 0;
        
        // Activate first tutorial collectible
        if (tutorialCollectibles.Count > 0)
        {
            tutorialCollectibles[0].SetActive(true);
        }
        
        UpdateUI();
        Debug.Log("Tutorial phase started");
    }
    
    /// <summary>
    /// Update UI text to show current progress and phase
    /// </summary>
    void UpdateUI()
    {
        if (progressText != null)
        {
            if (isInTutorialPhase)
            {
                progressText.text = $"Tutorial: {tutorialCollectiblesCollected}/4 collectibles";
            }
            else
            {
                progressText.text = $"Experiment: {currentCollectibleIndex}/{numberOfCollectibles} collectibles";
            }
        }
        
        if (phaseText != null)
        {
            phaseText.text = isInTutorialPhase ? "Tutorial Phase" : "Experiment Phase";
        }
        
        if (startExperimentButton != null)
        {
            startExperimentButton.SetActive(isInTutorialPhase && tutorialCollectiblesCollected >= 4);
        }
    }
    
    // Public methods for external access
    public int GetCurrentCollectibleIndex() => currentCollectibleIndex;
    public int GetTotalCollectibles() => numberOfCollectibles;
    public bool IsInTutorialPhase() => isInTutorialPhase;
    public bool IsExperimentActive() => experimentActive;
    public Vector3[] GetCollectiblePositions() => finalCollectiblePositions;
    public List<GameObject> GetExperimentCollectibles() => experimentCollectibles;
    public List<GameObject> GetTutorialCollectibles() => tutorialCollectibles;
}