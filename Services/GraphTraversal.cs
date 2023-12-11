using CDM_Lab_3._1.Models.Graph;
using System.Collections.Generic;

namespace CDM_Lab_3._1.Services
{
    internal class GraphTraversal
    {
        public static List<int> DepthFirstSearch(Graph graph, int startNodeId)
        {
            List<int> visited = new();
            DFS(graph, startNodeId, visited);
            return visited;
        }

        private static void DFS(Graph graph, int nodeId, List<int> visited)
        {
            visited.Add(nodeId);

            Node? node = graph.Nodes.Find(n => n.Id == nodeId);
            if (node != null)
            {
                foreach (var child in node.Children)
                {
                    if (!visited.Contains(child.Item2.Id))
                    {
                        DFS(graph, child.Item2.Id, visited);
                    }
                }
            }
        }

        public static List<int> BreadthFirstSearch(Graph graph, int startNodeId)
        {
            List<int> visited = new();
            Queue<int> queue = new();

            visited.Add(startNodeId);
            queue.Enqueue(startNodeId);

            while (queue.Count > 0)
            {
                int nodeId = queue.Dequeue();

                Node? node = graph.Nodes.Find(n => n.Id == nodeId);
                if (node != null)
                {
                    foreach (var child in node.Children)
                    {
                        if (!visited.Contains(child.Item2.Id))
                        {
                            visited.Add(child.Item2.Id);
                            queue.Enqueue(child.Item2.Id);
                        }
                    }
                }
            }
            return visited;
        }
    }
}
