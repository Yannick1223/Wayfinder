using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wayfinder.Model.Pathfinding;

namespace Wayfinder.Model
{
    public class Pathfinder
    {
        public virtual int[,] Costs { get; private set; }
        public Pathfinder()
        {

        }

        public virtual void SetCosts(int[,] _costs)
        {
            Costs = _costs;
        }

        public virtual List<Node>? FindPath(Node start, Node goal)
        {
            return null;
        }

        protected virtual List<Node> RetracePath(Node _path)
        {
            List<Node> result = new List<Node>();

            result.Add(_path);
            while (_path.Parent != null)
            {
                result.Add(_path.Parent);
                _path = _path.Parent;
            }
            result.Reverse();

            return result;
        }
    }
}
