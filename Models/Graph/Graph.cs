using System.Collections;
using System.Collections.Generic;

namespace CDM_Lab_3._1.Models.Graph
{
    internal class Graph : IEnumerable
    {
        public List<Node> Nodes = new();
        public Graph()
        {
        }
        public void AddNode(Node node)
        {
            Nodes.Add(node);
        }
        public void AddNode()
        {
            Node node = new(Nodes.Count - 1);
            Nodes.Add(node);
        }
        public IEnumerator GetEnumerator()
        {
            return Nodes.GetEnumerator();
        }
    }
}
