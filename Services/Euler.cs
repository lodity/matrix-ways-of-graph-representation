using CDM_Lab_3._1.Models.Graph;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Services
{
    public enum EulerType
    {
        Cycle,
        Path,
        None
    }
    public static class Euler
    {
        public static int CountNodesWithEvenEdges(Graph graph, out Node start, out Node finish)
        {
            int countEvenEdges = 0;
            start = null;
            finish = null;
            foreach (Node node in graph)
            {
                int nodeChildrenCount = node.Children.Count;
                foreach (var child in node.Children)
                {
                    if (child.Item1 == node.Id)
                        nodeChildrenCount--;
                }
                if (nodeChildrenCount % 2 == 0)
                    countEvenEdges++;
                else if (nodeChildrenCount % 2 == 1)
                {
                    if (start == null)
                        start = node;
                    else finish = node;
                }
            }
            if (countEvenEdges == graph.Count)
            {
                start = graph.Nodes[0];
                finish = graph.Nodes[0];
            }
            return countEvenEdges;
        }
        public static int CountPaths(Node start, Node end)
        {
            Dictionary<Node, bool> visited = new Dictionary<Node, bool>();
            return DFSCountPaths(start, end, visited);
        }
        private static int DFSCountPaths(Node current, Node end, Dictionary<Node, bool> visited)
        {
            if (current == end)
            {
                return 1;
            }

            visited[current] = true;

            int pathsCount = 0;

            foreach (var neighbor in current.Children)
            {
                if (!visited.ContainsKey(neighbor.Item2) || !visited[neighbor.Item2])
                {
                    pathsCount += DFSCountPaths(neighbor.Item2, end, visited);
                }
            }

            visited[current] = false;

            return pathsCount;
        }
        public static List<string> FindEulerianCycle(Graph graph, out EulerType eulerType)
        {
            List<string> result = new();

            foreach (Node node in graph.Nodes)
                if (node.Children.Count == 0)
                {
                    eulerType = EulerType.None;
                    return result;
                }

            Node start = graph.Nodes[0];
            Node finish = graph.Nodes[0];
            if (CountNodesWithEvenEdges(graph, out start, out finish) < graph.Count - 2)
            {
                eulerType = EulerType.None;
                return result;
            }
            if (start == finish)
                eulerType = EulerType.Cycle;
            else
                eulerType = EulerType.Path;

            Node temp = start;
            Node tempChild;
            while (true)
            {
                if (temp.HasChild(temp))
                {
                    List<int> loops = temp.RemoveAllLoops();
                    result = result.Concat(loops.Select(el => $"x{temp.Id}x{temp.Id} : a{el}")).ToList();
                }

                if (temp.Children.Count == 1)
                {
                    if (temp.Children[0].Item2 == finish && temp.Children.Count == 0)
                    {
                        result.Add($"x{temp.Id}x{temp.Children[0].Item1} : {temp.Edges[0].Item1}");
                        break;
                    }
                    tempChild = temp.Children[0].Item2;
                    result.Add($"x{temp.Id}x{temp.Children[0].Item1} : a{temp.Edges[0].Item1}");
                    tempChild.RemoveChild(temp.Id, temp.Edges[0].Item1);
                    temp.RemoveChild(temp.Children[0].Item1, temp.Edges[0].Item1);
                    temp = tempChild;
                    continue;
                }

                int countNonFinishChildren = temp.Children.Count(el => el.Item2 != finish);
                for (int i = 0; i < temp.Children.Count; i++)
                {
                    if ((temp.Children[i].Item2 != finish || countNonFinishChildren > 1) && CountPaths(temp, temp.Children[i].Item2) >= 2)
                    {
                        tempChild = temp.Children[i].Item2;
                        result.Add($"x{temp.Id}x{temp.Children[i].Item1} : a{temp.Edges[i].Item1}");
                        tempChild.RemoveChild(temp.Id, temp.Edges[i].Item1);
                        temp.RemoveChild(tempChild.Id, temp.Edges[i].Item1);
                        temp = tempChild;
                        break;
                    }
                }
                if (temp == finish && temp.Children.Count == 0)
                    break;
            }
            return result;
        }
    }
}
