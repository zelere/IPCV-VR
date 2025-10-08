# Experiment Setup Guide - Step 3.1 Implementation

## Overview
This implementation creates a controlled experiment progression where collectibles appear one at a time in a predetermined sequence, ensuring consistent conditions for all participants.

## ✅ Step 3.1 - Initialization of Sequence Variables

### What's Implemented:

#### 1. **Position Management System**
- **Manual Positions**: Set 12 specific positions via inspector
- **Random Positions**: Generate random positions within arena constraints
- **Grid Positions**: Fallback systematic positioning
- **Tutorial Positions**: 4 corner positions for training phase

#### 2. **Collectible Reference System**
Three flexible approaches:
- **Manual References**: Drag-and-drop 12 Transform references in inspector
- **Parent Loop**: Automatically find collectibles from parent object children
- **Prefab Creation**: Generate collectibles from prefab if none exist

#### 3. **Configuration Options**
```csharp
[Header("Experiment Configuration")]
private int numberOfCollectibles = 12;
private bool useRandomPositions = true;
private bool useManualPositions = false;

[Header("Random Position Configuration")]
private float arenaRadius = 4.0f;
private float minDistanceBetweenCollectibles = 1.5f;
private float collectibleHeight = 0.5f;
```

## 🛠 Setup Instructions

### Option 1: Manual Position Setup
1. **Create ExperimentManager**:
   - Add `ExperimentManager.cs` script to Canvas or dedicated GameObject
   - Set `useManualPositions = true`
   - Set `useRandomPositions = false`

2. **Configure Manual Positions**:
   - Expand "Manual Position Setup" in inspector
   - Set all 12 positions in `manualCollectiblePositions` array
   - Example positions:
   ```
   Position 0: (2, 0.5, 2)
   Position 1: (-2, 0.5, 2)
   Position 2: (-2, 0.5, -2)
   Position 3: (2, 0.5, -2)
   Position 4: (0, 0.5, 3)
   Position 5: (3, 0.5, 0)
   Position 6: (0, 0.5, -3)
   Position 7: (-3, 0.5, 0)
   Position 8: (1, 0.5, 1)
   Position 9: (-1, 0.5, 1)
   Position 10: (-1, 0.5, -1)
   Position 11: (1, 0.5, -1)
   ```

### Option 2: Existing Collectibles Setup
1. **Assign Collectibles Parent**:
   - Drag your "Collectibles" GameObject to `collectiblesParent` field
   - Script will automatically loop through children

2. **Manual References** (Alternative):
   - Expand "Manual Collectible References"
   - Drag each collectible Transform to the array

### Option 3: Automatic Creation
1. **Assign Collectible Prefab**:
   - Drag your collectible prefab to `collectiblePrefab` field
   - Script will create 12 collectibles automatically
   - Positions them according to selected positioning method

## 🎮 Key Features Implemented

### **Position Generation**
- **Random with constraints**: Minimum distance between collectibles
- **Arena bounds checking**: Keeps collectibles within playable area
- **Fallback system**: Grid positioning if random generation fails

### **Tutorial System**
- **4 corner positions**: Predefined tutorial locations
- **Separate management**: Tutorial collectibles independent from experiment
- **Progress tracking**: Counts tutorial completion

### **Flexible Configuration**
```csharp
// Core experiment variables
private Vector3[] finalCollectiblePositions;     // 12 main positions
private Vector3[] tutorialPositions;             // 4 tutorial positions  
private List<GameObject> experimentCollectibles; // Main experiment objects
private List<GameObject> tutorialCollectibles;   // Tutorial objects
private int currentCollectibleIndex = 0;         // Progress tracker
```

### **Validation System**
- **Distance checking**: Ensures minimum spacing between collectibles
- **Bounds validation**: Keeps positions within arena
- **Reference verification**: Checks for valid collectible assignments

## 📋 Next Steps (3.2 & 3.3)

### Ready for Implementation:
1. **Sequence Control** (Step 3.2):
   - `currentCollectibleIndex` tracks progress
   - `experimentCollectibles` list ready for activation
   - Position arrays prepared for sequential appearance

2. **Tutorial Phase** (Step 3.3):
   - Tutorial positions defined (4 corners)
   - `tutorialCollectibles` list ready
   - Phase tracking with `isInTutorialPhase` boolean

## 🧪 Testing Your Setup

### Validation Methods:
```csharp
// Public getters for testing
public Vector3[] GetCollectiblePositions()
public List<GameObject> GetExperimentCollectibles()
public List<GameObject> GetTutorialCollectibles()
public int GetTotalCollectibles()
```

### Debug Information:
- Console logs show initialization progress
- Position generation method used
- Number of collectibles found/created
- Phase and progress tracking

## 🎯 Why This Approach?

### **Study Control Benefits**:
1. **Consistent Sequence**: Every participant experiences identical collectible order
2. **Controlled Variables**: Focus on interaction method, not collection strategy
3. **Measurable Progress**: Track timing and success rate per collectible
4. **Tutorial Standardization**: Same learning experience for all participants

### **Tutorial Phase Importance**:
- **Familiarization**: Participants learn controls without study pressure
- **Comfort Building**: Reduces anxiety about unfamiliar interface
- **Baseline Establishment**: Ensures minimum competency before data collection
- **Mode Selection**: Natural point to choose interaction technique

This implementation provides a solid foundation for controlled VR/interaction studies, ensuring reliable and comparable data across all participants.