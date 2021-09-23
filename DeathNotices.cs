// #define DEBUG

using Newtonsoft.Json;
using Oxide.Core;
using Rust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using System.ComponentModel;
using System.Globalization;
using Oxide.Core.Libraries;
using Oxide.Core.Libraries.Covalence;
using Oxide.Core.Plugins;

namespace Oxide.Plugins
{
    [Info("Death Notices", "DarkAz", "1.0.0")]
    [Description("Announce kills within game chat.")]
    class DeathNotices : RustPlugin
    {


        #region Configuration

        private static Configuration _config;

        private class Configuration
        {
            [JsonProperty(PropertyName = "PostToDiscord")]
            public bool PostToDiscord = false;

            [JsonProperty(PropertyName = "DiscordWebhookURL")]
            public string DiscordWebhookURL = "";

            [JsonProperty(PropertyName = "HighlightHeadshots")]
            public bool HighlightHeadshots = true;

            [JsonProperty(PropertyName = "Colors")]
            public Dictionary<string, string> Colors = new Dictionary<string, string>()
            {
                ["default"] = "#AAAAAA",
                ["killer"] = "#C4FF00",
                ["victim"] = "#C4FF00",
                ["victimself"] = "#FF0000",
                ["weapon"] = "#C4FF00",
                ["attachments"] = "#C4FF00",
                ["distance"] = "#C4FF00",
                ["owner"] = "#C4FF00",
                ["headshot"] = "#FF0000",
            };

            [JsonProperty(PropertyName = "Entities")]
            public Dictionary<string, string> Entities = new Dictionary<string, string>()
            {
                ["banditguard"] = "a Bandit Guard",
                ["scientist"] = "a Scientist",
                ["heavyscientist"] = "a Heavy Scientist",
                ["tunneldweller"] = "a Tunnel Dweller",
                ["murderer"] = "a Murderer",
                ["scarecrow"] = "a Scarecrow",
                ["bear"] = "a Bear",
                ["boar"] = "a Boar",
                ["chicken"] = "a Chicken",
                ["horse"] = "a Horse",
                ["shark"] = "a Shark",
                ["stag"] = "a Stag",
                ["wolf"] = "a Wolf",
                ["zombie"] = "a Zombie",
                ["beartrap"] = "a Bear Trap",
                ["floorspikes"] = "Floor Spikes",
                ["flameturret"] = "a Flame Turret",
                ["landmine"] = "a Land Mine",
                ["autoturret"] = "an Auto Turret",
                ["guntrap"] = "a Gun Trap",
                ["sam_site_turret_deployed"] = "a SAM Site",
                ["napalm"] = "Napalm",
                ["barricadewood"] = "a Wooden Barricade",
                ["barricadewire"] = "a Wire Barricade",
                ["codelock"] = "a Code Lock",
                ["patrolhelicopter"] = "the Patrol Helicopter",
                ["bradleyapc"] = "the Bradley APC",
                ["highexternalwallwood"] = "a High External Wooden Wall",
                ["highexternalgatewood"] = "a High External Wooden Gate",
                ["highexternalwallstone"] = "a High External Stone Wall",
                ["highexternalgatestone"] = "a High External Stone Gate",
                ["highexternalwallice"] = "a High External Ice Wall",
                ["campfire"] = "a Campfire",
                ["skullfirepit"] = "a Skill Firepit",
            };

