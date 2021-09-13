
using System.Collections.Generic;

namespace MVC.Models
{
    public class Layout
    {
        public class BlockInfo
        {
            public class PositionInfo
            {
                public float X { get; set; }
                public float Y { get; set; }
                public float Z { get; set; }
            }

            public class ColorInfo
            {
                // || Properties

                public byte R { get; set; }
                public byte G { get; set; }
                public byte B { get; set; }
            }

            // || Fields


            // || Properties

            public string PrefabName { get; set; }
            public PositionInfo Position { get; set; }
            public ColorInfo Color { get; set; }
        }

        // || Properties

        public List<BlockInfo> Blocks { get; set; } = new List<BlockInfo>();
        public bool HasPrefabSpawner { get; set; } = false;
        public bool CanChooseRandomBlocks { get; set; } = false;
    }
}