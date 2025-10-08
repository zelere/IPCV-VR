# Roll-A-Ball with Ultraleap Hand Tracking

A Unity implementation of the classic Roll-a-Ball game enhanced with hand tracking capabilities using Ultraleap (formerly Leap Motion) technology. This project demonstrates advanced gesture-based interaction in 3D gaming environments.

## 🎮 Project Overview

This project extends the traditional Roll-a-Ball game by replacing keyboard controls with intuitive hand tracking gestures. Players can now control the ball using natural hand movements and even "cheat" by picking up the ball with pinch gestures.

### Key Features

- **Gesture-based Ball Control**: Point with your right hand's index finger to move the ball
- **Pinch-to-Pick**: Use your left hand to pinch and pick up the ball
- **Dual-hand Interaction**: Right hand for pointing/directing, left hand for picking up
- **Robust Hand Tracking**: Implemented with error handling and frame-based stability

## 🛠️ Technical Implementation

### Hand Tracking System

The core implementation is found in `Assets/Scripts/BallBehaviour.cs`, which integrates with the Ultraleap SDK to provide:

1. **Right Hand Pointing Control**
   - Tracks index finger tip position
   - Applies force toward finger position
   - Includes drag effects based on hand velocity
   - Frame-based stability (requires 5 consecutive frames)

2. **Left Hand Pinch Control**
   - Palm position-based pinch detection
   - Pinch strength threshold system
   - Distance-based pickup validation
   - Kinematic physics during pinched state

3. **Robust Error Handling**
   - Hand loss tolerance (30 frames for pinch, 15 for pointing)
   - Reflection-based API access for SDK compatibility
   - Fallback mechanisms for missing hand data


### Key Technical Challenges Solved

1. **Hand Tracking Precision Issues** - Ultraleap's low precision causing jittery ball movement
2. **Limited Field of View** - Small tracking area restricting gameplay interactions
3. **Hand Loss Handling** - Temporary tracking loss causing abrupt control interruptions
4. **Unstable Hand Detection** - Intermittent detection causing erratic ball behavior
5. **Overshooting Ball Movement** - Excessive force application without distance consideration
6. **Pinch Detection Reliability** - Difficult and inconsistent pinch gesture recognition

## 🚧 Issues Encountered and Solutions

### Issue 1: Hand Tracking Precision Issues
- **Problem**: Ultraleap's relatively low precision made fine control difficult
- **Root Cause**: Raw sensor data caused jittery and unpredictable ball movement
- **Solution**: Implemented smoothing algorithms and force damping
- **Code Implementation**:
```csharp
// Distance-based force reduction with maximum limits
float forceMagnitude = Mathf.Min(distance * moveForce, maxForce);
Vector3 force = dir.normalized * forceMagnitude * Time.deltaTime;
rb.AddForce(force, ForceMode.VelocityChange);
```

### Issue 2: Limited Field of View
- **Problem**: Small tracking area restricted gameplay
- **Root Cause**: Ultraleap's limited detection range when sitting close to the sensor made interactions difficult
- **Solution**: Optimized gesture thresholds and added larger pickup distances
- **Code Implementation**:
```csharp
public float pinchDistance = 2.0f; // Increased detection range
// Fallback with larger distance for easier pickup
startPinch = dist <= pinchDistance * 1.5f;
```

### Issue 3: Hand Loss Handling
- **Problem**: Temporary hand loss caused the ball to roll away or drop even when hand was lost only for a few frames
- **Root Cause**: No tolerance for brief tracking interruptions
- **Solution**: Frame-based tolerance system and gradual slowdown once hand is gone
- **Code Implementation**:
```csharp
public int handLostFramesTolerance = 30; // Frames to wait before releasing
public float slowdownRate = 0.95f; // Gradual slowdown rate

if (rightHandLostFrameCount >= rightHandLostFramesTolerance && rb != null)
{
    rb.velocity *= slowdownRate;
    rb.angularVelocity *= slowdownRate;
}
```