            [JsonProperty(PropertyName = "WeaponEntities")]
            public Dictionary<string, string> WeaponEntities = new Dictionary<string, string>()
            {
                ["ak47u.entity"] = "Assault Rifle",
                ["axe_salvaged.entity"] = "Salvaged Axe",
                ["bolt_rifle.entity"] = "Bolt Action Rifle",
                ["bone_club.entity"] = "Bone Club",
                ["bow_hunting.entity"] = "Hunting Bow",
                ["butcherknife.entity"] = "Butcher Knife",
                ["cake.entity"] = "Cake",
                ["candy_cane.entity"] = "Candy Cane",
                ["chainsaw.entity"] = "Chainsaw",
                ["compound_bow.entity"] = "Compound Bow",
                ["crossbow.entity"] = "Crossbow",
                ["double_shotgun.entity"] = "Double Barrel Shotgun",
                ["explosive.satchel.deployed"] = "Explosive Satchel",
                ["explosive.timed.deployed"] = "Timed Explosive Charge",
                ["explosive.satchel.entity"] = "Explosive Satchel",
                ["flamethrower_fireball"] = "Flamethrower",
                ["flamethrower.entity"] = "Flamethrower",
                ["flashlight.entity"] = "Flashlight",
                ["grenade.beancan.deployed"] = "Bean Can Grenade",
                ["grenade.beancan.entity"] = "Bean Can Grenade",
                ["grenade.f1.deployed"] = "F1 Grenade",
                ["grenade.f1.entity"] = "F1 Grenade",
                ["hacksaw.weapon"] = "",
                ["hammer_salvaged.entity"] = "Salvaged Hammer",
                ["hammer.entity"] = "Hammer",
                ["hatchet.entity"] = "Hatchet",
                ["icepick_salvaged.entity"] = "Salvaged Icepick",
                ["jackhammer.entity"] = "Jackhammer",
                ["knife_bone.entity"] = "Bone Knife",
                ["knife.combat.entity"] = "Combat Knife",
                ["l96.entity"] = "L96 Rifle",
                ["longsword.entity"] = "Longsword",
                ["lr300.entity"] = "LR-300 Assault Rifle",
                ["m249.entity"] = "M249",
                ["m39.entity"] = "M39 Rifle",
                ["m92.entity"] = "M92 Pistol",
                ["mace.entity"] = "Mace",
                ["machete.weapon"] = "machete",
                ["mgl.entity"] = "",
                ["militaryflamethrower.entity"] = "Flame Thrower",
                ["mp5.entity"] = "MP5A4",
                ["nailgun.entity"] = "Nailgun",
                ["paddle.entity"] = "Paddle",
                ["pickaxe.entity"] = "Pickaxe",
                ["pistol_eoka.entity"] = "Eoka Pistol",
                ["pistol_revolver.entity"] = "Revolver",
                ["pistol_semiauto.entity"] = "Semi-Automatic Pistol",
                ["pitchfork.entity"] = "Pitchfork",
                ["python.entity"] = "Python Revolver",
                ["rock.entity"] = "Rock",
                ["rocket_basic"] = "Rocket",
                ["rocket_fire"] = "Incendiary Rocket",
                ["rocket_hv"] = "High Velocity Rocket",
                ["rocket_launcher.entity"] = "Rocket Launcher",
                ["rocket_sam"] = "SAM Site",
                ["salvaged_cleaver.entity"] = "Salvaged Cleaver",
                ["salvaged_sword.entity"] = "Salvaged Sword",
                ["semi_auto_rifle.entity"] = "Semi-Automatic Rifle",
                ["shotgun_pump.entity"] = "Pump Action Shotgun",
                ["shotgun_waterpipe.entity"] = "Waterpipe Shotgun",
                ["sickle.entity"] = "Sickle",
                ["smg.entity"] = "Custom SMG",
                ["snowballgun.entity"] = "Snowball Gun",
                ["spas12.entity"] = "SPAS-12 Shotgun",
                ["spear_stone.entity"] = "Stone Spear",
                ["spear_wooden.entity"] = "Wooden Spear",
                ["stone_pickaxe.entity"] = "Stone Pickaxe",
                ["stonehatchet.entity"] = "Stone Hatchet",
                ["thompson.entity"] = "Thompson",
                ["toolgun.entity"] = "Toolgun",
                ["torch.entity"] = "Torch",
                ["waterball"] = "Water",
                ["waterbucket.entity"] = "Water Bucket",
            };            

            [JsonProperty(PropertyName = "Verbs")]
            public Dictionary<string, string> Verbs = new Dictionary<string, string>()
            {
                ["default"] = "fought off",
                ["playerdefault"] = "killed",
                ["bite"] = "bit",
                ["blunt"] = "bashed",
                ["explosion"] = "blew up",
                ["shot"] = "shot",
                ["shotarrow"] = "shot",
                ["slash"] = "slashed",
                ["stab"] = "stabbed",
                ["nailgun"] = "nailed",
                ["flamethrower"] = "roasted",
                ["chainsaw"] = "cut up",
                ["hatchet"] = "bashed",
                ["pickaxe"] = "bashed",
                ["jackhammer"] = "destroyed",
                ["grenade"] = "blew up",
                ["beancangrenade"] = "blew up",
                ["satchel"] = "blew up",
                ["snowball"] = "snowballed",
                ["landmine"] = "destroyed",
                ["funwater"] = "saturated",
                ["antivehicle"] = "took out",
                ["vehicle"] = "ran over",
            };

