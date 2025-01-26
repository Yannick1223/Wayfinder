using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Wayfinder.Model
{
    public class PathfinderHandler
    {
        private Pathfinder Pathfinder { get; set; }

        public PathfinderHandler(Pathfinder _pathfinder)
        {
            Pathfinder = _pathfinder;
        }

        public void SetPathfinderAlgorithm(Pathfinder _pathfinder)
        {
            if (_pathfinder == null) throw new ArgumentNullException("Pathfinder is null.");
            Pathfinder = _pathfinder;
        }

        public List<Node>? GetPath(Point _from, Point _to, int[,] _costs)
        {
            return GetPath(new Node((int)_from.X, (int)_from.Y), new Node((int)_to.X, (int)_to.Y), _costs);
        }

        public List<Node>? GetPath(Node _from, Node _to, int[,] _costs)
        {
            Pathfinder.SetCosts(_costs);
            return Pathfinder.FindPath(_from, _to);
        }
    }
}
