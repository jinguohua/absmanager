using System.Collections.Generic;

namespace ChineseAbs.ABSManagement.Utils.TreeUtils
{
    public class Node<T>
    {
        public Node()
        {
            Nodes = new List<Node<T>>();
        }

        public string Name { get; set; }

        public List<Node<T>> Nodes { get; set; }

        public T Info { get; set; }
    }
}
