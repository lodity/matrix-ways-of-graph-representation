using CDM_Lab_3._1.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Services
{
    public static class ShortestPathFinder
    {
        public static List<Tuple<int, int>> FindShortestPath(int startNodeId, int endNodeID, Graph graph)
        {
            Node? startNode = graph.Nodes.First(el => el.Id == startNodeId);
            Node? endNode = graph.Nodes.First(el => el.Id == endNodeID);

            List<Node> nodes = TOPOLOGIC(graph);

            //foreach (var node in graph.Nodes)
            //{
            //    nodes.Add(node);
            //}

            // <key - node, value - shortest distance from start node>
            Dictionary<Node, int> distances = new();
            // <key - node, value - previous node>
            Dictionary<Node, Node> previousNodes = new();
            List<Node> visitedNodes = new();

            distances[startNode] = 0;

            while (true)
            {
                Node? currentNode = null;
                int currentDistance = int.MaxValue;

                // Find the unvisited nodes with the smallest edge weight
                foreach (var node in nodes.Where(n => !visitedNodes.Contains(n)))
                {
                    if (distances.ContainsKey(node) && distances[node] < currentDistance)
                    {
                        currentNode = node;
                        currentDistance = distances[node];
                    }
                }
                // If node wasn't found or it's endNode
                if (currentNode == null || currentNode == endNode)
                    break;

                visitedNodes.Add(currentNode);

                // Update distances for neighbors of the current node
                foreach (var edge in currentNode.Edges)
                {
                    Node neighbor = edge.Item4;
                    int newWeight = distances[currentNode] + edge.Item3;

                    if (!distances.ContainsKey(neighbor) || newWeight < distances[neighbor])
                    {
                        distances[neighbor] = newWeight;
                        previousNodes[neighbor] = currentNode;
                    }
                }
            }

            // Reconstructing the path from the end
            Node? current = endNode;
            // <edge id, edge weight>
            List<Tuple<int, int>> result = new();

            // Formation of the result
            while (current != null)
            {
                if (previousNodes.ContainsKey(current))
                    result.Add(previousNodes[current].LessWeightedEdge(current));
                current = previousNodes.ContainsKey(current) ? previousNodes[current] : null;
            }

            result.Reverse();
            return result;
        }

        public static List<Node> TOPOLOGIC(Graph graph)
        {
            List<int> visited = new();
            List<Node> order = new();
            foreach (var node in graph.Nodes)
            {
                if (!visited.Contains(node.Id))
                    DFS(graph, node.Id, visited, order);
            }
            return order;
        }

        private static void DFS(Graph graph, int nodeId, List<int> visited, List<Node> order)
        {
            visited.Add(nodeId);

            Node? node = graph.Nodes.Find(n => n.Id == nodeId);
            if (node == null)
                return;
            foreach (var child in node.Children)
                if (!visited.Contains(child.Item2.Id))
                    DFS(graph, child.Item2.Id, visited, order);

            order.Add(node);
        }
    }
}
