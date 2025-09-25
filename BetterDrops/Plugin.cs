using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using BetterDrops;
using DRS.UI;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace MoreResources
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BasePlugin
    {
        private static ConfigEntry<int> _luckBuff;

        private new static ManualLogSource Log;

        public override void Load()
        {
            Harmony.CreateAndPatchAll(typeof(Plugin));
            _luckBuff = Config.Bind("Cheats", "Luck", 25, new ConfigDescription("How much to increase luck by. Higher values give better upgrade choices during the runs.", new AcceptableValueRange<int>(0, 100)));

            Log = base.Log;
            Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        }

        [HarmonyPatch(typeof(RarityWeight), "PullRarity")]
        [HarmonyPrefix]
        private static void PullRarityPrefix(RarityWeight __instance, ref float luck)
        {
            luck += _luckBuff.Value;
        }
    }
}
