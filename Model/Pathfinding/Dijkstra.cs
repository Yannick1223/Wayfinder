using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public class Dijkstra: Pathfinder
    {
        public override List<Node>? FindPath(Node start, Node goal)
        {
            return base.FindPath(start, goal);
        }
    }
}
