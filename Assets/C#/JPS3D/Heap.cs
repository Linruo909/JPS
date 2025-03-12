using System;
using System.Collections.Generic;

public class Heap<T> where T : IComparable<T>
{
    private List<T> items;
    public int Count => items.Count;

    public Heap(int capacity)
    {
        items = new List<T>(capacity);
    }

    public void Add(T item)
    {
        items.Add(item);
        int i = items.Count - 1;
        while (i > 0)
        {
            int parent = (i - 1) / 2;
            if (items[i].CompareTo(items[parent]) >= 0) break;
            Swap(i, parent);
            i = parent;
        }
    }

    public T RemoveFirst()
    {
        T first = items[0];
        items[0] = items[items.Count - 1];
        items.RemoveAt(items.Count - 1);
        HeapifyDown(0);
        return first;
    }

    private void HeapifyDown(int i)
    {
        while (true)
        {
            int left = 2 * i + 1;
            int right = 2 * i + 2;
            int smallest = i;

            if (left < items.Count && items[left].CompareTo(items[smallest]) < 0)
                smallest = left;
            if (right < items.Count && items[right].CompareTo(items[smallest]) < 0)
                smallest = right;

            if (smallest == i) break;
            Swap(i, smallest);
            i = smallest;
        }
    }

    private void Swap(int a, int b)
    {
        (items[a], items[b]) = (items[b], items[a]);
    }

    public bool Contains(T item) => items.Contains(item);
}