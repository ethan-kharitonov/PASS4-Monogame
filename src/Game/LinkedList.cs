using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PASS4
{
    class LinkedList<T>
    {
        class Node
        {
            public Node Next;
            public T Data;

            public Node(Node next, T data)
            {
                Next = next;
                Data = data;
            }

            public Node(T data)
            {
                Data = data;
            }
        }

        private Node head;
        public T Head => head != null ? head.Data : throw new NullReferenceException("Head was null");

        public void Clear() => head = null;

        public bool IsEmpty => head == null;
        private int count = 0;
        public void AddToTail(T data)
        {
            if (head == null) 
            {
                head = new Node(data);
            }
            else
            {
                QueryNode(n => n.Next == null).Next = new Node(data);
            }
            ++count;
        }

        public void RemoveFromTail(T data)
        {
            if (head == null)
            {
                return;
            }

            QueryNode(n => n.Next == null).Next = null;
            --count;
        }

        public T RemoveFromHead()
        {
            if(head == null)
            {
                throw new InvalidOperationException("The linked list is empty");
            }

            T data = head.Data;
            head = head.Next;

            --count;
            return data;
        }

        public (T, bool found) QueryData(Func<T, bool> predicate)
        {
            Node node = QueryNode(n => predicate(n.Data));
            return node == null ? (default, false) : (node.Next.Data, true);
        }

        private Node QueryNode(Func<Node, bool> predicate)
        {
            foreach(Node node in YieldNodes())
            {
                if (predicate(node))
                {
                    return node;
                }
            }

            return null;
        }

        private IEnumerable<Node> YieldNodes()
        {
            Node curNode = head;
            while(curNode != null)
            {
                yield return curNode;
                curNode = curNode.Next;
            }
        }
        public override string ToString()
        {
            string output = string.Empty;

            foreach(T data in YieldNodes().Select(n => n.Data))
            {
                output += data + ", ";
            }

            return output;
        }

        public int Count => count;
    }
}
