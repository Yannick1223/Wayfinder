using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wayfinder.Model
{
    public class AStar: Pathfinder
    {
        public override List<Node>? FindPath(Node _start, Node _goal)
        {
            if (Costs == null || _start == null || _goal == null) return null;

            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();
            openList.Add(_start);

            while (openList.Count > 0)
            {
                openList.Sort((a, b) => b.F.CompareTo(a.F));

                Node current = openList[openList.Count - 1];

                openList.Remove(current);
                closedList.Add(current);

                if (FoundPath(current, _goal))
                {
                    return RetracePath(current);
                }

                foreach (Node neighbour in GetValidNeighbors(current))
                {
                    if (!IsCostOverZero(current.X, current.Y) || IsNodeInList(neighbour, closedList.ToList()))
                    {
                        continue;
                    }

                    bool nodeInOpenList = IsNodeInList(neighbour, openList);
                    float newCostToNeighbour = (current.G + Heuristic(current, _goal));

                    if (newCostToNeighbour < current.F || !nodeInOpenList)
                    {
                        neighbour.G = current.G + Costs[neighbour.X, neighbour.Y];
                        neighbour.H = Heuristic(current, neighbour);

                        neighbour.Parent = current;
                        if (!nodeInOpenList)
                        {
                            openList.Add(neighbour);
                        }
                    }
                }
            }

            return null;
        }

        private float Heuristic(Node _start, Node _goal)
        {
            return (float)(MathF.Abs(_start.X - _goal.X) + MathF.Abs(_start.Y - _goal.Y));
        }

        private List<Node> GetValidNeighbors(Node _node)
        {
            List<Node> result = new List<Node>();

            int[] dirX = { -1, 1, 0, 0 };
            int[] dirY = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int x = _node.X + dirX[i];
                int y = _node.Y + dirY[i];

                if (IsValidPosition(x, y))
                {
                    result.Add(new Node(x, y));
                }
            }
            return result;
        }

        private bool IsNodeInList(Node _node, List<Node> _list)
        {
            bool result = false;

            foreach (Node node in _list)
            {
                if (_node.X == node.X && _node.Y == node.Y)
                {
                    result = true;
                    break;
                }
            }
            return result;
        }

        private bool IsValidPosition(int _x, int _y)
        {
            return IsXPositionValid(_x) && IsYPositionValid(_y);
        }

        private bool IsXPositionValid(int _x)
        {
            return _x >= 0 && _x < Costs.GetLength(0);
        }

        private bool IsYPositionValid(int _y)
        {
            return _y >= 0 && _y < Costs.GetLength(1);
        }

        private bool IsCostOverZero(int _x, int _y)
        {
            return Costs[_x, _y] > 0;
        }

        private bool FoundPath(Node _current, Node _goal)
        {
            return _current.X == _goal.X && _current.Y == _goal.Y;
        } 
    }
}
