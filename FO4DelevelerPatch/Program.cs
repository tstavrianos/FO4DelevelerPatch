﻿using System;
using System.Threading.Tasks;
using Mutagen.Bethesda;
using Mutagen.Bethesda.Fallout4;
using Mutagen.Bethesda.Synthesis;

namespace FO4DelevelerPatch
{
    public static class Program
    {
        private static Lazy<Settings> _settings;
        private static Settings Settings => _settings.Value;

        public static async Task<int> Main(string[] args)
        {
            return await SynthesisPipeline.Instance
                .AddPatch<IFallout4Mod, IFallout4ModGetter>(RunPatch)
                .SetAutogeneratedSettings("Settings", "settings.json", out _settings)
                .SetTypicalOpen(GameRelease.Fallout4, "Deleveler_Patch.esp")
                .Run(args);
        }

        public static void RunPatch(IPatcherState<IFallout4Mod, IFallout4ModGetter> state)
        {
            if (Settings.ActorNpcSettings.AdjustLevelRelativeToPlayer)
            {
                foreach (var npcContext in state.LoadOrder.PriorityOrder.Npc().WinningContextOverrides())
                {
                    //if(npcContext is null) continue;
                    if (npcContext.Record is null) continue;
                    if (npcContext.Record.IsDeleted) continue;

                    var npc = npcContext.Record;
                    if (npc.EditorID != null &&
                        npc.EditorID.Equals("Player", StringComparison.OrdinalIgnoreCase)) continue;
                    
                    if(npc.Flags.HasFlag(Npc.Flag.IsGhost) || npc.Flags.HasFlag(Npc.Flag.Invulnerable)) continue;

                    if (npc.Level is not PcLevelMult)
                    {
                        var newRecord = npcContext.GetOrAddAsOverride(state.PatchMod);
                        var minLevel = newRecord.CalcMinLevel;
                        if (minLevel <= 0)
                        {
                            minLevel = (short) (((INpcLevelGetter) newRecord.Level).Level);
                        }

                        if (minLevel < 99)
                        {
                            newRecord.CalcMinLevel = minLevel;
                        }

                        newRecord.CalcMaxLevel = 0;
                        newRecord.Level = new PcLevelMult
                        {
                            LevelMult = Settings.ActorNpcSettings.LevelRelativeToPlayer / 100.0f
                        };
                    }
                }
            }

            if (Settings.EncounterZoneLevelSettings.AdjustEncounterZoneLevel || Settings.EncounterZoneFlagsSettings.AdjustEncounterZoneFlags)
            {
                foreach (var encounterZoneContext in
                         state.LoadOrder.PriorityOrder.EncounterZone().WinningContextOverrides())
                {
                    //if(encounterZoneContext is null) continue;
                    if (encounterZoneContext.Record is null) continue;
                    if (encounterZoneContext.Record.IsDeleted) continue;
                    var encounterZone = encounterZoneContext.Record;
                    
                    if(encounterZone.Flags.HasFlag(EncounterZone.Flag.Workshop)) continue;
                    if(encounterZone.EditorID is not null && encounterZone.EditorID.Equals("NoZoneZone", StringComparison.OrdinalIgnoreCase)) continue;

                    var minLevel = (sbyte) (encounterZone.MinLevel * Settings.EncounterZoneLevelSettings.EncounterZoneMinLevel / 100.0f);
                    var newRecord = encounterZone.DeepCopy();
                    var modified = false;
                    if (Settings.EncounterZoneLevelSettings.AdjustEncounterZoneLevel && ((Settings.EncounterZoneLevelSettings.EncounterZoneMinLevel != 100 && encounterZone.MinLevel != minLevel) || (Settings.EncounterZoneLevelSettings.EncounterZoneMaxLevel > -1 && encounterZone.MaxLevel != Settings.EncounterZoneLevelSettings.EncounterZoneMaxLevel)))
                    {
                        if (Settings.EncounterZoneLevelSettings.EncounterZoneMinLevel != 100)
                        {
                            newRecord.MinLevel = minLevel;
                            modified |= newRecord.MinLevel != encounterZone.MinLevel;
                        }

                        if (Settings.EncounterZoneLevelSettings.EncounterZoneMaxLevel > -1)
                        {
                            newRecord.MaxLevel = Settings.EncounterZoneLevelSettings.EncounterZoneMaxLevel;
                            modified |= newRecord.MaxLevel != encounterZone.MaxLevel;
                        }
                    }

                    if (Settings.EncounterZoneFlagsSettings.AdjustEncounterZoneFlags)
                    {
                        if (Settings.EncounterZoneFlagsSettings.RemoveNeverResetsFlag || Settings.EncounterZoneFlagsSettings.DisableCombatBoundary)
                        {
                            if (Settings.EncounterZoneFlagsSettings.RemoveNeverResetsFlag)
                            {
                                newRecord.Flags |= EncounterZone.Flag.NeverResets;
                            }
                            if (Settings.EncounterZoneFlagsSettings.DisableCombatBoundary)
                            {
                                newRecord.Flags |= EncounterZone.Flag.DisableCombatBoundary;
                            }
                            modified |= newRecord.Flags != encounterZone.Flags;
                        }
                    }
                    if(modified)
                        state.PatchMod.EncounterZones.Set(newRecord);
                }
            }
        }
    }
}