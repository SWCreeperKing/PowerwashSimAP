using System.Collections.Generic;
using System.Linq;

namespace PowerwashSimAP.Patches;

public static class Locations
{
    public static string[][] RawLocationData =
    [
        // land
        ["HOME_VAN", "Van", "Game_Vehicle_PWVan"],
        ["SUBURBIA_VINTAGECAR", "Vintage Car", "Game_Suburbia_VintageCar"],
        ["RESIDENTIALSMALL_RACINGCAR", "Grandpa Miller's Car", "Game_Residential Small_80sRacingCar"],
        ["FIRESTATION_FIRETRUCK", "Fire Truck", "Game_FireStation_FireTruck"],
        ["HOME_DIRTBIKE", "Dirt Bike", "Game_Vehicle_DirtBike"],
        ["HOME_GOLFCART", "Golf Cart", "Game_Vehicle_GolfCart"],
        ["HOME_MOTORBIKESIDECAR", "Motorbike and Sidecar", "Game_Vehicle_MotorbikeSidecar"],
        ["HOME_SUV", "SUV", "Game_Vehicle_SUV"],
        ["HOME_PENNYFARTHING", "Penny Farthing", "Game_Vehicle_PennyFarthing"],
        ["SUBURBIA_CAMPERVAN", "Recreation Vehicle", "Game_Suburbia_CamperVan"],
        ["HOME_DRILL", "Drill", "Game_Vehicle_Drill"],
        ["HANGAR_MONSTERTRUCK", "Monster Truck", "Game_Hangar_MonsterTruck"],

        // water
        ["MANSION_FROLICBOAT", "Frolic Boat", "Game_Mansion_FrolicBoat"],
        ["MARINA_FISHINGBOAT", "Fishing Boat", "Game_Marina_FishingBoat"],

        // air
        ["FIRESTATION_FIREHELICOPTER", "Fire Helicopter", "Game_FireStation_FireHelicopter"],
        ["AIRPORT_PRIVATEJET", "Private Jet", "Game_Airport_PrivateJet"],
        ["HANGAR_STUNTPLANE", "Stunt Plane", "Game_Hangar_StuntPlane"],
        ["DESERT_UFO", "Recreational Vehicle (Again)", "Game_Desert_UFO"],

        // places
        ["RESIDENTIALSMALL_BACKYARD", "Back Garden", "Game_ResidentialSmall_Backyard"],
        ["RESIDENTIALSMALL_BUNGALOW", "Bungalow", "Game_Residential Small_Bungalow"],
        ["RECREATIONGROUND_PLAYGROUND", "Playground", "Game_RecreationGround_Playground"],
        ["SUBURBIA_DETACHEDHOUSE", "Detached House", "Game_Suburbia_DetachedHouse"],
        ["NATIONALPARK_SHOEHOUSE", "Shoe House", "Game_NationalPark_ShoeHouse"],
        ["FIRESTATION_FIRESTATION", "Fire Station", "Game_FireStation_FireStation"],
        ["RECREATIONGROUND_SKATEPARK", "Skatepark", "Game_RecreationGround_SkatePark"],
        ["NATIONALPARK_STORYBOOKHOUSE", "Forest Cottage", "Game_NationalPark_StoryBookCottage"],
        ["MANSION_FRONT", "Mayor's Mansion", "Game_Mansion_Front"],
        ["FAIRGROUND_MERRYGOROUND", "Carousel", "Game_FairGround_MerryGoRound"],
        ["NATIONALPARK_TREEHOUSE", "Tree House", "Game_NationalPark_TreeHouse"],
        ["COUNTRYSIDE_TEMPLE", "Temple", "Game_CountrySide_Temple"],
        ["SUBWAY_SUBWAYWASHROOM", "Washroom", "Game_Subway_SubwayWashroom"],
        ["FAIRGROUND_HELTERSKELTER", "Helter Skelter", "Game_FairGround_HelterSkelter"],
        ["FAIRGROUND_BIGWHEEL", "Ferris Wheel", "Game_FairGround_BigWheel"],
        ["SUBWAY_SUBWAYPLATFORM", "Subway Platform", "Game_Subway_SubwayPlatform"],
        ["FAIRGROUND_FORTUNETELLERCARAVAN", "Fortune Teller's Wagon", "Game_FairGround_FortuneTellerCaravan"],
        ["DESERT_ANCIENTSTATUE", "Ancient Statue", "Game_Desert_AncientStatue"],
        ["DESERT_ANCIENTHAND", "Ancient Monument", "Game_Desert_AncientHand"],
        ["SEATEMPLE", "Lost City Palace", "Game_SeaTemple"],
    ];

    public static Dictionary<string, string> LevelUnlockDictionary =
        RawLocationData.ToDictionary(arr => $"{arr[1]} Unlock", arr => arr[0]);

    public static Dictionary<string, string> SceneNameToLocationName =
        RawLocationData.ToDictionary(arr => arr[2], arr => arr[1]);
}