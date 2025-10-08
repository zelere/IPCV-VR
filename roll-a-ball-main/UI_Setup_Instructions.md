# Roll-A-Ball UI Setup Instructions

## Setting Up Two-Button Interface for Interaction Selection

Follow these steps to modify your Unity scene to support choosing between Hand Tracking and Keyboard controls:

### Step 1: Modify the StartMenu in Unity Editor

1. **Open the Roll-A-Ball scene** in Unity Editor
2. **Find the Canvas object** in the hierarchy
3. **Locate the StartMenu GameObject** under the Canvas
4. **Delete or disable the existing Start button** (if any)

### Step 2: Create Two New Buttons

1. **Right-click on StartMenu** → UI → Button - TextMeshPro
2. **Rename the first button** to "HandTrackingButton"
3. **Create a second button** the same way
4. **Rename the second button** to "KeyboardButton"

### Step 3: Position and Style the Buttons

1. **Select HandTrackingButton**:
   - Set Position: X = -100, Y = 50, Z = 0
   - Set Size: Width = 180, Height = 40
   - Change button text to "Hand Tracking"

2. **Select KeyboardButton**:
   - Set Position: X = -100, Y = 0, Z = 0
   - Set Size: Width = 180, Height = 40
   - Change button text to "Keyboard Controls"

### Step 4: Add MenuManager Component

1. **Create an empty GameObject** in the scene and name it "MenuManager"
2. **Add the MenuManager script** to this GameObject
3. **Assign the following references** in the inspector:
   - Hand Tracking Button: Drag HandTrackingButton here
   - Keyboard Button: Drag KeyboardButton here
   - Start Menu: Drag StartMenu GameObject here
   - Instruction Text: Create a Text component for instructions (optional)
   - Game Manager: Drag the GameObject that has GameBehaviour script

### Step 5: Add Instruction Text (Optional)

1. **Right-click on StartMenu** → UI → Text - TextMeshPro
2. **Position it above the buttons**
3. **Set the text** to "Choose your interaction method:"
4. **Assign this text component** to the MenuManager's Instruction Text field

### Step 6: Update GameBehaviour References

1. **Find the GameObject with GameBehaviour script**
2. **Make sure it's assigned** to the MenuManager's Game Manager field

### Step 7: Test the Setup

1. **Play the scene**
2. **You should see two buttons** for choosing interaction methods
3. **Click each button** to test both interaction modes

## Alternative: Programmatic UI Creation

If you prefer to create the UI through code, you can use the `CreateUIElementsProgrammatically()` method in MenuManager. This will automatically create the required UI elements at runtime.

## Key Features

- **Hand Tracking Mode**: Uses Leap Motion hand tracking for ball control
- **Keyboard Mode**: Uses WASD/Arrow keys for ball movement
- **Clean Mode Switching**: Each mode properly initializes and cleans up
- **Visual Feedback**: Clear button labels indicate the interaction method

## Troubleshooting

- **Buttons not responding**: Check that the MenuManager script is properly assigned and button references are set
- **Game not starting**: Verify that the GameBehaviour script is found and assigned
- **Mode not switching**: Ensure the BallBehaviour script has the new interaction mode code

## Testing Both Modes

### Hand Tracking Mode
- Ensure Leap Motion device is connected
- Use left hand to pinch and grab the ball
- Use right hand index finger to point and guide the ball

### Keyboard Mode
- Use WASD or Arrow Keys to move the ball
- Ball movement is force-based for realistic physics