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

        //Cost from Startpoint
        public float G { get; set; }
        //Heuristic Cost
        public float H { get; set; }
        //Total Cost
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

        public int CompareTo(Node _other)
        {
            return F.CompareTo(_other.F);
        }
    }
}
