using CDM_Lab_3._1.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Services
{
    public class SpanningTree
    {
        private Graph graph;

        public SpanningTree(Graph graph)
        {
            this.graph = graph;
        }

        public List<int> GetMinimumSpanningTreeEdgeIds()
        {
            List<int> selectedNodes = new();
            List<Tuple<int, Node, Node>> priorityQueue = new();
            List<int> result = new();

            selectedNodes.Add(graph.Nodes[0].Id);
            for (int i = 0; i < graph.Nodes[0].Children.Count; i++)
            {
                priorityQueue.Add(new Tuple<int, Node, Node>(graph.Nodes[0].Edges[i].Item3, graph.Nodes[0], graph.Nodes[0].Children[i].Item2));
            }
            priorityQueue = priorityQueue.OrderBy(x => x.Item1).ToList();

            while (selectedNodes.Count < graph.NodeCount)
            {
                if (!priorityQueue.Any())
                {
                    break;
                }

                Tuple<int, Node, Node> nextEdge = priorityQueue.First();
                priorityQueue.Remove(nextEdge);

                if (selectedNodes.Contains(nextEdge.Item3.Id))
                {
                    continue;
                }

                int? edgeId = FindEdgeId(nextEdge.Item2, nextEdge.Item3);
                if (edgeId.HasValue)
                {
                    result.Add(edgeId.Value);
                }

                selectedNodes.Add(nextEdge.Item3.Id);

                for (int i = 0; i < nextEdge.Item3.Children.Count; i++)
                {
                    if (!selectedNodes.Contains(nextEdge.Item3.Children[i].Item2.Id))
                    {
                        priorityQueue.Add(new Tuple<int, Node, Node>(nextEdge.Item3.Edges[i].Item3, nextEdge.Item3, nextEdge.Item3.Children[i].Item2));
                    }
                }
                priorityQueue = priorityQueue.OrderBy(x => x.Item1).ToList();
            }

            return result;
        }

        private static int? FindEdgeId(Node node1, Node node2)
        {
            foreach (var edge in node1.Edges)
            {
                if (node1.HasChild(edge.Item1) && node2.HasChild(edge.Item1))
                {
                    return edge.Item1;
                }
            }
            return null;
        }
    }
}