### Issue 4: Hand Tracking Stability and Loss Handling
- **Problem**: Intermittent hand detection and brief hand loss caused erratic ball behavior and immediate control cessation
- **Root Cause**: Single-frame hand detection without stability checks and no tolerance for temporary tracking loss
- **Solution**: Implemented frame-based stability system with tolerance for temporary hand loss
- **Code Implementation**:
```csharp
// Stability requirements for hand detection
public int requiredFrames = 5; // Number of frames hand must be present
public int handLostFramesTolerance = 30; // Frames to wait before releasing
public int rightHandLostFramesTolerance = 15; // Frames for pointing control
public float slowdownRate = 0.95f; // Gradual slowdown rate

// Right hand stability check
if (rightHandFrameCount >= requiredFrames)
{
    // Only then allow hand control
    if (TryGetIndexTipPosition(rightHand, out Vector3 tipWorld))
    {
        // Apply ball movement
    }
}

// Left hand loss tolerance for pinch
if (leftHandLostFrameCount >= handLostFramesTolerance)
{
    Debug.Log("Left hand lost for too long, releasing pinch");
    ReleasePinch();
}
else
{
    // Keep ball at last known position while hand is temporarily lost
    Vector3 target = lastKnownPalmPosition + pinchOffset;
    transform.position = Vector3.Lerp(transform.position, target, Time.deltaTime * followSmoothness * 0.5f);
}

// Right hand loss handling with gradual slowdown
if (rightHandLostFrameCount >= rightHandLostFramesTolerance && rb != null)
{
    rb.velocity *= slowdownRate;
    rb.angularVelocity *= slowdownRate;
}
```

### Issue 5: Overshooting Ball Movement
- **Problem**: Ball moved too aggressively toward finger position
- **Root Cause**: Excessive force application without distance consideration
- **Solution**: Distance-based force damping with maximum force limits
- **Code Implementation**:
```csharp
public float maxForce = 10f; // Maximum force to prevent overshooting

// Reduce force based on distance to prevent overshooting
float forceMagnitude = Mathf.Min(distance * moveForce, maxForce);
Vector3 force = dir.normalized * forceMagnitude * Time.deltaTime;
rb.AddForce(force, ForceMode.VelocityChange);
```

### Issue 6: Pinch Detection Reliability
- **Problem**: Pinch gestures were difficult to perform consistently
- **Root Cause**: High pinch strength threshold and small pickup distance
- **Solution**: Lowered thresholds and increased detection range
- **Code Implementation**:
```csharp
public float pinchThreshold = 0.5f; // Lowered from 0.8f for easier pinching
public float pinchDistance = 2.0f; // Increased range

// Lower threshold and larger distance for easier pickup
startPinch = (pinchStrength > pinchThreshold) && dist <= pinchDistance;
```

## 🎯 Advanced Interaction Techniques

### Implemented Solutions for Ultraleap Limitations

#### 1. **Dual-Hand Paradigm**
- **Rationale**: Separates control and manipulation functions
- **Implementation**: Right hand for direction, left hand for pickup
- **Benefit**: Reduces gesture conflicts and improves precision

#### 2. **Velocity-Based Drag**
- **Rationale**: Adds momentum to ball movement
- **Implementation**: Tracks hand velocity and applies drag forces
- **Benefit**: More natural and predictable ball physics

#### 3. **Grace Period System**
- **Rationale**: Prevents abrupt stops after hand interactions
- **Implementation**: 2-second grace period after pinch release
- **Benefit**: Smoother transition between interaction modes

#### 4. **Adaptive Thresholds**
- **Rationale**: Accommodates varying hand sizes and positions
- **Implementation**: Multiple fallback detection methods
- **Benefit**: Improved accessibility and reliability

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── BallBehaviour.cs      # Main hand tracking implementation
│   ├── GameBehaviour.cs      # Game state management
│   ├── CameraBehaviour.cs    # Camera controls
│   └── UIBehaviour.cs        # User interface
├── Scenes/                   # Unity scenes
├── Materials/                # Ball and environment materials
└── Samples/                  # Ultraleap SDK examples
    └── Ultraleap Tracking/
        └── 7.2.0/            # SDK version 7.2.0
```

## 🎮 How to Play

1. **Setup**: Ensure Ultraleap device is connected and positioned correctly
2. **Movement**: Point with your right hand's index finger to direct the ball
3. **Pickup**: Pinch with your left hand near the ball to pick it up
4. **Release**: Open your left hand to drop the ball
5. **Collect**: Gather all collectible objects to win

## 🔧 Setup Instructions

### Prerequisites
- Unity 2022.x
- Ultraleap device (Leap Motion Controller)
- Ultraleap SDK 7.2.0

### Installation
1. Clone this repository
2. Install Ultraleap Tracking from Unity Package Manager
3. Connect your Ultraleap device
4. Open the project in Unity
5. Run the main scene

## 🎥 Demo

Demo of the implementation is available at: https://youtu.be/XzdozDj5kb4

## 🚀 Future Improvements

### Proposed Enhancements

1. **Gesture Recognition**
   - Implement swipe gestures for ball launching
   - Add hand orientation for spin control

2. **Adaptive Sensitivity**
   - Dynamic threshold adjustment based on user performance
   - Personal calibration system

3. **Visual Feedback**
   - Hand tracking overlay in game view
   - Gesture strength indicators

4. **Multi-player Support**
   - Two-player mode with separate hand assignment
   - Competitive ball control

## 👥 Authors

Regina Zelei
Sevda Ibrahim