            [JsonProperty(PropertyName = "DiscordIcons")]
            public Dictionary<string, string> DiscordIcons = new Dictionary<string, string>()
            {
                ["default"] = "skull_crossbones",
                ["playerdefault"] = "skull_crossbones",
                ["bite"] = "vampire",
                ["blunt"] = "fist",
                ["explosion"] = "boom",
                ["shot"] = "gun",
                ["shotarrow"] = "bow_and_arrow",
                ["slash"] = "knife",
                ["stab"] = "dagger",
                ["nailgun"] = "gun",
                ["flamethrower"] = "fire",
                ["chainsaw"] = "carpentry_saw",
                ["hatchet"] = "axe",
                ["pickaxe"] = "hammer_pick",
                ["jackhammer"] = "hammer",
                ["grenade"] = "boom",
                ["snowball"] = "snowman",
                ["landmine"] = "boom",
                ["funwater"] = "sweat_drops",
                ["antivehicle"] = "boom",
                ["vehicle"] = "blue_car",
                ["beancangrenade"] = "canned_food",
                ["satchel"] = "school_satchel",
                ["bear"] = "bear",
                ["boar"] = "boar",
                ["chicken"] = "chicken",
                ["shark"] = "shark",
                ["stag"] = "deer",
                ["wolf"] = "wolf",
                ["cold"] = "cold_face",
                ["fall"] = "parachute",
                ["bleeding"] = "drop_of_blood",
                ["heat"] = "hot_face",
                ["drowned"] = "ocean",
                ["radiation"] = "radioactive",
            };
        }

        protected override void LoadConfig()
        {
            base.LoadConfig();
            try
            {
                _config = Config.ReadObject<Configuration>();
                if (_config == null) throw new Exception();
                SaveConfig();
            }
            catch
            {
                PrintError("Your configuration file contains an error. Using default configuration values.");
                LoadDefaultConfig();
            }
        }

        protected override void SaveConfig() => Config.WriteObject(_config);

        protected override void LoadDefaultConfig() => _config = new Configuration();

        #endregion

        #region Private Data

