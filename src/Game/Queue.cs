using System;
using System.Collections.Generic;
using System.Text;

namespace PASS4
{
    public class Queue<T>
    {
        LinkedList<T> items = new LinkedList<T>();

        public int Count => items.Count;

        public bool IsEmpty => items.IsEmpty;
        public void Enqueue(T item) => items.AddToTail(item);

        public T Dequeue() => items.RemoveFromHead();

        public T Peek => items.Head;

        public override string ToString() => items.ToString();

        public void Clear() => items.Clear();
    }
}
