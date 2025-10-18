# Tutorial Phase Implementation Guide

## Overview
This guide explains how to implement the tutorial phase for your VR study, including setting up 4 tutorial collectibles in arena corners, automatic control mode selection, and smooth transition to the main study.

## Implementation Steps

### 1. Scene Setup

#### A. Create Tutorial Collectibles
1. **Create 4 tutorial collectible objects:**
   - Duplicate your existing collectible prefab 4 times
   - Position them at the 4 corners of your arena
   - Rename them to: `TutorialCollectible_1`, `TutorialCollectible_2`, etc.
   - Change their tag from "Collectible" to "TutorialCollectible"

2. **Positioning (approximate coordinates, adjust based on your arena):**
   - Corner 1: (4, 0.5, 4)
   - Corner 2: (-4, 0.5, 4)  
   - Corner 3: (-4, 0.5, -4)
   - Corner 4: (4, 0.5, -4)

#### B. Update Collectible Scripts
1. **Replace CubeBehaviour on tutorial collectibles:**
   - Remove the `CubeBehaviour` component from tutorial collectibles
   - Add the `TutorialCollectibleBehaviour` component instead

#### C. Create Tutorial Complete Menu
1. **Create a new Canvas/Panel for Tutorial Complete Menu:**
   ```
   TutorialCompleteMenu (GameObject)
   ├── Panel (Image)
   │   ├── Title (Text) - "Tutorial Complete!"
   │   ├── SelectedModeText (Text) - "Control mode selected: [Mode]"
   │   ├── StartStudyButton (Button) - "Start Study"
   │   └── BackToMenuButton (Button) - "Back to Menu"
   ```

2. **Update Start Menu:**
   - Add a "Start Tutorial" button to your existing start menu
   - Keep the existing "Start with Hand Tracking" and "Start with Keyboard" buttons for direct study access

### 2. Component Setup

#### A. GameBehaviour Component
1. **Assign new references in the inspector:**
   - `Tutorial Complete Menu`: Assign your new tutorial complete menu GameObject
   - `Tutorial Menu Manager`: Will be found automatically or assign manually

#### B. UIBehaviour Component  
1. **Add Tutorial Text reference:**
   - `Tutorial Text`: Assign a Text component to show tutorial-specific messages

#### C. TutorialSetup Component (Optional)
1. **Add to an empty GameObject in your scene:**
   - `Collectible Prefab`: Assign your collectible prefab
   - `Arena Center`: Assign your arena center transform (or leave empty for Vector3.zero)
   - `Arena Size`: Set to match your arena size (default: 10)
   - `Collectible Height`: Height above ground (default: 0.5)

2. **Use Context Menu options:**
   - Right-click component → "Create Tutorial Collectibles" (creates 4 collectibles automatically)
   - Right-click component → "Remove Tutorial Collectibles" (removes all tutorial collectibles)

#### D. TutorialMenuManager Component
1. **Add to an empty GameObject in your scene:**
   - Assign all button references from your menus
   - The GameBehaviour reference will be found automatically

### 3. Tags Setup
Make sure you have these tags in your project:
- `Collectible` (for study collectibles)
- `TutorialCollectible` (for tutorial collectibles)
- `Player` (for the ball/player object)

### 4. Game Flow

The new game flow will be:
1. **Start Menu** → Choose "Start Tutorial" or direct study options
2. **Tutorial Phase** → Collect 4 collectibles in corners
3. **Tutorial Complete Menu** → Shows selected control mode, button to start study
4. **Study Phase** → Collect all 12 collectibles with automatically selected control mode
5. **Victory** → Return to start menu

### 5. Control Mode Selection

The tutorial phase automatically selects a control mode for the study:
- Currently uses random selection (50/50 between Keyboard and Hand Tracking)
- You can modify the selection logic in `GameBehaviour.OnTutorialComplete()`
- Consider implementing based on:
  - Participant ID (even/odd)
  - Performance metrics from tutorial
  - External configuration
  - Counterbalancing requirements

### 6. Customization Options

#### A. Tutorial Instructions
Modify the tutorial text in `UIBehaviour.UpdateCollectiblesText()`:
```csharp
tutorialText.text = "Your custom tutorial instructions here";
```

#### B. Control Mode Selection Logic
In `GameBehaviour.OnTutorialComplete()`, replace random selection:
```csharp
// Example: Use participant ID for counterbalancing
int participantId = GetParticipantId(); // Implement this method
selectedControlMode = (participantId % 2 == 0) ? 
    BallBehaviour.InteractionMode.Keyboard : 
    BallBehaviour.InteractionMode.HandTracking;
```

#### C. Tutorial Collectible Positioning
Modify positions in `TutorialSetup.CreateTutorialCollectibles()` or manually position in scene.

### 7. Testing Checklist

- [ ] Tutorial collectibles appear only during tutorial phase
- [ ] Study collectibles appear only during study phase  
- [ ] Control mode is automatically selected after tutorial
- [ ] Transition button works correctly
- [ ] UI text updates appropriately for each phase
- [ ] Player can return to menu from any phase
- [ ] Collectible counts reset properly between phases

### 8. Additional Features to Consider

1. **Tutorial Performance Tracking:**
   - Time to complete tutorial
   - Number of attempts per collectible
   - Movement patterns analysis

2. **Adaptive Control Selection:**
   - Base selection on tutorial performance
   - Consider user preferences or difficulties

3. **Transition Animations:**
   - Smooth fade between phases
   - Visual feedback for mode selection

4. **Instructions Display:**
   - Show control instructions during tutorial
   - Contextual help based on selected mode

## Troubleshooting

### Common Issues:
1. **Tutorial collectibles not showing:** Check tag assignment and GameBehaviour references
2. **Control mode not applying:** Verify BallBehaviour.SetInteractionMode() is being called
3. **Menu transitions not working:** Check button event assignments in TutorialMenuManager
4. **Collectible counts wrong:** Ensure proper tag usage and UIBehaviour references

### Debug Tips:
- Check Console for debug messages during phase transitions
- Verify all GameObjects have correct tags
- Test each phase transition manually using debug buttons
- Confirm all script references are properly assigned