        private readonly Regex _colorTagRegex = new Regex(@"<color=.{0,7}>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly Regex _sizeTagRegex = new Regex(@"<size=\d*>", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private readonly List<string> _richTextLiterals = new List<string>
        {
            "</color>", "</size>", "<b>", "</b>", "<i>", "</i>"
        };
        private readonly List<string> _environmentalDamage = new List<string>
        {
            "Cold"
        };

        private Dictionary<string, string> _entityShortPrefabs = new Dictionary<string, string>()
        {
            // players
            { "player", "player" },
            { "npcplayertest", "player" },
            // bandit
            { "bandit_guard", "banditguard" },
            // scientists
            { "scientist_astar_full_any", "scientist" },
            { "scientist_full_any", "scientist" },
            { "scientist_full_lr300", "scientist" },
            { "scientist_full_mp5", "scientist" },
            { "scientist_full_pistol", "scientist" },
            { "scientist_full_shotgun", "scientist" },
            { "scientist_junkpile_pistol", "scientist" },
            { "scientist_turret_any", "scientist" },
            { "scientist_turret_lr300", "scientist" },
            { "scientist", "scientist" },
            { "scientist_gunner", "scientist" },
            { "scientistnpc_cargo", "scientist" },
            { "scientistnpc_excavator", "scientist" },
            { "scientistnpc_oilrig", "scientist" },
            { "scientistnpc_patrol", "scientist" },
            { "scientistnpc_roam", "scientist" },
            { "scientistjunkpile", "scientist" },
            { "scientistpeacekeeper", "scientist" },
            { "scientiststationary", "scientist" },
            { "scientistnpc", "scientist" },
            { "humannpc", "scientist" },
            { "heavyscientist", "heavyscientist" },
            { "heavyscientistad", "heavyscientist" },
            { "tunneldweller", "tunneldweller" },
            // halloween
            { "murderer", "murderer" },
            { "scarecrow", "scarecrow" },
            // animals
            { "bear", "bear" },
            { "boar", "boar" },
            { "chicken", "chicken" },
            { "horse", "horse" },
            { "testridablehorse", "horse" },
            { "simpleshark", "shark" },
            { "stag", "stag" },
            { "wolf", "wolf" },
            // zombie
            { "zombie", "zombie" },
            // traps, etc
            { "beartrap", "beartrap" },
            { "spikes.floor", "floorspikes" },
            { "flameturret_fireball", "flameturret" },
            { "landmine", "landmine" },
            { "autoturret_deployed", "autoturret" },
            { "guntrap.deployed", "guntrap" },
            { "flameturret.deployed", "flameturret" },
            { "napalm", "napalm" },
            { "barricade.wood", "barricadewood" },
            { "barricade.woodwire", "barricadewire" },
            { "lock.code", "codelock" },
            // helicopter
            { "patrolhelicopter", "patrolhelicopter" },
            // bradley
            { "bradleyapc", "bradleyapc"},
            // misc
            { "wall.external.high", "highexternalwallwood" },
            { "wall.external.high.stone", "highexternalwallstone" },
            { "wall.external.high.ice", "highexternalwallice" },
            { "gates.external.high.wood", "highexternalgatewood" },
            { "gates.external.high.stone", "highexternalgatestone" },
            { "campfire", "campfire" },
            { "skull_fire_pit", "skullfirepit" },
            //exceptions (require further handling)
            { "flamethrower_fireball", "other" },
        };

        #endregion

        #region Helpers

#if DEBUG
        private static void LogDebug(string text)
        {
            if (BasePlayer.activePlayerList.Count >= 1)
            {
                BasePlayer.activePlayerList[0].ConsoleMessage($"<color=orange>{text}</color>");
            }
        } 
#endif


        private string StripRichText(string text)
        {
            if (string.IsNullOrEmpty(text))
                return string.Empty;

            text = _colorTagRegex.Replace(text, string.Empty);
            text = _sizeTagRegex.Replace(text, string.Empty);

            foreach (var richTextLiteral in _richTextLiterals)
                text = text.Replace(richTextLiteral, string.Empty, StringComparison.InvariantCulture);

            return text;
        }

        private struct DeathData
        {
            public String KillerEntityType { get; set; }
            [JsonIgnore] public BaseEntity KillerEntity { get; set; }
            public String KillerName { get; set; }

            public String VictimEntityType { get; set; }
            [JsonIgnore] public BaseCombatEntity VictimEntity { get; set; }
            public String VictimName { get; set; }

            public DamageType DamageType { get; set; }
            [JsonIgnore] public HitInfo HitInfo { get; set; }

            public String WeaponType { get; set; }

            public String Distance { get; set; }

            public String IconType { get; set; }

            public String Message { get; set; }

            public Dictionary<string, object> ToDictionary() => new Dictionary<string, object>
            {
                ["KillerEntity"] = KillerEntity,
                ["KillerEntityType"] = KillerEntityType,
                ["KillerName"] = KillerName,
                ["VictimEntity"] = VictimEntity,
                ["VictimEntityType"] = VictimEntityType,
                ["VictimName"] = VictimName,
                ["DamageType"] = DamageType,
                ["HitInfo"] = HitInfo,
                ["WeaponType"] = WeaponType,
                ["Distance"] = Distance,
                ["IconType"] = IconType,
                ["Message"] = Message,
            };
        }

        private String GetCombatEntityType(BaseEntity entity)
        {
            if (entity == null)
                return "none";

            String EntityName;
            String ConfigEntityName;

            bool knownEntity = _entityShortPrefabs.TryGetValue(entity.ShortPrefabName, out EntityName);

            if (knownEntity)
            {
                bool knownEntityName = _config.Entities.TryGetValue(EntityName, out ConfigEntityName);

                if (knownEntityName)
                    return ConfigEntityName;

            }

#if DEBUG
        Puts("******* Death Notices : Unknown Entity Start *******");
        Puts($"Entity: {entity}");
        Puts($"Entity PrefabName: {entity?.PrefabName}");
        Puts($"Entity ShortPrefabName: {entity?.ShortPrefabName}");
        Puts("******* Death Notices : Unknown Entity End *******");
#endif

            // if (CombatEntityType.ContainsKey(entity.ShortPrefabName))
            //   return CombatEntityType.Other;
            //     return CombatEntityType[entity.ShortPrefabName];

            // if (CombatEntityType.Contents.ContainsKey(entity.GetType().Name))
            //     return CombatEntityType[entity.GetType().Name];

            // if (entity is BaseOven)
            //     return CombatEntityType.HeatSource;

            // if (entity is SimpleBuildingBlock)
            //     return CombatEntityType.ExternalWall;

            // if (entity is BaseAnimalNPC)
            //     return CombatEntityType.Animal;

            // if (entity is BaseTrap)
            //     return CombatEntityType.Trap;

            // if (entity is Barricade)
            //     return CombatEntityType.Barricade;

            // if (entity is IOEntity)
            //     return CombatEntityType.Trap;

            return EntityName;

        }


        private String GetWeaponType(BaseEntity entity)
        {
            if (entity == null)
                return "none";

            String EntityName;

            bool knownEntity = _config.WeaponEntities.TryGetValue(entity.ShortPrefabName, out EntityName);

            if (knownEntity)
                return EntityName;

#if DEBUG
        Puts("********************* Death Notices : Unknown Weapon Entity Start *******");
        Puts($"Entity: {entity}");
        Puts($"Entity PrefabName: {entity?.PrefabName}");
        Puts($"Entity ShortPrefabName: {entity?.ShortPrefabName}");
        Puts("********************* Death Notices : Unknown Weapon Entity End *******");
#endif
            return "other";
        }

        private DeathData BuildKillMessage(DeathData data)
        {

            string message = $"<color={_config.Colors["default"]}>";

            message += $"<color={_config.Colors["killer"]}>";
            message += FirstLetterToUpper(data.KillerName);
            message += "</color>";

            string verbage = "";

            switch (data.DamageType)
            {
                case DamageType.Bullet:
                    verbage = _config.Verbs["shot"];
                    data.IconType = _config.DiscordIcons["shot"];
                    if (data.WeaponType == "Nailgun")
                    { // fix: because, nailguns
                        verbage = _config.Verbs["nailgun"];
                        data.IconType = _config.DiscordIcons["nailgun"];
                    }
                    break;

                case DamageType.Arrow:
                    verbage = _config.Verbs["shotarrow"];
                    data.IconType = _config.DiscordIcons["shotarrow"];
                    break;

                case DamageType.Slash:
                    verbage = "slashed";
                    data.IconType = _config.DiscordIcons["slash"];
                    if (data.WeaponType == "Flamethrower" || data.KillerEntity?.ShortPrefabName == "flameturret.deployed")
                    { // fix: Flamethrowers do slash damage!
                        verbage = _config.Verbs["flamethrower"];
                        data.IconType = _config.DiscordIcons["flamethrower"];
                    }
                    if (data.WeaponType == "Chainsaw")
                    { // fix: Chainsaws do slash damage!
                        verbage = _config.Verbs["chainsaw"];
                        data.IconType = _config.DiscordIcons["chainsaw"];
                    }
                    if (data.WeaponType == "Hatchet" || data.WeaponType == "Hatchet")
                    { // fix: Hatchets do slash damage!
                        verbage = _config.Verbs["hatchet"];
                        data.IconType = _config.DiscordIcons["hatchet"];
                    }
                    break;

                case DamageType.Stab:
                    verbage = "stabbed";
                    data.IconType = _config.DiscordIcons["stab"];
                    if (data.WeaponType == "Bean Can Grenade")
                    { // fix: bean can grenades and explosive satchels do stab damage!
                        verbage = _config.Verbs["beancangrenade"];
                        data.IconType = _config.DiscordIcons["beancangrenade"];
                    }
                    if (data.WeaponType == "Explosive Satchel")
                    { // fix: bean can grenades and explosive satchels do stab damage!
                        verbage = _config.Verbs["satchel"];
                        data.IconType = _config.DiscordIcons["satchel"];
                    }
                    if (data.WeaponType == "Stone Pickaxe")
                    { // fix: Stone Pickaxe do stab damage!
                        verbage = _config.Verbs["pickaxe"];
                        data.IconType = _config.DiscordIcons["pickaxe"];
                    }
                    if (data.WeaponType == "Jackhammer")
                    { // fix: Jackhammers do stab damage!
                        verbage = _config.Verbs["jackhammer"];
                        data.IconType = _config.DiscordIcons["jackhammer"];
                    }
                    break;

                case DamageType.Bite:
                    verbage = _config.Verbs["bite"];
                        data.IconType = _config.DiscordIcons["bite"];
                    break;

                case DamageType.Blunt:
                    verbage = _config.Verbs["blunt"];
                    data.IconType = _config.DiscordIcons["blunt"];
                    if (data.WeaponType == "F1 Grenade")
                    { // fix: F1 grenades do blunt damage!
                        verbage = _config.Verbs["grenade"];
                        data.IconType = _config.DiscordIcons["grenade"];
                    }
                    if (data.WeaponType == "Snowball Gun")
                    { // fix: Snowball Guns do blunt damage!
                        verbage = _config.Verbs["snowball"];
                        data.IconType = _config.DiscordIcons["snowball"];
                    }
                    if (data.KillerEntity.ShortPrefabName == "landmine")
                    { // fix: Landmines do blunt damage!
                        verbage = _config.Verbs["landmine"];
                        data.IconType = _config.DiscordIcons["landmine"];
                    }
                    break;

                case DamageType.Fun_Water:
                    verbage = _config.Verbs["funwater"];
                    data.IconType = _config.DiscordIcons["funwater"];
                    break;

                case DamageType.AntiVehicle:
                    verbage = _config.Verbs["antivehicle"];
                    data.IconType = _config.DiscordIcons["antivehicle"];
                    break;

                case DamageType.Explosion:
                    verbage = _config.Verbs["explosion"];
                    data.IconType = _config.DiscordIcons["explosion"];
                    break;

                case DamageType.Generic:
                    if (data.KillerEntity.ToPlayer().isMounted)
                    {
                        verbage = _config.Verbs["vehicle"];
                        data.IconType = _config.DiscordIcons["vehicle"];
                        break;
                    }
                    break;
                
                default:
                    if (data.VictimEntityType == "player") {
                        verbage = _config.Verbs["playerdefault"];
                        data.IconType = _config.DiscordIcons["playerdefault"];
                    } else {
                        verbage = _config.Verbs["default"];
                        data.IconType = _config.DiscordIcons["default"];
                    }
                    break;

            }

            message += $" {verbage} ";

            if (data.VictimName == data.KillerName || string.IsNullOrEmpty(data.KillerName))
            {
                message += $"<color={_config.Colors["victimself"]}>themself!</color>";

            }
            else
            {
                message += $"<color={_config.Colors["victim"]}>{data.VictimName}</color>";
            }

            if (data.WeaponType != "other" && data.WeaponType != "none" && data.KillerEntity.ShortPrefabName != "autoturret_deployed" && data.KillerEntity.ShortPrefabName != "sam_site_turret_deployed")
            {
                message += $" with their <color={_config.Colors["weapon"]}>{data.WeaponType}</color>";
            }

            if (data.Distance != "" && verbage == _config.Verbs["shot"] && data.VictimName != data.KillerName)
            {
                message += $" over a distance of <color={_config.Colors["distance"]}>{data.Distance}</color>";
            }

            if(_config.HighlightHeadshots && data.HitInfo.boneArea == HitArea.Head) {
                message += $"<color={_config.Colors["headshot"]}> [Headshot]</color>";
            }

            message += "</color>";

            data.Message = message;

            return data;
        }


        private DeathData BuildOtherMessage(DeathData data)
        {
            string message = $"<color={_config.Colors["default"]}>";
            message += $"<color={_config.Colors["victim"]}>{data.VictimName}</color>";

            bool isValidMessage = false;

            switch (data.DamageType)
            {
                case DamageType.Radiation:
                    message += " died of radiation poisoning";
                    data.IconType = _config.DiscordIcons["radiation"];
                    isValidMessage = true;
                    break;

                case DamageType.Bleeding:
                    message += " bled to death";
                    data.IconType = _config.DiscordIcons["bleeding"];
                    isValidMessage = true;
                    break;

                case DamageType.Drowned:
                    message += " drowned";
                    data.IconType = _config.DiscordIcons["drowned"];
                    isValidMessage = true;
                    break;

            }

            message += "</color>";

            data.Message = isValidMessage ? message : "";

            return data;
        }

        private DeathData DiscordAnimalIcons(DeathData data)
        {
            if (string.IsNullOrEmpty(data.KillerEntityType))
                return data;
            
            string entityKey = _config.Entities.FirstOrDefault(x => x.Value == data.KillerEntityType).Key;

            if (string.IsNullOrEmpty(entityKey))
                return data;

            String iconName;

            bool entityIconExists = _config.DiscordIcons.TryGetValue(entityKey, out iconName);

            if (entityIconExists)
                data.IconType = iconName;

            return data;
        }

        private String BuildEnvironmentalMessage(BaseCombatEntity victimEntity)
        {

            String victimName = StripRichText(victimEntity.ToPlayer().displayName);

            String message = "<color=#aaaaaa>";

            message += $"<color={_config.Colors["victim"]}>{victimName}</color>";

            switch (victimEntity?.lastDamage)
            {

                // case "Collision":
                case DamageType.Cold:
                    message += " froze to death";
                    break;

                case DamageType.Fall:
                    message += " fell to their death";
                    break;

                case DamageType.Bleeding:
                    message += " bled to death";
                    break;

                case DamageType.Heat:
                    message += " overheated";
                    break;

                default:
                    return "";
            }

            message += "</color>";

            return message;
        }

        private void BroadcastMessage(String message)
        {
            if (string.IsNullOrEmpty(message))
                return;

            foreach (var player in BasePlayer.activePlayerList)
            {
                Player.Reply(
                    player,
                    message,
                    ulong.Parse("0")
                );
            }

        }

        private static string GetDistance(BaseEntity killerEntity, BaseCombatEntity victimEntity)
        {
            if (killerEntity == null || victimEntity == null)
                return "";

            var distance = Math.Round(killerEntity.Distance(victimEntity), 2);
            string unit = distance == 1
                ? "meter"
                : "meters";

            if (distance <= 0)
                return "";

            return $"{distance} {unit}";
        }


        public static string FirstLetterToUpper(string s)
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;

            char[] a = s.ToCharArray();
            a[0] = char.ToUpper(a[0]);
            return new string(a);
        }

