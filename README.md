# Surviving the apocalypse

This is an unfinished 3D survival game project that I developed as a school project.

## The Most Important Game Features

- First person player movement
  - Smooth moving (acceleration and deceleration)
  - Walking and sprinting
- Enemy movement
  - Made using the Unity's navigation system (pathfinding)
  - Enemies have multiple states with different behaviours (for exampe patrolling and chasing the player)
  - Enemies check if they can see the player which affects the state transitions
  - Multiple enemies can group to gether when they go near each other
  - Enemies can attack the player
  - Enemies spawn randomly to the world
- Item system
  - Different item types with different functionalities can be registered to the system
  - Inventory for storing the player's items
  - Items of a usable item type can be used
  - Building system (item placing) for placeable item types
  - Item crafting system with crafting stations
- Tree chopping
- Health system
  - Both the player and the enemies use the system
  - Health display implementations for both the player and the enemies
- Day-night cycle
- Visual effects
  - Post processing
  - Fog
  - Lighting

## The Development of the Game

- I developed the game to its current state in about eight weeks. The project was so big that I didn't have time to add actual content (for example building the world and adding new item types).
- The graphic assets I used were mainly free assets downloaded from the internet. Despite this I made the inventory menu UI assets myself.
