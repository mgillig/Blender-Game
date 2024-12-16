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
        public GridCellModel() 
        {
            NWallActive = true;
            SWallActive = true;
            EWallActive = true;
            WWallActive = true;
            IsFrontier = true;
        }
        public bool NWallActive { get; set; }
        public bool SWallActive { get; set; }
        public bool EWallActive { get; set; }
        public bool WWallActive { get; set; }
        public bool IsFrontier { get; set; }
    }
}
