using System;
using System.Collections.Generic;
using System.Text;

namespace Game
{
    public class Queue<T>
    {
        LinkedList<T> items = new LinkedList<T>();

        public void Enqueue(T item) => items.AddToTail(item);

        public T Dequeue() => items.RemoveFromHead();

        public override string ToString()
        {
            return items.ToString();
        }
    }
}
