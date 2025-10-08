using System.Collections;
using UnityEngine;

public class InteractionTestManager : MonoBehaviour
{
    [Header("Testing")]
    public BallBehaviour ballBehaviour;
    public MenuManager menuManager;
    public bool enableAutomaticTesting = false;
    public float testSwitchInterval = 10f;
    
    private void Start()
    {
        if (ballBehaviour == null)
            ballBehaviour = FindObjectOfType<BallBehaviour>();
            
        if (menuManager == null)
            menuManager = FindObjectOfType<MenuManager>();
            
        if (enableAutomaticTesting)
        {
            StartCoroutine(AutomaticModeTestingCoroutine());
        }
    }
    
    private void Update()
    {
        // Manual testing with number keys
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            TestHandTrackingMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            TestKeyboardMode();
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            TestModeSwitch();
        }
        
        // Display current mode info
        if (Input.GetKeyDown(KeyCode.I))
        {
            DisplayModeInfo();
        }
    }
    
    public void TestHandTrackingMode()
    {
        Debug.Log("=== Testing Hand Tracking Mode ===");
        if (ballBehaviour != null)
        {
            ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.HandTracking);
            Debug.Log("Hand Tracking mode activated. Use Leap Motion to control the ball.");
        }
    }
    
    public void TestKeyboardMode()
    {
        Debug.Log("=== Testing Keyboard Mode ===");
        if (ballBehaviour != null)
        {
            ballBehaviour.SetInteractionMode(BallBehaviour.InteractionMode.Keyboard);
            Debug.Log("Keyboard mode activated. Use WASD/Arrow keys to control the ball.");
        }
    }
    
    public void TestModeSwitch()
    {
        if (ballBehaviour == null) return;
        
        Debug.Log("=== Testing Mode Switch ===");
        
        // Switch between modes
        if (ballBehaviour.currentInteractionMode == BallBehaviour.InteractionMode.HandTracking)
        {
            TestKeyboardMode();
        }
        else
        {
            TestHandTrackingMode();
        }
    }
    
    public void DisplayModeInfo()
    {
        if (ballBehaviour == null) return;
        
        Debug.Log($"=== Current Mode Info ===");
        Debug.Log($"Current Interaction Mode: {ballBehaviour.currentInteractionMode}");
        Debug.Log($"Ball Position: {ballBehaviour.transform.position}");
        Debug.Log($"Ball Velocity: {ballBehaviour.GetComponent<Rigidbody>().velocity}");
    }
    
    private IEnumerator AutomaticModeTestingCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(testSwitchInterval);
            TestModeSwitch();
            DisplayModeInfo();
        }
    }
    
    private void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 200));
        GUILayout.Label("=== Interaction Test Manager ===");
        GUILayout.Label($"Current Mode: {(ballBehaviour ? ballBehaviour.currentInteractionMode.ToString() : "N/A")}");
        GUILayout.Space(10);
        
        GUILayout.Label("Manual Testing:");
        GUILayout.Label("Press 1 - Hand Tracking Mode");
        GUILayout.Label("Press 2 - Keyboard Mode"); 
        GUILayout.Label("Press 3 - Switch Mode");
        GUILayout.Label("Press I - Display Info");
        
        GUILayout.Space(10);
        if (GUILayout.Button("Test Hand Tracking"))
        {
            TestHandTrackingMode();
        }
        
        if (GUILayout.Button("Test Keyboard"))
        {
            TestKeyboardMode();
        }
        
        if (GUILayout.Button("Switch Mode"))
        {
            TestModeSwitch();
        }
        
        GUILayout.EndArea();
    }
}