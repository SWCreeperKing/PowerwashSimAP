## Randomizer Info BEFORE INSTALLING

### DISCLAIMER: THIS MOD WILL >>NOT<< GIVE YOU THE PAID DLC FOR FREE
### REMINDER: YOU MUST BEAT ALL EXCEPT BONUS JOBS IN VANILLA BEFORE ADDING THEM TO THE RANDOMIZER SETTINGS

> [!Note]
> Other mods might cause problems with levels 

## Features/Information

- Can only play in Free Play
- Levels are locked and need an unlock item to play them
  - unlock item is random
- Starting level is random unless you use the yaml setting
- Levels will hide themselves once all their checks are done
- Can choose which levels to add to the level pool
- All free dlc is added except `Seasonal Specials`
  - be careful: `Seasonal Specials` do NOT count (I can't delete them as hard as I may try)
- Percentsanity
  - Total % cleaned of a level gives checks
- Objectsanity (requires host.yaml setting to enable)
  - Cleaning individual parts gives checks
  - [Here is a list of levels and object count](https://github.com/SWCreeperKing/PowerwashSimAP/blob/master/stats.md)
- Goal Conditions
  - McGuffin Hunt
    - find `A Job Well Done`s to goal
  - Level Hunt
    - complete a certain amount of required levels to goal 
- Includes local fill (not adjustable)

### Multiplayer

Working on levels you don't have a `[level] Unlock` for is considered out of logic 

- Only host with mod
  - confirmed to work
- Host doesn't have mod but a player does
  - confirmed to work
- Multiple people with the mod
  - they are all connected to same slot
    - theoretically works
  - they all have different slots
    - confirmed to work
    - can cause out of logic checks for different players

---

## How to install
(tutorial totally not copy and pasted from Tunic AP mod)

- Download [BepInEx 6.0.0-pre1](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.1/BepInEx_UnityIL2CPP_x64_6.0.0-pre.1.zip).
- Extract the BepInEx zip folder you downloaded from the previous step into your game's install directory (For example: C:\Program Files (x86)\Steam\steamapps\common\PowerWash Simulator)
- Launch the game and close it. This will finalize the BepInEx installation.
- Download and extract the `SW_CreeperKing.ArchipelagoMod.Zip` from the [latest release page](https://github.com/SWCreeperKing/PowerwashSimAP/releases/latest).
    - Copy the `SW_CreeperKing.ArchipelagoMod` folder from the release zip into `BepInEx/plugins` under your game's install directory.
    - If there is no `BepInEx/plugins` folder: 
      - try running as administrator
      - if using linux/wine: type "WINEDLLOVERRIDES="winhttp=n,b" %command%" into launch options
      - check to see if your antivirus or something else that maybe interfering with BepInEx
- Launch the game again and you should see the connection input on the top left of the title screen!
- To uninstall the mod, either remove/delete the `SW_CreeperKing.ArchipelagoMod` folder from the plugins folder or rename the winhttp.dll file located in the game's root directory (this will disable all installed mods from running).

---

# Special Thanks

- Silent for programming support
- BadMagic for telling me about IlRepack

---

# Tools:

- BepInEx (obv)
- Rider
- ILRepacker
- UnityExplorer
