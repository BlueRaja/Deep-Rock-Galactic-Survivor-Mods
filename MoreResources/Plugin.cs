using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using DRS.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using BepInEx.Configuration;

namespace MoreResources
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private static ConfigEntry<float> _multiplierGold;
        private static ConfigEntry<float> _multiplierNitra;
        private static ConfigEntry<float> _multiplierRedSugar;
        private static ConfigEntry<float> _multiplierCredits;
        private static ConfigEntry<float> _multiplierRankXP;
        private static ConfigEntry<float> _multiplierRunXP;
        private static ConfigEntry<float> _multiplierBismor;
        private static ConfigEntry<float> _multiplierCroppa;
        private static ConfigEntry<float> _multiplierEnorPearls;
        private static ConfigEntry<float> _multiplierJadiz;
        private static ConfigEntry<float> _multiplierMagnite;
        private static ConfigEntry<float> _multiplierUmanite;
        private static ConfigEntry<float> _multiplierMorkite;

        private static readonly Random Random = new();
        private new static ManualLogSource Log;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            _multiplierGold = Config.Bind("In-run resources", "Gold", 1.0f, "Multiplier for gold. Used to buy things during the run.");
            _multiplierNitra = Config.Bind("In-run resources", "Nitra", 1.0f, "Multiplier for nitra. Used to buy things during the run.");
            _multiplierRedSugar = Config.Bind("In-run resources", "Red sugar", 1.0f, "Multiplier for red sugar. Used to heal during the run.");
            _multiplierRunXP = Config.Bind("In-run resources", "Run XP", 1.0f, "Multiplier for XP gained during a run. Gives access to meta-upgrades that only last for that run.");
            _multiplierCredits = Config.Bind("Post-run resources", "Credits", 2.0f, "Multiplier for credits. Used to upgrade things between runs. Note that this is based on the resources earned during a run, so if you're increasing those, you will already have an increase to credits.");
            _multiplierRankXP = Config.Bind("Post-run resources", "Rank XP", 2.0f, "Multiplier for rank XP. Levels up your character between runs. Note that this is based on the resources earned during a run, so if you're increasing those, you will already have an increase to rank XP.");
            _multiplierBismor = Config.Bind("Post-run resources", "Bismor", 2.0f, "Multiplier for Bismor, the yellow+red mineral. Used to upgrade things between runs.");
            _multiplierCroppa = Config.Bind("Post-run resources", "Croppa", 2.0f, "Multiplier for Croppa, the purple+teal mineral. Used to upgrade things between runs.");
            _multiplierEnorPearls = Config.Bind("Post-run resources", "Enor pearls", 2.0f, "Multiplier for Enor pearls, the white mineral. Used to upgrade things between runs.");
            _multiplierJadiz = Config.Bind("Post-run resources", "Jadiz", 2.0f, "Multiplier for Jadiz, the green mineral. Used to upgrade things between runs.");
            _multiplierMagnite = Config.Bind("Post-run resources", "Magnite", 2.0f, "Multiplier for Magnite, the red+gray mineral. Used to upgrade things between runs.");
            _multiplierUmanite = Config.Bind("Post-run resources", "Umanite", 2.0f, "Multiplier for Umanite, the green+yellow mineral. Used to upgrade things between runs.");
            _multiplierMorkite = Config.Bind("Post-run resources", "Morkite", 2.0f, "Multiplier for Morkite, the blue mineral that can only be found in masteries and anomaly dives. Used to upgrade things between runs.");
            
            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(PickupSpawner), "SpawnMaterial")]
        [HarmonyPrefix]
        private static void SpawnMaterialPrefix(PickupSpawner __instance, ref ECurrency material, ref int count, ref Vector3 position, ref float forceMod)
        {
            var multiplier = material switch
            {
                ECurrency.GOLD => _multiplierGold.Value,
                ECurrency.NITRA => _multiplierNitra.Value,
                ECurrency.RED_SUGAR => _multiplierRedSugar.Value,
                ECurrency.BISMOR => _multiplierBismor.Value,
                ECurrency.CROPPA => _multiplierCroppa.Value,
                ECurrency.ENOR_PEARL => _multiplierEnorPearls.Value,
                ECurrency.JADIZ => _multiplierJadiz.Value,
                ECurrency.MAGNITE => _multiplierMagnite.Value,
                ECurrency.UMANITE => _multiplierUmanite.Value,
                ECurrency.MORKITE => _multiplierMorkite.Value,
                _ => 1f
            };

            count = RoundRandomly(count * multiplier);
        }

        [HarmonyPatch(typeof(PickupSpawner), "SpawnXP")]
        [HarmonyPrefix]
        private static void SpawnXpPrefix(PickupSpawner __instance, ref int amount)
        {
            amount = RoundRandomly(amount * _multiplierRunXP.Value);
        }

        [HarmonyPatch(typeof(Economy), "CalculateCreditsEarned")]
        [HarmonyPostfix]
        private static void CalculateCreditsEarnedPostfix(ref int __result)
        {
            __result = RoundRandomly(__result * _multiplierCredits.Value);
        }

        [HarmonyPatch(typeof(Economy), "CalculateRankXpEarned")]
        [HarmonyPostfix]
        private static void CalculateRankXpEarnedPostfix(ref int __result)
        {
            __result = RoundRandomly(__result * _multiplierRankXP.Value);
        }

        // Round up/down in a way that gives roundMe as an average value
        // For instance, if roundMe = 1.25, then it will round down to "1" 75% of the time, and round up to "2" 25% of the time
        private static int RoundRandomly(float roundMe)
        {
            int floor = (int)Math.Floor(roundMe);
            float fractionalPart = roundMe - floor;
            return fractionalPart > 0 && fractionalPart > Random.NextDouble() ? floor + 1 : floor;
        }
    }
}
