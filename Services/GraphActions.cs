using CDM_Lab_3._1.Models;
using CDM_Lab_3._1.Models.Graph;
using System;
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
                                case GraphType.Undirected: edgeType = EdgeType.Undirected; break;
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
                            graph.Nodes[x].AddChild(graph.Nodes[y], new Tuple<int, EdgeType>(edgeCount++, edgeType));
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
            for (int i = 0; i < nodeCount; i++)
            {
                for (int j = 0; j < nodeCount; j++)
                {
                    if (Math.Sqrt(MatrixAdjacencyTable.Length) > i && Math.Sqrt(MatrixAdjacencyTable.Length) > j)
                        MatrixAdjacencyTableCopy[i, j] = MatrixAdjacencyTable[i, j];
                    else MatrixAdjacencyTableCopy[i, j] = 0;
                }
            }
            MatrixAdjacencyTable = MatrixAdjacencyTableCopy;
        }
        public static void GraphAddEdge(ref Graph graph, ref short[,] MatrixAdjacencyTable, int nodeIndexFrom, int nodeIndexTo, int edgeCount, GraphType GraphTypeCurrent)
        {
            bool isLoop = nodeIndexFrom == nodeIndexTo;
            EdgeType edgeType = GraphTypeCurrent == GraphType.Undirected ? EdgeType.Undirected : EdgeType.Directed;
            graph.Nodes[nodeIndexFrom].AddChild(graph.Nodes[nodeIndexTo], new Tuple<int, EdgeType>(edgeCount, isLoop ? EdgeType.Loop : edgeType));

            if (GraphTypeCurrent == GraphType.Undirected)
            {
                MatrixAdjacencyTable[nodeIndexFrom, nodeIndexTo]++;
                MatrixAdjacencyTable[nodeIndexTo, nodeIndexFrom]++;
            }
            else
                MatrixAdjacencyTable[nodeIndexFrom, nodeIndexTo]++;
        }
        public static void GraphAddEdge(ref short[,] MatrixAdjacencyTable, int nodeIndexFrom, int nodeIndexTo, GraphType GraphTypeCurrent)
        {
            if (GraphTypeCurrent == GraphType.Undirected)
            {
                MatrixAdjacencyTable[nodeIndexFrom, nodeIndexTo]++;
                MatrixAdjacencyTable[nodeIndexTo, nodeIndexFrom]++;
            }
            else
                MatrixAdjacencyTable[nodeIndexFrom, nodeIndexTo]++;
        }
    }
}
