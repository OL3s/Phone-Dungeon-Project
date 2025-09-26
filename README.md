[Back to index](../README.md)
# Path of Salvation

*A pixel-style, real-time dungeon crawler for mobile platforms. Your ultimate goal is to collect all three legendary gems, each earned by defeating a unique final boss. After collecting a gem, the game resets for a new challenge.*

---

# 📘 General Overview

## Gameplay Loop

1. Browse and buy gear in the market/inventory
2. Choose a mission (contract) to undertake
3. Enter the dungeon and fight through enemies
4. Get to the final boss and defeat them to earn a legendary gem
5. Return to the main menu and start a new run

## Menu Overview

- **Home:** Main hub and starting point
- **Contracts:** Choose missions (up to 3 per run)
- **Market:** Shop for new gear and equipment
- **Loadout:** Equip weapons and armor
- **Skills:** Assign and upgrade abilities

## Biomes

- The game features multiple biomes, each with unique visuals and enemy types [here](./scripts/data/MyEnums.cs):

``` c#
public enum Biomes {
    Woodland,
    Swamp,
    Desert,
    Mountain,
    Cave,
    IceCave,
    Volcanic,
    Ruins,
    Crypt,
    Abyss,
}
```
*Current biome chooses what next available biome is.* 

## Mob Families

- Enemy groups ("mob families") are categorized and vary by biome and contract [here](./scripts/data/MyEnums.cs):

``` c#
public enum MobFamilies {
    Wild,
    Elemental,
    Goblin,
    Bandit,
    Undead,
    Cultist,
    Abyssal
}
```
*Each family has unique abilities and combat behaviors.*

## Combat Overhaul

The combat system has been overhauled to add more depth and tactical options:

**[Damage Types:](./scripts/data/MyEnums.cs)**
``` c#
public enum DamageType {
    Physical,
    Slash, 
    Pierce, 
    Crush, 
    Heat, 
    Cold, 
    Acid
}
```
**[Status Types:](./scripts/data/MyEnums.cs)**
``` c#
public enum StatusType {
    Hit, 
    Stun, 
    Slow, 
    Burn, 
    Freeze
}
```
- **Status Tracking:** Effects are tracked and updated each frame, with visual feedback
