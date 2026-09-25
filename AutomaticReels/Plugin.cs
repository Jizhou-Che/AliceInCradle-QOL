using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;

namespace AutomaticReels;

public enum ReelTriggerMode
{
    Always,
    HoldKey,
    Never,
}

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    internal static ConfigEntry<ReelTriggerMode> TriggerMode;
    internal static ConfigEntry<KeyCode> TriggerKey;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} successfully loaded.");

        TriggerMode = Config.Bind("General", "TriggerMode", ReelTriggerMode.HoldKey,
            "Always optimize the reel selection, optimize only while the trigger key is held, or never optimize.");
        TriggerKey = Config.Bind("General", "TriggerKey", KeyCode.RightShift,
            "The key held to optimize the reel selection when TriggerMode is HoldKey.");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }

    internal static bool ShouldOptimize()
    {
        return TriggerMode.Value switch
        {
            ReelTriggerMode.Always => true,
            ReelTriggerMode.HoldKey => Input.GetKey(TriggerKey.Value),
            _ => false,
        };
    }
}