        #endregion

        #region Hooks

        private void OnEntityDeath(BaseCombatEntity victimEntity, HitInfo hitInfo)
        {
            // Ignore - there is no victim for some reason
            if (victimEntity == null)
                return;

            // if there's no hitInfo we should still handle environmental deaths
            if (hitInfo == null)
            {
                bool validDamage = false;
                string discordIcon = _config.DiscordIcons["default"];

                switch (victimEntity.lastDamage)
                {
                    case DamageType.Cold:
                        validDamage = true;
                        discordIcon = _config.DiscordIcons["cold"];
                        break;
                    case DamageType.Fall:
                        validDamage = true;
                        discordIcon = _config.DiscordIcons["fall"];
                        break;
                    case DamageType.Bleeding:
                        validDamage = true;
                        discordIcon = _config.DiscordIcons["bleeding"];
                        break;
                    case DamageType.Heat:
                        validDamage = true;
                        discordIcon = _config.DiscordIcons["heat"];
                        break;
                }

                if(validDamage) {
                    String environmentalMessage = BuildEnvironmentalMessage(victimEntity);
                    BroadcastMessage(environmentalMessage);
                    // @todo: environmental icons
                    if(_config.PostToDiscord) {
                        SendDiscordMessage(environmentalMessage, discordIcon);
                    }
                }
                return;
            }

            // Try to avoid error when entity was destroyed
            if (victimEntity.gameObject == null)
                return;

            var data = new DeathData
            {
                VictimEntity = victimEntity,
                KillerEntity = victimEntity?.lastAttacker ?? hitInfo?.Initiator,
                VictimEntityType = GetCombatEntityType(victimEntity),
                KillerEntityType = GetCombatEntityType(victimEntity?.lastAttacker),
                DamageType = victimEntity.lastDamage,
                HitInfo = hitInfo,
                WeaponType = GetWeaponType(hitInfo?.WeaponPrefab),
                Distance = GetDistance(victimEntity.lastAttacker ?? hitInfo?.Initiator, victimEntity),
            };

#if DEBUG
            Puts("------- Death Notices Debug Start -------");
            Puts($"VictimEntity: {data.VictimEntity}");
            Puts($"VictimEntity PrefabName: {data.VictimEntity?.PrefabName}");
            Puts($"VictimEntity ShortPrefabName: {data.VictimEntity?.ShortPrefabName}");
            Puts($"VictimEntityType: {data.VictimEntityType}");
            Puts($"VictimName: {data.VictimName}");
            Puts($"KillerEntity: {data.KillerEntity}");
            Puts($"KillerEntity PrefabName: {data.KillerEntity?.PrefabName}");
            Puts($"KillerEntity ShortPrefabName: {data.KillerEntity?.ShortPrefabName}");
            Puts($"KillerEntityType: {data.KillerEntityType}");
            Puts($"KillerName: {data.KillerName}");
            Puts($"DamageType: {data.DamageType}");
            Puts($"HitInfo: {data.HitInfo}");
            Puts($"HitInfo.boneArea: {data.HitInfo?.boneArea}");
            Puts($"HitInfo.WeaponPrefab: {data.HitInfo?.WeaponPrefab}");
            Puts($"HitInfo.WeaponPrefab.ShortPrefabName: {data.HitInfo?.WeaponPrefab?.ShortPrefabName}");
            Puts($"WeaponType: {data.WeaponType}");
            Puts($"Distance: {data.Distance}");
            Puts("------- Death Notices Debug End -------");
#endif

            // Ignore decay
            if (data.DamageType == DamageType.Decay)
                return;

            // Ignore "other" ?
            // if (data.VictimEntityType == "other" || data.KillerEntityType == "other")
            //   return;
            bool hasPlayer = false;

            if (data.VictimEntityType == "player")
            {
                hasPlayer = true;
                data.VictimName = StripRichText(data.VictimEntity.ToPlayer().displayName);
            }
            else if (data.VictimEntityType == _config.Entities["murderer"])
            {
                data.VictimName = StripRichText(data.VictimEntity.ToPlayer().displayName);
            }
            else
            {
                data.VictimName = data.VictimEntityType;
            }

            if (data.KillerEntityType == "player")
            {
                hasPlayer = true;
                data.KillerName = StripRichText(data.KillerEntity.ToPlayer().displayName);
            }
            else if (data.KillerEntityType == _config.Entities["murderer"])
            {
                data.KillerName = StripRichText(data.KillerEntity.ToPlayer().displayName);
            }
            else
            {
                data.KillerName = data.KillerEntityType;
                if (data.HitInfo?.WeaponPrefab?.ShortPrefabName == "rocket_sam")
                {
                    data.KillerName = "A SAM Site";
                }
            }

            // we only want to consider things that involve a player somewhere
            if (hasPlayer == false)
            {
#if DEBUG
                Puts("------- Death Notices Skipped as No Player Was Involved -------");
                Puts($"Killer: {data.KillerName} Victim: {data.VictimName}");
                Puts("---------------------------------------------------------------------------------");
#endif
                return;
            }

            string message = "";

            if (data.KillerEntityType == "none")
            {
                data = BuildOtherMessage(data);
            }
            else
            {
                data = BuildKillMessage(data);
            }

            BroadcastMessage(data.Message);
            if(_config.PostToDiscord) {
                data = DiscordAnimalIcons(data);
                SendDiscordMessage(data.Message, data.IconType);
            }

        }

