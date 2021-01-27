//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: LinkedList.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Generic Linked list data structre (Includes Node class)
using System;
using System.Collections.Generic;
using System.Linq;

namespace PASS4
{
    class LinkedList<T>
    {
        /// <summary>
        /// Linked list node. sotres data and reference to another node
        /// </summary>
        class Node
        {
            //Next node and this node data
            public Node Next;
            public T Data;

            /// <summary>
            /// Crates a Node given data and a next node
            /// </summary>
            /// <param name="next">Next nope</param>
            /// <param name="data">Data</param>
            public Node(Node next, T data)
            {
                Next = next;
                Data = data;
            }

            /// <summary>
            /// Crates a Node given data
            /// </summary>
            /// <param name="data"Dataparam>
            public Node(T data)
            {
                Data = data;
            }
        }


        //The first/Head Node of the list
        private Node head;

        /// <summary>
        /// Returns the head if not null or throws an exception
        /// </summary>
        public T Head => head != null ? head.Data : throw new NullReferenceException("Head was null");

        /// <summary>
        /// Clears the list by setting head to null
        /// </summary>
        public void Clear() => head = null;

        /// <summary>
        /// Indicates if the list is empty by checking if head is null
        /// </summary>
        public bool IsEmpty => head == null;

        /// <summary>
        /// Adds a node with a given data to the end of the list
        /// </summary>
        /// <param name="data">The data of the node</param>
        public void AddToTail(T data)
        {
            //If head is null then the given node becomes head, otherwise adds to end
            if (head == null) 
            {
                head = new Node(data);
            }
            else
            {
                //Finds the last Node (node with no next Node)
                QueryNode(n => n.Next == null).Next = new Node(data);
            }

            //Increments the number of Nodes in the list
            ++Count;
        }

        /// <summary>
        /// Removes a node from the front of the list
        /// </summary>
        /// <returns>The node that used to be the front</returns>
        public T RemoveFromHead()
        {
            //If the list is empty, throw an exception
            if(head == null)
            {
                throw new InvalidOperationException("The linked list is empty");
            }

            //Saves the head nope and removes it from the list by making the next node the head
            T data = head.Data;
            head = head.Next;


            //Drements the number of Nodes in the list and returns the node that was removed
            --Count;
            return data;
        }

        /// <summary>
        /// Finds a Node that fits a given predicate
        /// </summary>
        /// <param name="predicate">The condition the Node has to meet</param>
        /// <returns>The node that fits if it exists, null otherwise</returns>
        private Node QueryNode(Func<Node, bool> predicate)
        {
            //Checks each node untill it finds one that fits or runs out of nodes
            foreach(Node node in YieldNodes())
            {
                //Checks if it fits and returns it if it does
                if (predicate(node))
                {
                    return node;
                }
            }

            //Return null if no node has been found
            return null;
        }

        /// <summary>
        /// Yields all the Nodes in the linked list
        /// </summary>
        /// <returns>An IEnumerable containing all the Nodes in the list</returns>
        private IEnumerable<Node> YieldNodes()
        {
            //stores the current node starting from head
            Node curNode = head;

            //loops as long as the current node is not null
            while(curNode != null)
            {
                //yields the current node and sets the current node equal to the next node
                yield return curNode;
                curNode = curNode.Next;
            }
        }
        
        /// <summary>
        /// Converts a Linked List to a String (used in unit tests)
        /// </summary>
        /// <returns>A string containing the data of all the Nodes in the list</returns>
        public override string ToString()
        {
            //Creates and empty string
            string output = string.Empty;

            //Adds the data of each Node onto the string followed by a comma
            foreach(T data in YieldNodes().Select(n => n.Data))
            {
                output += data + ", ";
            }

            //Returns the string
            return output;
        }

        /// <summary>
        /// Stores the number of Nodes inside the list
        /// </summary>
        public int Count { get; private set; } = 0;
    }
}
