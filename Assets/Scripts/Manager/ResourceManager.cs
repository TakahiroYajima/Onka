using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager
{
    public const string ItemResourcePath = "Sprites/Items/";
    public const string CharactorResourcePath = "Sprites/Charactors/";

    public static Sprite LoadResourceSprite(string resourcePath, string spriteName)
    {
        Texture2D tex = Resources.Load(resourcePath + spriteName) as Texture2D;
        Sprite sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.zero);
        return sprite;
    }
}
