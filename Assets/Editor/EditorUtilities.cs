using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Core;
using MVC.Models;

public class EditorUtilities
{
    private static string content = string.Empty;

    [MenuItem("Utilities/Get Blocks")]
    private static void GetBlocks()
    {
        try
        {
            List<Layout.BlockInfo> blocks = new List<Layout.BlockInfo>();
            foreach (Block block in MonoBehaviour.FindObjectsOfType<Block>())
            {
                block.Start();
                block.DefineParticlesColor(block.BlockColor);
                blocks.Add(new Layout.BlockInfo()
                {
                    PrefabName = block.MaxHits.ToString() + "Hit",
                    Position = new Layout.BlockInfo.PositionInfo()
                    {
                        X = block.transform.position.x,
                        Y = block.transform.position.y,
                        Z = block.transform.position.z,
                    },
                    Color = new Layout.BlockInfo.ColorInfo()
                    {
                        R = block.ParticlesColor.r,
                        G = block.ParticlesColor.g,
                        B = block.ParticlesColor.b,
                    }
                });
            }

            Layout layout = new Layout()
            {
                HasPrefabSpawner = false,
                Blocks = blocks
            };

            content = Newtonsoft.Json.JsonConvert.SerializeObject(layout);
            content = content.Replace("Hit", "HitBlock");

            Debug.Log(content);

            Layout other = Newtonsoft.Json.JsonConvert.DeserializeObject<Layout>(content);
            Debug.Log(other.Blocks.Count);
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }

    [MenuItem("Utilities/Spawn Blocks")]
    private static void Instantiate()
    {
        Debug.Log("Instantiate");
        Debug.Log(content);
    }
}