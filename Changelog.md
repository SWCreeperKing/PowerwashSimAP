# v.0.5.6 

- [Client] Fixed a bug with the failsafe not failsafing
- [ApWorld] Added Universal Tracker support
- [ApWorld] Fixed a check preventing the below 50% from working properly

> [!WARNING]
> FAILSAFE MIGHT BE TRIGGER HAPPY, HOST DISCRETION ADVISED

---
# v.0.5.5

- [ApWorld] Fixed a bug that included extra filler causing failure in some cases

---
# v.0.5.4

- [ApWorld] Finally replaced the band-aid fix on mcguffin hunt with a real fix
- [ApWorld] Forced a min amount of levels required to goal for level hunt to 50% of enabled levels
- [Client] Fixed a bug where completing a level would send locations from levels with similar starting names
- [Client] Improved information displayed when playing with level hunt

---
# v.0.5.3

- [ApWorld] Fixed a bug where `levels_to_goal` wouldn't count "Random" as an exception to a yaml error test

---
# v.0.5.2

- [ApWorld] Fixed a logic bug from a bug caused by fixing logic bugs (forgot to check goal type)

---
# v.0.5.1

- [ApWorld] Fixed logic bugs
- [Client] Totally didn't forget to rebuild before releasing v.0.5.0

---
# v.0.5.0

- [ApWorld] Added support for paid dlc
- [Client] Added support for paid dlc

---
# v.0.4.1

- [ApWorld] Fixed generation problem with multiple powerwash simulators
- [Client] Fixed a bug when connecting with more than 1 goal level

---
# v.0.4

- [ApWorld] Added `Level hunt` goal type
- [ApWorld] Changed how local fill works
- [ApWorld] Fixed a few locations missing parts for objectsanity
- [Yaml] Added `local_fill` yaml setting
- [Yaml] Added `goal_type` yaml setting
- [Yaml] Added `levels_to_goal` yaml setting
- [Yaml] Added `amount_of_levels_to_goal` yaml setting
- [Yaml] Added `allow_below_localfill_minimums` host yaml setting
- [Client] Support for `Level hunt`
- [Client] Minor optimization to my bad code that slowed down level load times

---
# v.0.3.2

- [ApWorld] Added yaml error for when no sanity exists
- [Client] Optimized receiving items

---
# v.0.3.1

- [ApWorld] Hopefully balanced local fill a bit better
  - Percentsanity: 50%
  - Objectsanity: 60%
  - Percentsanity and Objectsanity: 97%

---
# v.0.3

- [ApWorld] Added local fill (98%)
- [ApWorld] Added all free dlc except seasonal specials
- [Yaml] Locations are now separated
- [Client] Made more buttons disappear based on what you can do
- [Client] Now hides instead of disables the navigation buttons
- [Client] Possible optimization witch Objectsanity code
- [Client] Fixed Objectsanity sending checks only when level progress changed

---
# v.0.2.2

- [ApWorld] Fixed some Land Vehicle Objectsanity Parts not being mapped correctly 
- [Client] Improved stability of check sending

---
# v.0.2.1

- [Client] Optimized (a little bit) sending a lot of checks at once 
- [Client] Fixed a few bugs around sending checks

---
# v.0.2

- [ApWorld] Changed how needed mcguffins is calculated
- [ApWorld] Added Objectsanity
- [ApWorld] Percentsanity is now configurable
- [Yaml] Added `sanities` option
- [Yaml] Added `percentsanity` setting
- [Yaml] Added `allow_percentsanity_below_7` host yaml option
- [Yaml] Added `allow_objectsanity` host yaml option
- [Client] Support for Precentsanity settings
- [Client] Support for Objectsanity
- [Client] Disabled navigation buttons

---
# v.0.1.4

- [Client] Completed levels will hide themselves in the level selection

---
# v.0.1.3

- [ApWorld] Fixed `start with van` causing an error if turned on (2am programming moment)
- [ApWorld] If you have `start with van` but don't have "Van" in locations, "Van" will be added

---
# v.0.1.2

- [ApWorld] Hopefully fixed a major logic bug

---
# v.0.1.1

- [ApWorld] Fixed `start with van` yaml setting braking generation when not having "Van" in `locations`
- [Client] Made the mcguffins have and total needed more clear that its mcguffin count and not level completion count 

---
# v.0.1

- Initial Release