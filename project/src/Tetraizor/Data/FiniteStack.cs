using System;
using System.Collections;
using System.Collections.Generic;

public class FiniteStack<T> : IEnumerable<T>
{
    private T[] _items;
    private int _count;

    public int Count => _count;
    public int Capacity => _items.Length;

    public bool IsEmpty => _count == 0;
    public bool IsFull => _count == _items.Length;

    public FiniteStack(int capacity)
    {
        _items = new T[capacity];
        _count = 0;
    }

    public void Push(T item)
    {
        if (_count == _items.Length)
        {
            for (int i = 1; i < _items.Length; i++)
            {
                _items[i - 1] = _items[i];
            }

            _count--;
        }

        _items[_count] = item;
        _count++;
    }

    public T Pop()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        _count--;

        return _items[_count];
    }

    public T Peek()
    {
        if (_count == 0)
        {
            throw new InvalidOperationException("Stack is empty");
        }

        return _items[_count - 1];
    }

    public void Clear()
    {
        _count = 0;
    }

    #region IEnumerable Implementation
    IEnumerator<T> IEnumerable<T>.GetEnumerator()
    {
        for (int i = 0; i < _count; i++)
        {
            yield return _items[i];
        }
    }

    public IEnumerator GetEnumerator()
    {
        return ((IEnumerable<T>)this).GetEnumerator();
    }
    #endregion
}