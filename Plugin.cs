using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Logging;
using BepInEx.Configuration;
using UnityEngine;

namespace CustomLoadingIcon;

[BepInPlugin(PluginInfo.GUID, PluginInfo.NAME, PluginInfo.VERSION)]
public class Plugin : BaseUnityPlugin
{
    public static new ManualLogSource Logger;
    private ConfigEntry<string> IconSet;

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"I'm awake uwu");
        Logger.LogInfo($"{BepInEx.Paths.PluginPath}");

        IconSet = Config.Bind<string>("General", "Icont Set Name", "Default", "what icon set to use");
    }

    private void Start()
    {
        string IconDirPath = Path.Combine(BepInEx.Paths.PluginPath, "CustomLoadingIcon", "Icons", IconSet.Value);
        Logger.LogInfo(IconDirPath);
        Logger.LogInfo(CustomIconHandler.TrySetIcon(IconDirPath));

    }
}
