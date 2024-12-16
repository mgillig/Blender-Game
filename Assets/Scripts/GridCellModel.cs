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
}
