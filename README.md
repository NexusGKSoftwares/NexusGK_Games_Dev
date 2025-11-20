# 3D Endless Runner Game - Unity Project

A 3D endless runner game set in a temple/jungle environment where the player automatically runs forward, dodging obstacles and collecting coins and power-ups.

## Features

- **Automatic Forward Movement**: Player continuously runs forward
- **Lane-based Movement**: Move left/right between 3 lanes
- **Jump and Slide Mechanics**: Avoid obstacles with jumping and sliding
- **Third-Person Camera**: Smooth camera following the player
- **Obstacle System**: Randomly spawning obstacles in lanes
- **Coin Collection**: Collect coins scattered across lanes
- **Power-ups**: Various power-ups including speed boost, shield, magnet, and double coins
- **Progressive Difficulty**: Game speed and obstacle spawn rate increase over time
- **Score System**: Score based on distance traveled and coins collected
- **High Score Tracking**: Persistent high score storage
- **Smooth Animations**: Player animations for jump and slide
- **Particle Effects**: Visual effects for jumps, slides, and coin collection
- **HUD Display**: Shows score, high score, and coins collected
- **Game Over System**: Restart functionality with game over screen

## Project Structure

```
Assets/
└── Scripts/
    ├── Player/
    │   └── PlayerController.cs          # Main player movement and input handling
    ├── Camera/
    │   └── CameraController.cs          # Third-person camera follow system
    ├── Managers/
    │   ├── GameManager.cs               # Core game logic, scoring, difficulty
    │   ├── HUDManager.cs                # UI management and display
    │   └── PowerUpManager.cs            # Power-up effects management
    ├── Obstacles/
    │   ├── ObstacleSpawner.cs           # Random obstacle spawning system
    │   └── ObstacleMovement.cs          # Obstacle movement behavior
    ├── Collectibles/
    │   ├── Coin.cs                      # Coin collectible behavior
    │   ├── CoinSpawner.cs               # Coin spawning system
    │   └── PowerUp.cs                   # Power-up collectible behavior
    └── World/
        ├── InfiniteGround.cs            # Infinite ground tile system
        └── MovingEnvironment.cs         # Environment movement system
```

## Setup Instructions

### 1. Scene Setup

1. Create a new Unity scene (3D)
2. Set up the scene with a ground plane or terrain for the temple/jungle environment

### 2. Player Setup

1. Create a Player GameObject:
   - Add a **Capsule Collider** component
   - Add a **Rigidbody** component:
     - Freeze Rotation: X, Y, Z
     - Drag: 5-10 (for better control)
   - Add the **PlayerController** script
   - Tag as "Player"
   - Add an **Animator** component (optional, for animations)

2. Configure PlayerController:
   - Set `forwardSpeed` (default: 10)
   - Set `laneChangeSpeed` (default: 5)
   - Set `jumpForce` (default: 10)
   - Set `laneDistance` (default: 3) - distance between lanes
   - Configure ground check settings
   - Assign jump and slide particle effects (optional)
   - Assign animator (if using animations)

### 3. Camera Setup

1. Create a Main Camera or use the default one
2. Add the **CameraController** script
3. Assign the Player transform as the target
4. Adjust offset, follow speed, and rotation speed in the inspector

### 4. Game Manager Setup

1. Create an empty GameObject named "GameManager"
2. Add the **GameManager** script
3. Configure:
   - `baseSpeed`: Starting game speed
   - `maxSpeed`: Maximum game speed
   - `speedIncreaseRate`: How fast speed increases
   - `coinScoreValue`: Points per coin
   - Difficulty settings

### 5. HUD Setup

1. Create a Canvas for UI
2. Add UI elements:
   - Text/TextMeshPro for Score
   - Text/TextMeshPro for High Score
   - Text/TextMeshPro for Coins
   - Panel for Game Over screen
   - Buttons for Restart and Quit

3. Add **HUDManager** script to a GameObject (can be on Canvas)
4. Assign all UI references in the inspector

