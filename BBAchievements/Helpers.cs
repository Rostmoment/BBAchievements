﻿using MTM101BaldAPI.AssetTools;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace BBAchievements
{
    public static class Helpers
    {
        public static List<List<T>> SplitList<T>(this List<T> values, int chunkSize)
        {
            List<List<T>> res = new List<List<T>>();
            for (int i = 0; i < values.Count; i += chunkSize)
            {
                res.Add(values.GetRange(i, System.Math.Min(chunkSize, values.Count - i)));
            }
            return res;
        }
        public static List<Sprite> CreateSpriteSheet(Texture2D texture, int tilesByX, int tilesByY, float pixelsPerUnit = 1f)
        {
            List<Sprite> result = new List<Sprite>();
            int XSize = texture.width / tilesByX;
            int YSize = texture.height / tilesByY;
            for (int y = 0; y < tilesByY; y++)
            {
                for (int x = 0; x < tilesByX; x++)
                {
                    result.Add(Sprite.Create(texture, new Rect(x * XSize, y * YSize, XSize, YSize), Vector2.one / 2f, pixelsPerUnit, 0, SpriteMeshType.FullRect));
                }
            }
            return result;
        }
        public static void AddFromResources<T>(this AssetManager asset, string resourseName, string name = null) where T : Object
        {
            if (name == null) name = resourseName;
            asset.Add<T>(name, LoadAsset<T>(resourseName));
        }
        public static T LoadAsset<T>(string name) where T : Object
        {
            return (from x in Resources.FindObjectsOfTypeAll<T>()
                    where x.name.ToLower() == name.ToLower()
                    select x).First();
        }
    }
}
