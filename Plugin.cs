using BepInEx;
using BepInEx.Logging;
using HarmonyLib;

namespace CustomLoadingIcon;

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"Plugin {PluginInfo.GUID} is loaded!");

        Harmony harmony = new Harmony(PluginInfo.GUID);

        harmony.PatchAll();
    }

    private void Start()
    {
        CommandHandler.InitializeCommands();
    }
}
