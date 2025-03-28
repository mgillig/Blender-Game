using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Scripts
{
    public class GridCellModel
    {
        public bool NWallActive { get; set; } = true;
        public bool SWallActive { get; set; } = true;
        public bool EWallActive { get; set; } = true;
        public bool WWallActive { get; set; } = true;
        public bool IsFrontier { get; set; } = true;
    }

    public class GridCellNavModel
    {
        public Vector2Int Location { get; set; }
        public int Frontier { get; set; }

        public GridCellNavModel(Vector2Int location)
        {
            Location = location;
        }

        public override bool Equals(object obj)
        {
            var gridCellNavModel = obj as GridCellNavModel;
            var vector2Int = obj as Vector2Int?;

            return (gridCellNavModel != null && gridCellNavModel.Location == Location) ||
                (vector2Int.HasValue && vector2Int.Value == Location);
        }
    }
}
