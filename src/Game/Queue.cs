//Author name: Ethan Kharitonov
//Project name: PASS4
//File name: Queue.cs
//Date Created: January 17th, 2021
//Date Modified: January 27th, 2021
//Description: Generic queue data structre. Based on a linked list

namespace PASS4
{
    public class Queue<T>
    {
        /// <summary>
        /// The underlying data structre (linked list). Stores all the items of the queue
        /// </summary>
        LinkedList<T> items = new LinkedList<T>();

        /// <summary>
        /// Returns the number of items in the queue
        /// </summary>
        public int Count => items.Count;

        /// <summary>
        /// Returns true if the queue is empty, false otherwise
        /// </summary>
        public bool IsEmpty => items.IsEmpty;

        /// <summary>
        /// Adds an item to the queue by adding it to the end of the list
        /// </summary>
        /// <param name="item">The item that will be added</param>
        public void Enqueue(T item) => items.AddToTail(item);

        /// <summary>
        /// Removes and returns the item at the front of the queue/ front of the linked list
        /// </summary>
        /// <returns>The item that was dequeued</returns>
        public T Dequeue() => items.RemoveFromHead();

        /// <summary>
        /// Converts the queue into a string by converting its linked list into a string
        /// </summary>
        /// <returns>A string representing all the items in the queue</returns>
        public override string ToString() => items.ToString();

        /// <summary>
        /// Removes all the items in the queue by clearign the linked list
        /// </summary>
        public void Clear() => items.Clear();
    }
}
