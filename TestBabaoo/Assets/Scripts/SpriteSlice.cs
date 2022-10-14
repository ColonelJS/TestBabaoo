using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditorInternal;
using System.Linq;
using UnityEditor;

public class SpriteSlice : MonoBehaviour
{

    private void Start()
    {
        sliceSprite();
    }

    void sliceSprite()
    {
        Debug.Log("ComputeSprites: start");

        int sliceWidth = 64;
        int sliceHeight = 64;

        string folder = "Assets/Resources/Sprite/";

        Texture2D[] textures = Resources.LoadAll(folder, typeof(Texture2D)).Cast<Texture2D>().ToArray();
        Debug.Log("ComputeSprites: textures.Length: " + textures.Length);

        List<string> allowlist = new List<string>{
            "character15",
            // "character28", // will not be updated by this script
            "character41",
        };

        foreach (Texture2D texture in textures)
        {
            //Texture2D tex = resource as Texture2D;
            if (!allowlist.Contains(texture.name))
            {
                continue;
            }
            Debug.Log("ComputeSprites:name: " + texture.name);
            string path = AssetDatabase.GetAssetPath(texture);
            TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
            ti.isReadable = true;
            //  constants
            ti.textureType = TextureImporterType.Sprite;
            ti.spriteImportMode = SpriteImportMode.Multiple;
            ti.spritePixelsPerUnit = 73;
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;

            List<SpriteMetaData> newData = new List<SpriteMetaData>();

            Rect[] rects = InternalSpriteUtility.GenerateGridSpriteRectangles(
                 texture, Vector2.zero, new Vector2(sliceWidth, sliceHeight), Vector2.zero);
            for (int i = 0; i < rects.Length; i++)
            {
                SpriteMetaData smd = new SpriteMetaData();
                smd.rect = rects[i];
                smd.pivot = new Vector2(0.5f, 0.5f);
                smd.alignment = (int)SpriteAlignment.Center;
                smd.name = texture.name + "_" + i; // name_41
                newData.Add(smd);
            }

            ti.spritesheet = newData.ToArray();
            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate); // this takes time, approx. 3s per Asset
            Debug.Log("ComputeSprites: resource ok");
        }
        Debug.Log("ComputeSprites: done");
    }

}
