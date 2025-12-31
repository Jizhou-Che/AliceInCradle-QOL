using System.Reflection;
using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace AutomaticReels;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} successfully loaded.");

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
    }
}