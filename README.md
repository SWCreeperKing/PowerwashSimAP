## How to install
(tutorial totally not copy and pasted from Tunic AP mod)

- Download [BepInEx 6.0.0-pre1](https://github.com/BepInEx/BepInEx/releases/download/v6.0.0-pre.1/BepInEx_UnityIL2CPP_x64_6.0.0-pre.1.zip).
- Extract the BepInEx zip folder you downloaded from the previous step into your game's install directory (For example: C:\Program Files (x86)\Steam\steamapps\common\PowerWash Simulator)
- Launch the game and close it. This will finalize the BepInEx installation.
- Download and extract the `SW_CreeperKing.ArchipelagoMod.Zip` from the release page.
    - Copy the `SW_CreeperKing.ArchipelagoMod` folder from the release zip into `BepInEx/plugins` under your game's install directory.
    - If there is no `BepInEx/plugins` folder:
      - make sure you have at least [.net8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) 
      - check to see if your antivirus or something else that maybe interfering with BepInEx
- Launch the game again and you should see the connection input on the top left of the title screen!
- To uninstall the mod, either remove/delete the `SW_CreeperKing.ArchipelagoMod` folder from the plugins folder or rename the winhttp.dll file located in the game's root directory (this will disable all installed mods from running).

---

## Randomizer Info

- Can only play in Free Play
- Levels are locked and need an unlock item to play them
  - unlock item is random
- Starting level is random unless you use the yaml setting
- Levels will hide themselves once all their checks are done
- Can choose which levels to add to the level pool
- Objectsanity
  - Cleaning individual parts gives checks
- Navigation (left and right) buttons disabled

- Current Goal Condition is McGuffin Hunt

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