        #endregion

        #region Discord

        private readonly Dictionary<string, string> _discordHeaders = new Dictionary<string, string>()
        {
            {"Content-Type", "application/json"}
        };

        private class DiscordMessage
        {
            [JsonProperty("content")]
            private string Content { get; set; }

            public DiscordMessage(string content)
            {
                Content = content;
            }

            public StringBuilder ToJson() => new StringBuilder(JsonConvert.SerializeObject(this, Formatting.None,
                new JsonSerializerSettings {NullValueHandling = NullValueHandling.Ignore}));
        }

        private void SendDiscordMessage(string message, string discordIcon)
        {
            if(string.IsNullOrEmpty(_config.DiscordWebhookURL))
                return;

            DiscordMessage dmessage = new DiscordMessage(FormatDiscordMessage(message, discordIcon));

            StringBuilder json = dmessage.ToJson();

            webrequest.Enqueue(_config.DiscordWebhookURL, json.ToString(), SendDiscordMessageCallback, this, RequestMethod.POST, _discordHeaders);
        }

        private void SendDiscordMessageCallback(int code, string message)
        {
            if (code != 204)
            {
                PrintError(message);
            }
        }

        private readonly List<Regex> _regexTags = new List<Regex>
        {
            new Regex("<color=.+?>", RegexOptions.Compiled),
            new Regex("<size=.+?>", RegexOptions.Compiled)
        };

        private readonly List<string> _tags = new List<string>
        {
            "</color>",
            "</size>",
            "<i>",
            "</i>",
            "<b>",
            "</b>"
        };

        private string FormatDiscordMessage(string original, string discordIcon)
        {
            if (string.IsNullOrEmpty(original))
            {
                return string.Empty;
            }

            foreach (string tag in _tags)
            {
                original = original.Replace(tag, "");
            }

            foreach (Regex regexTag in _regexTags)
            {
                original = regexTag.Replace(original, "");
            }


            original = $":{discordIcon}: {original}";

            return original;
        }
        #endregion

    }

}
