﻿using UnityEngine;
using System.Collections.Generic;

public class IndexedSpriteManager : MonoBehaviour
{
    public static IndexedSpriteManager Instance()
    {
        return _instance;
    }

    public static Texture2D GetAtlas(string path, string name)
    {
        return _instance._atlases[path][name];
    }

    public static Sprite[] GetSprites(string path, string atlasName)
    {
        return _instance._sprites[path][atlasName];
    }

    public static Sprite GetSprite(string path, string atlasName, string spriteName)
    {
        Sprite[] spriteGroup = _instance._sprites[path][atlasName];
        for (int i = 0; i < spriteGroup.Length; ++i)
        {
            if (spriteGroup[i].name == spriteName)
                return spriteGroup[i];
        }
        return null;
    }

    public static bool HasAtlas(string path, string atlasName)
    {
        return _instance._sprites[path].ContainsKey(atlasName);
    }

    public static List<Sprite> GetAllSpritesAtPath(string path)
    {
        List<Sprite> sprites = new List<Sprite>();
        foreach (Sprite[] spriteGroup in _instance._sprites[path].Values)
        {
            for (int i = 0; i < spriteGroup.Length; ++i)
                sprites.Add(spriteGroup[i]);
        }
        return sprites;
    }

    public PackedSpriteGroup[] SpriteGroups;

    void Awake()
    {
        _instance = this;
        aggregateInformation();
    }

    /**
     * Private
     */
    private static IndexedSpriteManager _instance;
    private Dictionary<string, Dictionary<string, Texture2D>> _atlases;
    private Dictionary<string, Dictionary<string, Sprite[]>> _sprites;

    private void aggregateInformation()
    {
        _atlases = new Dictionary<string, Dictionary<string, Texture2D>>();
        _sprites = new Dictionary<string, Dictionary<string, Sprite[]>>();

        for (int i = 0; i < this.SpriteGroups.Length; ++i)
        {
            for (int j = 0; j < this.SpriteGroups[i].Atlases.Length; ++j)
            {
                PackedSpriteGroup.AtlasEntry atlasEntry = this.SpriteGroups[i].Atlases[j];
                if (!_atlases.ContainsKey(atlasEntry.RelativePath))
                    _atlases.Add(atlasEntry.RelativePath, new Dictionary<string, Texture2D>());
                if (!_sprites.ContainsKey(atlasEntry.RelativePath))
                    _sprites.Add(atlasEntry.RelativePath, new Dictionary<string, Sprite[]>());
                _atlases[atlasEntry.RelativePath].Add(atlasEntry.AtlasName, atlasEntry.Atlas);
                _sprites[atlasEntry.RelativePath].Add(atlasEntry.AtlasName, atlasEntry.Sprites);
            }
        }
    }
}
