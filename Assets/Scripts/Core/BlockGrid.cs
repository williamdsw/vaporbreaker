using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class BlockGrid
    {
        // || Properties

        public static Dictionary<Vector3, Block> Grid { get; private set; }
        public static Vector2 MinCoordinatesInXY => new Vector2(-13f, -7.5f);
        public static Vector2 MaxCoordinatesInXY => new Vector2(13f, 7.5f);

        /// <summary>
        /// Initialize Grid with positions
        /// </summary>
        public static void InitGrid()
        {
            Grid = new Dictionary<Vector3, Block>();
            for (float x = MinCoordinatesInXY.x; x <= MaxCoordinatesInXY.x; x++)
            {
                for (float y = MinCoordinatesInXY.y; y <= MaxCoordinatesInXY.y; y += 0.5f)
                {
                    Debug.Log(new Vector3(x, y, 0));
                    Grid.Add(new Vector3(x, y, 0), null);
                }
            }
        }

        /// <summary>
        /// Put block at position
        /// </summary>
        /// <param name="position"> Desired position </param>
        /// <param name="block"> Desired block </param>
        public static void PutBlock(Vector3 position, Block block) => Grid.Add(position, block);

        /// <summary>
        /// Check if position exists
        /// </summary>
        /// <param name="position"> Desired position </param>
        /// <returns> true | false </returns>
        public static bool CheckPosition(Vector3 position) => Grid.ContainsKey(position);

        /// <summary>
        /// Get block at position
        /// </summary>
        /// <param name="position"> Desired position </param>
        /// <returns> Instance of Block </returns>
        public static Block GetBlock(Vector3 position) => Grid[position];

        /// <summary>
        /// Redefine block at position
        /// </summary>
        /// <param name="position"> Desired position </param>
        /// <param name="block"> Instance of Block </param>
        public static void RedefineBlock(Vector3 position, Block block) => Grid[position] = block;
    }
}