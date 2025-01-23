using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wayfinder.Model.Pathfinding;

namespace Wayfinder.Model
{
    public class AStar: Pathfinder
    {
        private float[,] BestF {  get; set; }

        private float Heuristic(Node start, Node goal)
        {
            return (float)(MathF.Abs(start.X - goal.X) + MathF.Abs(start.Y - goal.Y));
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

        private List<Node> GetValidNeighbors(Node _node)
        {
            List<Node> result = new List<Node>();

            int[] dirX = { -1, 1, 0, 0 };
            int[] dirY = { 0, 0, -1, 1 };

            for (int i = 0; i < 4; i++)
            {
                int x = _node.X + dirX[i];
                int y = _node.Y + dirY[i];

                if(IsValidPosition(x, y))
                {
                    result.Add(new Node(x, y));
                }
            }
            return result;
        }

        private bool IsNodeInClosedList(Node _node,HashSet<Node> _closedList)
        {
            bool result = false;

            foreach (Node node in _closedList)
            {
                if(_node.X == node.X && _node.Y == node.Y)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        private bool IsNodeInOpenList(Node _node, List<Node> _openList)
        {
            bool result = false;

            foreach (Node node in _openList)
            {
                if (_node.X == node.X && _node.Y == node.Y)
                {
                    result = true;
                    break;
                }
            }

            return result;
        }

        public override List<Node>? FindPath(Node start, Node goal)
        {
            if (Costs == null) return null;

            List<Node> openList = new List<Node>();
            HashSet<Node> closedList = new HashSet<Node>();
            openList.Add(start);

            while (openList.Count > 0)
            {
                openList.Sort((a, b) => b.F.CompareTo(a.F));

                Node current = openList[openList.Count - 1];

                openList.Remove(current);
                closedList.Add(current);

                if(FoundPath(current, goal))
                {
                    return RetracePath(current);
                }

                foreach (Node neighbour in GetValidNeighbors(current))
                {
                    if(!IsCostOverZero(current.X, current.Y) || IsNodeInClosedList(neighbour ,closedList))
                    {
                        continue;
                    }
                    
                    bool nodeInOpenList = IsNodeInOpenList(neighbour ,openList);
                    float newCostToNeighbour = (current.G + Heuristic(current,  goal));

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

        private bool FoundPath(Node _current, Node _goal)
        {
            return _current.X == _goal.X && _current.Y == _goal.Y;
        }
    }
}
