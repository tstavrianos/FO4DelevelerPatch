using Mutagen.Bethesda.WPF.Reflection.Attributes;

namespace FO4DelevelerPatch
{
    internal class Settings
    {
        [MaintainOrder] [SettingName("ENCOUNTER ZONE LEVEL")]
        public EncounterZoneLevelSettings EncounterZoneLevelSettings = new();

        [MaintainOrder] [SettingName("ENCOUNTER ZONE FLAGS")]
        public EncounterZoneFlagsSettings EncounterZoneFlagsSettings = new();

        [MaintainOrder] [SettingName("ACTOR / NPC")]
        public ActorNpcSettings ActorNpcSettings = new();
    }

    internal class EncounterZoneLevelSettings
    {
        [MaintainOrder] [SettingName("Enable Level Section")][Tooltip("Enable the fields 'New Maximum Level' and 'New Minimum Level'.")]
        public bool AdjustEncounterZoneLevel = true;
        [MaintainOrder] [SettingName("New Maximum Level (Absolute)")][Tooltip("A value of 0 is equivalent to unlimited. Set to -1 to disable.")]
        public sbyte EncounterZoneMaxLevel = 0;
        [MaintainOrder] [SettingName("New Minimum Level (%)")][Tooltip("This is relative to a zone's original value. Set to 100 to disable.")]
        public byte EncounterZoneMinLevel = 100;
    }

    internal class EncounterZoneFlagsSettings
    {
        [MaintainOrder] [SettingName("Enable Flags Section")][Tooltip("Enable the flags settings below.")]
        public bool AdjustEncounterZoneFlags = false;
        [MaintainOrder] [SettingName("Remove 'Never Resets' Flag")][Tooltip("Enable the mod Encounter Zone Recalculation to reset all zones (except workshops).")]
        public bool RemoveNeverResetsFlag = true;
        [MaintainOrder] [SettingName("Set 'Disable Combat Boundary' Flag")][Tooltip("If checked, allows actors in combat that follow the player through load doors that have different encounter zones.")]
        public bool DisableCombatBoundary = true;
    }

    internal class ActorNpcSettings
    {
        [MaintainOrder] [SettingName("Dynamic NPC Level")][Tooltip("NPCs have their level set relative to player level.")]
        public bool AdjustLevelRelativeToPlayer = true;
        [MaintainOrder] [SettingName("NPCs have their level set relative to player level")]
        public byte LevelRelativeToPlayer = 100;
    }

}