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
    internal static new ManualLogSource Logger;
    private ConfigEntry<string> IconSet;
    private ConfigEntry<bool> ShowGlow;
    static string IconSetName = "BoyKisser";

    private void Awake()
    {
        Logger = base.Logger;
        Logger.LogInfo($"I'm awake uwu");
        Logger.LogInfo($"{BepInEx.Paths.PluginPath}");

        IconSet = Config.Bind<string>("General", "Icont Set Name", "Default", "what icon set to use");
        ShowGlow = Config.Bind<bool>("General", "Show Glow", false, "should it show glow");

        IconSetName = IconSet.Value;
    }

    private void Start()
    {
        string IconSetPath = Path.Combine(BepInEx.Paths.PluginPath, "CustomLoadingIcon", "Icons", IconSetName);
        Console.WriteLine(IconSetPath);
        Console.WriteLine($"{TrySomething(IconSetPath)}");
    }

    bool TrySomething(string path)
    {
        try
        {
            if (!Directory.Exists(path)) return false;

            string[] spritePaths = Directory.GetFiles(path, "*.png");

            if (spritePaths.Length == 0) return false;

            List<Sprite> sprites = new List<Sprite>();

            foreach (string spritePath in spritePaths)
            {
                Byte[] imageData = File.ReadAllBytes(spritePath);
                Texture2D tex = new Texture2D(0, 0);
                ImageConversion.LoadImage(tex, imageData);
                tex.filterMode = FilterMode.Point;
                tex.Apply();

                Rect rect = new Rect(x: 0.00f, y: 0.00f, width: 256.00f, height: 256.00f);
                Vector2 pivot = new Vector2(0.5f, 0.5f);

                Sprite sprite = Sprite.Create(tex, rect, pivot);
                sprites.Add(sprite);

                Console.WriteLine($"Loaded sprite {Path.GetFileName(spritePath)}");
            }
            
            GameObject.Find("Game(Clone)/UI/Loading Icon-canvas/Glow").SetActive(ShowGlow.Value);
            Game.loadingIcon.animator.delay = 0.64f / sprites.Count;
            Game.loadingIcon.animator.image.color = Color.white;
            Game.loadingIcon.animator.sprites = sprites.ToArray();

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
        }
        return false;
    }

    // load images
    // make sprites
    // replace the laoding icons list
}
