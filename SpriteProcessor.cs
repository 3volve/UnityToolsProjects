using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class SpriteProcessor : AssetPostprocessor
{
    void OnPostprocessTexture(Texture2D texture)
    {
        string lCAssetPath = assetPath.ToLower();
        bool isInSpritesDirectory = lCAssetPath.IndexOf("/sprites/") != -1;
        
        if(isInSpritesDirectory)
            ((TextureImporter)assetImporter).textureType = TextureImporterType.Sprite;
    }
}
