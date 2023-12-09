using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Models.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using static CDM_Lab_3._1.Models.Graph.Node;

namespace CDM_Lab_3._1.Services
{
    public static class GraphActions
    {
        public static Graph CreateGraph_AdjacencyBased(short[,] MatrixAdjacencyTable, GraphType GraphTypeCurrent)
        {
            int nodeCount = MatrixAdjacencyTable.GetLength(0);
            Graph graph = new(nodeCount);

            short[,] AdjacencyTableCopy = (short[,])MatrixAdjacencyTable.Clone();
            bool isNoZeroRemained;
            int edgeCount = 0;
            do
            {
                isNoZeroRemained = false;
                for (int x = 0; x < nodeCount; x++)
                {
                    for (int y = 0; y < nodeCount; y++)
                    {
                        if (AdjacencyTableCopy[x, y] > 0 && (GraphTypeCurrent != GraphType.Undirected || x >= y))
                        {
                            EdgeType edgeType = EdgeType.Directed;
                            switch (GraphTypeCurrent)
                            {
                                case GraphType.Undirected:
                                    {
                                        if (x == y)
                                            edgeType = EdgeType.Loop;
                                        else
                                            edgeType = EdgeType.Undirected;
                                    }
                                    break;
                                case GraphType.Mixed:
                                    {
                                        if (x == y)
                                        {
                                            AdjacencyTableCopy[x, y]--;
                                            edgeType = EdgeType.Loop;
                                            break;
                                        }
                                        if ((AdjacencyTableCopy[x, y] - AdjacencyTableCopy[y, x]) < 1)
                                        {
                                            AdjacencyTableCopy[x, y]--;
                                            AdjacencyTableCopy[y, x]--;
                                            edgeType = EdgeType.Undirected;
                                        }
                                        else if (AdjacencyTableCopy[x, y] > AdjacencyTableCopy[y, x]) AdjacencyTableCopy[x, y]--;
                                        else AdjacencyTableCopy[y, x]--;
                                    }
                                    break;
                            }
                            graph.Nodes[x].AddChild(graph.Nodes[y], new Tuple<int, EdgeType, int>(edgeCount++, edgeType, 1));
                        }
                        if (GraphTypeCurrent != GraphType.Mixed)
                            AdjacencyTableCopy[x, y]--;
                        if (AdjacencyTableCopy[x, y] > 0)
                            isNoZeroRemained = true;
                    }
                }
            } while (isNoZeroRemained);
            return graph;
        }
        public static void GraphAddNode(ref short[,] MatrixAdjacencyTable)
        {
            int nodeCount = MatrixAdjacencyTable.GetLength(0) + 1;
            short[,] MatrixAdjacencyTableCopy = new short[nodeCount, nodeCount];
            for (int x = 0; x < nodeCount; x++)
            {
                for (int y = 0; y < nodeCount; y++)
                {
                    if (nodeCount - 1 > x && nodeCount - 1 > y)
                        MatrixAdjacencyTableCopy[x, y] = MatrixAdjacencyTable[x, y];
                    else MatrixAdjacencyTableCopy[x, y] = 0;
                }
            }
            MatrixAdjacencyTable = MatrixAdjacencyTableCopy;
        }
        public static void GraphAddEdge(ref short[,] MatrixAdjacencyTable, int nodeIndexFrom, int nodeIndexTo, GraphType GraphTypeCurrent)
        {
            MatrixAdjacencyTable[nodeIndexFrom, nodeIndexTo]++;
            if (GraphTypeCurrent == GraphType.Undirected && nodeIndexFrom != nodeIndexTo)
                MatrixAdjacencyTable[nodeIndexTo, nodeIndexFrom]++;
        }
        public static Graph DoSymetricGraph(Graph graph)
        {

            List<Node> graphList = new List<Node>(graph.Nodes).Select(el => el = el.Copy()).ToList();
            foreach (Node node in graphList)
                for (int i = 0; i < node.Children.Count; i++)
                {
                    node.Children[i] = new Tuple<int, Node>(node.Children[i].Item1, graphList.First(el => el.Id == node.Children[i].Item2.Id));
                    if (!node.Children[i].Item2.HasChild(node.Edges[i].Item1))
                        node.Children[i].Item2.AddChild(node, new Tuple<int, EdgeType, int>(node.Edges[i].Item1, node.Edges[i].Item2, node.Edges[i].Item3));
                }

            Graph newGraph = new(0);
            foreach (Node node in graphList)
                newGraph.AddNode(node);
            return newGraph;
        }
    }
}
