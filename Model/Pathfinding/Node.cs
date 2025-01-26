using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model.Pathfinding
{
    public class Node
    {
        public int X { get; set; }
        public int Y { get; set; }

        /// <summary>
        /// Cost from Startpoint
        /// </summary>
        public float G { get; set; }
        /// <summary>
        /// Heuristic Cost
        /// </summary>
        public float H { get; set; }
        /// <summary>
        /// Total Cost
        /// </summary>
        public float F => G + H;

        public Node? Parent { get; set; }

        public Node(int x, int y)
        {
            X = x;
            Y = y;
            G = 0;
            H = 0;
            Parent = null;
        }
    }
}