### 6. Obstacle System Setup

1. Create obstacle prefabs (cubes, spheres, or custom models)
   - Add Collider components
   - Tag as "Obstacle"
   - Optionally add **ObstacleMovement** script

2. Create an empty GameObject named "ObstacleSpawner"
3. Add **ObstacleSpawner** script
4. Configure:
   - Assign obstacle prefabs array
   - Set spawn distance and intervals
   - Set lane distance (should match PlayerController)

### 7. Coin System Setup

1. Create a Coin prefab:
   - Use a yellow sphere, coin model, or custom asset
   - Add a **Sphere Collider** (set as trigger)
   - Add the **Coin** script
   - Tag as "Coin"
   - Optional: Add particle effects and audio

2. Create an empty GameObject named "CoinSpawner"
3. Add **CoinSpawner** script
4. Assign the coin prefab
5. Configure spawn settings

### 8. Power-Up Setup

1. Create power-up prefabs (similar to coins)
2. Add **PowerUp** script to each prefab
3. Set power-up type in inspector
4. Create a GameObject with **PowerUpManager** script (or it will auto-create)

### 9. Ground/Environment Setup

1. Create ground tiles or use terrain
2. For infinite scrolling:
   - Create a ground tile prefab
   - Add **MovingEnvironment** script to moving parts
   - Or use **InfiniteGround** script for automatic tile management

3. Set up a Ground layer for ground checking:
   - Create a "Ground" layer
   - Assign ground objects to this layer
   - Set the layer mask in PlayerController

### 10. Lighting and Environment

1. Add appropriate lighting for temple/jungle theme
2. Add environmental objects (trees, temple structures, etc.)
3. Ensure proper shadows and lighting settings

### 11. Animation Setup (Optional)

1. Import or create animations for:
   - Jump
   - Slide
   - Running

2. Create an Animator Controller
3. Set up animation parameters:
   - "Jump" (trigger)
   - "Slide" (trigger)
   - "IsGrounded" (bool)

4. Assign to player's Animator component

### 12. Particle Effects (Optional)

1. Create particle systems for:
   - Jump effect
   - Slide effect
   - Coin collection effect

2. Assign to appropriate scripts via inspector

## Controls

- **A / Left Arrow**: Move left
- **D / Right Arrow**: Move right
- **W / Up Arrow / Space**: Jump
- **S / Down Arrow**: Slide

## Tags and Layers Setup

Required tags:
- "Player"
- "Obstacle"
- "Coin"

Required layers:
- "Ground" (for ground checking)

## Customization

### Adjust Difficulty
- Edit `GameManager.cs`: Modify speed increase rate, spawn intervals
- Edit `ObstacleSpawner.cs`: Adjust spawn rates and intervals

### Add More Lanes
- Change `numberOfLanes` in both `PlayerController` and `ObstacleSpawner`

### Modify Movement
- Adjust speeds in `PlayerController.cs`
- Modify camera settings in `CameraController.cs`

### Add New Power-Ups
- Extend `PowerUpType` enum in `PowerUp.cs`
- Add logic in `PowerUpManager.cs`

## Notes

- The player moves forward automatically, and obstacles/coins move backward
- Ensure all scripts reference each other correctly (GameManager singleton pattern)
- Test ground checking works properly with your ground setup
- Adjust collider sizes for proper hit detection
- The game uses a singleton pattern for GameManager - only one should exist in the scene

## Troubleshooting

**Player doesn't move:**
- Check Rigidbody component is present
- Verify PlayerController script is attached
- Check that GameManager exists and IsGameActive is true

**Camera doesn't follow:**
- Assign player transform to CameraController target
- Check camera position and offset values

**Obstacles don't spawn:**
- Verify ObstacleSpawner has prefabs assigned
- Check spawn distance settings
- Ensure GameManager.IsGameActive is true

**Ground check not working:**
- Verify Ground layer is created and assigned
- Check ground check radius and position
- Ensure ground objects have colliders

Enjoy your endless runner game!

