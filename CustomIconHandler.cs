using System;
using System.IO;
using System.Xml.Serialization;
using System.Collections.Generic;
using UnityEngine;

namespace CustomLoadingIcon
{
    static class CustomIconHandler
    {
        public static bool TrySetIcon(string dirPath)
        {
            if (!Directory.Exists(dirPath)) return false;

            string xmlPath = Path.Combine(dirPath, "config.xml");

            if (!File.Exists(xmlPath))
            {

                Console.WriteLine("no config.xml file");
                return false;
            }

            string xml = File.ReadAllText(xmlPath);
            IconConfig iconConfig;

            XmlSerializer serializer = new XmlSerializer(typeof(IconConfig));
            using (StringReader reader = new StringReader(xml))
            {
                iconConfig = (IconConfig)serializer.Deserialize(reader);
            }

            if (!TryLoadIcons(dirPath, iconConfig, out Sprite[] sprites)) return false;

            GameObject.Find("Game(Clone)/UI/Loading Icon-canvas/Glow").SetActive(iconConfig.showGlow);

            Game.loadingIcon.animator.delay = iconConfig.delay;
            Game.loadingIcon.animator.image.color = iconConfig.color;
            Game.loadingIcon.animator.sprites = sprites;

            return true;
        }

        static bool TryLoadIcons(string dirPath, IconConfig iconConfig, out Sprite[] spritesArray)
        {
            spritesArray = new Sprite[0];
            
            try
            {

                if (!Directory.Exists(dirPath))
                {
                    Plugin.Logger.LogWarning($"no directory {Path.GetDirectoryName(dirPath)}");
                    return false;
                }

                string[] spritePaths = Directory.GetFiles(dirPath, "*.png");

                if (spritePaths.Length == 0)
                {
                    Plugin.Logger.LogWarning($"no images");
                    return false;

                }

                List<Sprite> sprites = new List<Sprite>();



                if (iconConfig.iconMode == IconMode.single)
                {
                    foreach (string spritePath in spritePaths)
                    {
                        Byte[] imageData = File.ReadAllBytes(spritePath);
                        Texture2D tex = new Texture2D(0, 0);
                        try
                        {
                            ImageConversion.LoadImage(tex, imageData);
                        }
                        catch (Exception ex)
                        {
                            Plugin.Logger.LogError($"Failed to load image {spritePath}: {ex.Message}");
                            return false;
                        }
                        tex.filterMode = FilterMode.Point;
                        tex.Apply();

                        Rect rect = new Rect(x: 0.00f, y: 0.00f, width: 256.00f, height: 256.00f);
                        Vector2 pivot = new Vector2(0.5f, 0.5f);

                        Sprite sprite = Sprite.Create(tex, rect, pivot);
                        sprites.Add(sprite);

                        Console.WriteLine($"Loaded sprite {Path.GetFileName(spritePath)}");
                    }

                    spritesArray = sprites.ToArray();
                    return true;
                }
                else if (iconConfig.iconMode == IconMode.spriteSheet)
                {
                    Byte[] imageData = File.ReadAllBytes(spritePaths[0]);
                    Texture2D tex = new Texture2D(0, 0);
                    try
                    {
                        ImageConversion.LoadImage(tex, imageData);
                    }
                    catch (Exception ex)
                    {
                        Plugin.Logger.LogError($"Failed to load image {spritePaths[0]}: {ex.Message}");
                        return false;
                    }
                    tex.filterMode = FilterMode.Point;

                    int cols = tex.width / 256;
                    int rows = tex.height / 256;


                    for (int col = 0; col < cols; col++)
                    {
                        for (int row = 0; row < rows; row++)
                        {
                            Rect rect = new Rect(x: col * 256.00f, y: row * 256.00f, width: 256.00f, height: 256.00f);
                            Vector2 pivot = new Vector2(0.5f, 0.5f);

                            Sprite sprite = Sprite.Create(tex, rect, pivot);
                            sprites.Add(sprite);

                            Console.WriteLine($"Loaded sprite at {col * 256} {row * 256}");

                        }
                    }

                    spritesArray = sprites.ToArray();
                    return true;
                }

            }
            catch (Exception ex)
            {
                Plugin.Logger.LogInfo(ex);
            }

            return false;
        }

    }

    struct IconConfig
    {
        public IconMode iconMode;
        public bool showGlow;
        public float delay;
        public Color color;

        public IconConfig(IconMode iconMode_, bool showGlow_, float delay_, Color color_)
        {
            iconMode = iconMode_;
            showGlow = showGlow_;
            delay = delay_;
            color = color_;
        }
    }

    enum IconMode
    {
        single,
        spriteSheet,
    }
}


/*
            GameObject.Find("Game(Clone)/UI/Loading Icon-canvas/Glow").SetActive(ShowGlow.Value);
            Game.loadingIcon.animator.delay = 0.64f / sprites.Count;
            Game.loadingIcon.animator.image.color = Color.white;
            Game.loadingIcon.animator.sprites = sprites.ToArray();
*/
