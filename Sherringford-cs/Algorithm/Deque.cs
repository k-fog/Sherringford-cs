using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Sherringford.Algorithm
{
  class Deque<T> : IEnumerable<T>
  {
    public T this[int i]
    {
      get { return this.Buffer[(this.FirstIndex + i) % this.Capacity]; }
      set
      {
        if (i < 0) throw new ArgumentOutOfRangeException();
        this.Buffer[(this.FirstIndex + i) % this.Capacity] = value;
      }
    }
    private T[] Buffer;
    private int Capacity;
    private int FirstIndex;
    private int LastIndex
    {
      get { return (this.FirstIndex + this.Length) % this.Capacity; }
    }
    public int Length;
    public Deque(int capacity = 16)
    {
      this.Capacity = capacity;
      this.Buffer = new T[this.Capacity];
      this.FirstIndex = 0;
    }
    public void PushBack(T data)
    {
      if (this.Length == this.Capacity) this.Resize();
      this.Buffer[this.LastIndex] = data;
      this.Length++;
    }
    public void PushFront(T data)
    {
      if (this.Length == this.Capacity) this.Resize();
      var index = this.FirstIndex - 1;
      if (index < 0) index = this.Capacity - 1;
      this.Buffer[index] = data;
      this.Length++;
      this.FirstIndex = index;
    }
    public T PopBack()
    {
      if (this.Length == 0) throw new InvalidOperationException("deque: empty data");
      var data = this[this.Length - 1];
      this.Length--;
      return data;
    }
    public T PopFront()
    {
      if (this.Length == 0) throw new InvalidOperationException("deque: empty data");
      var data = this[0];
      this.FirstIndex++;
      this.FirstIndex %= this.Capacity;
      this.Length--;
      return data;
    }
    private void Resize()
    {
      var newArray = new T[this.Capacity * 2];
      for (int i = 0; i < this.Length; i++)
      {
        newArray[i] = this[i];
      }
      this.FirstIndex = 0;
      this.Capacity *= 2;
      this.Buffer = newArray;
    }
    public IEnumerator<T> GetEnumerator()
    {
      for (int i = 0; i < this.Length; i++)
      {
        yield return this[i];
      }
    }
    IEnumerator IEnumerable.GetEnumerator()
    {
      for (int i = 0; i < this.Length; i++)
      {
        yield return this[i];
      }
    }
  }
}
