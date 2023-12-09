using CDM_Lab_3._1.Controls;
using CDM_Lab_3._1.Models.Graph;
using System.Collections.Generic;
using System.Linq;

namespace CDM_Lab_3._1.Services
{
    public static class GraphColoring
    {
        public static void ColorNodes(Graph graph)
        {
            List<Node> nodesByDegree = graph.Nodes.OrderByDescending(n => n.Children.Count).ToList();
            int color = 0;

            while (nodesByDegree.Any(n => n.Color == 0))
            {
                List<Node> availableNodes = nodesByDegree.Where(n => n.Color == 0).ToList();

                foreach (Node node in availableNodes)
                {
                    if (CanColorNodes(node, color, graph))
                    {
                        node.Color = color;
                    }
                }

                color++;
            }
        }

        private static bool CanColorNodes(Node node, int color, Graph graph)
        {
            foreach (var child in node.Children)
            {
                Node adjacentNode = graph.Nodes.First(n => n.Id == child.Item2.Id);
                if (adjacentNode.Color == color)
                {
                    return false;
                }
            }

            return true;
        }

        public static void ColorEdges(List<ControlEdge> controlEdges)
        {
            int color = 0;

            foreach (ControlEdge edge in controlEdges)
                edge.Color = GetAvailableEdgeColor(edge, controlEdges, color);
        }

        private static int GetAvailableEdgeColor(ControlEdge edge, List<ControlEdge> allEdges, int startColor)
        {
            int color = startColor;
            while (HasEdgeConflict(edge, allEdges, color))
            {
                color++;
            }
            return color;
        }
        private static bool HasEdgeConflict(ControlEdge edge, List<ControlEdge> allEdges, int color)
        {
            int startNodeId = edge.NodeStart.index;
            int endNodeId = edge.NodeEnd.index;

            foreach (ControlEdge otherEdge in allEdges)
            {
                if (otherEdge != edge && otherEdge.Color == color)
                {
                    if (otherEdge.NodeStart.index == startNodeId ||
                        otherEdge.NodeStart.index == endNodeId ||
                        otherEdge.NodeEnd.index == startNodeId ||
                        otherEdge.NodeEnd.index == endNodeId)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
